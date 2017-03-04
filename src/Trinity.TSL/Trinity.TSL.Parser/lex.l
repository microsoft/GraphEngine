%top{
//Fix the INT{X}_{MIN,MAX} redefinition warnings, and define flex types in our c++ style
//TODO platform
#define FLEXINT_H
#include <cstdint>
typedef int8_t flex_int8_t;
typedef uint8_t flex_uint8_t;
typedef int16_t flex_int16_t;
typedef uint16_t flex_uint16_t;
typedef int32_t flex_int32_t;
typedef uint32_t flex_uint32_t;
}


%{
#include <string>
#include <cstdio>
#include <iostream>
#include <stack>
#include <vector>

#include "debug.h"
#include "error.h"
#include "flex_bison_common.h"

#include "parser.tab.h"

#include <Trinity/IO/Path.h>
 
using namespace std;
using namespace Trinity::IO;


    //Token capturing facilities{{{
//IssueID #27
#define CLIP_STRING(l,r)                            new string(yytext+l, yyleng-r-l)

string* retrieveVerbatimString()
{
    char       *currentPtr, *endPtr, *cpPtr;
    string     *str = new string();

    currentPtr	=	yytext+2;       //point currentPtr right after first "
    endPtr		=	yytext+yyleng-1;//point endPtr to last "
    cpPtr       =   currentPtr;     //head of the string to be appended

    for(;currentPtr != endPtr; ++currentPtr)
    {
        if(*currentPtr == '"')
        {
            str->append(cpPtr, currentPtr - cpPtr + 1);
            ++currentPtr;
            cpPtr = currentPtr + 1;
        }
    }
    str->append(cpPtr, currentPtr - cpPtr);
    return str;
}

long int retrieveInteger()
{
    return strtol(yytext, NULL, 10);
}

#ifdef LEX_TEST
#define LEX_DEBUG(t)                                wcout<< t <<endl
#define PRINT_TOKEN(t)								printf("TOKEN:\t%s\t;LINE:\t%d\tCOLUMN:\t%d\n",#t, yylineno, yycolumnno)
#define CAPTURE_STRING_ENCLOSED(t)                  PRINT_TOKEN(t); tmp_string = CLIP_STRING(1,1); wcout<<"\t"<<*tmp_string<<endl; delete tmp_string
#define CAPTURE_STRING(t)					        PRINT_TOKEN(t); tmp_string = CLIP_STRING(0,0); wcout<<"\t"<<*tmp_string<<endl; delete tmp_string
#define CAPTURE_STRING_VERBATIM(t)                  PRINT_TOKEN(t); tmp_string = retrieveVerbatimString(); wcout<<*tmp_string<<endl; delete tmp_string
#define CAPTURE_INTEGER(t)                          PRINT_TOKEN(t); printf("\t%d\n", retrieveInteger());
#define CAPTURE_TOKEN(t)                            PRINT_TOKEN(t);

static string									    *tmp_string;
#else
#define LEX_DEBUG(str)                              {}
#define CAPTURE_STRING_ENCLOSED(t)                  yylval.string = CLIP_STRING(1,1); return t
#define CAPTURE_STRING(t)					        yylval.string = CLIP_STRING(0,0); return t
#define CAPTURE_STRING_VERBATIM(t)                  yylval.string = retrieveVerbatimString(); return t
#define CAPTURE_INTEGER(t)                          yylval.integer= retrieveInteger(); return t
#define CAPTURE_TOKEN(t)                            yylval.token = t; return t
#endif

//}}}

static bool lex_nonstop = false;
void set_lex_nonstop_mode(bool nonstop)
{
    ::lex_nonstop = nonstop;
}

#define CAPTURE_COMMENT(comment_token) do                   \
{                                                           \
    if(::lex_nonstop)                                       \
        {CAPTURE_TOKEN(comment_token);}                     \
    else if (comment_token == T_COMMENT_BLOCK_UNCLOSED)     \
    {                                                       \
        error("Unterminated comment:");                     \
        yyterminate();                                      \
    }                                                       \
} while(0)

#define REPORT_UNTERMINATED_STRING do{                      \
    if(::lex_nonstop){                                      \
        CAPTURE_TOKEN(T_STRING_UNCLOSED);                   \
    }                                                       \
    else{                                                   \
        error("Unterminated string:"); yyterminate();       \
    }                                                       \
} while(0) 

/* multi file facilities */
static vector<string*>      filename_stack;
static vector<int>          lineno_stack, colno_stack;//flex & bison don't handle these. On our own.
static string              *current_filename = NULL; 
static vector<string*>      disposed_filenames; // Will be deleted at parser reset

static bool recursive_inclusion(string* filename)
{
    for(auto *str:filename_stack)
    if(*str == *filename) return true;

    if(current_filename != NULL && *filename == *current_filename)
        return true;

    return false;
}

static bool duplicate_inclusion(string* filename)
{
    for(auto *str:disposed_filenames)
    if(*str == *filename) return true;

    return false;
}

void push_new_yy_buffer_state(YY_BUFFER_STATE new_buffer, string* filename, FILE* fp)
{
    if(current_filename != NULL)
    {
        filename_stack.push_back(current_filename);
        lineno_stack.push_back(yylineno);
        colno_stack.push_back(yycolumnno);
    }

    current_filename = filename;
    yylineno = 1;
    yycolumnno = 1;
    if(*filename != "")
    {
        /* If filename == "", then yy_scan_string has already
           done pushing the buffer into the LEX internal stack.
           Otherwise, we just allocated a new FILE* based buffer
           and we have to do it manually here. */
        yypush_buffer_state (new_buffer);
        yyin = fp;//point yyin to new file pointer and we're ready to roll!
    }
}

void push_new_buffer(const char* buffer)
{
    string* filename = new string("");
    FILE* fp = NULL;

    auto text_buffer = yy_scan_string(buffer);
    push_new_yy_buffer_state(text_buffer, filename, fp);
}

// Remove UTF-8 byte order mark(BOM) if found
// The UTF-8 BOM byte sequence is 0xEF, 0xBB, 0xBF
// See https://en.wikipedia.org/wiki/Byte_order_mark#UTF-8
// If BOM is not found, the file is seeked to its beginning.
void strip_utf8_bom(FILE* fp)
{
    // !According to http://www.cplusplus.com/reference/cstdio/ungetc/
    // Multiple ungetc cause undefined behavior.
    // Some implementations allow characters to be retrieved in reverse order
    // done in multiple ungetc, but this is not portable behavior.

    if(getc(fp) != 0xEF)goto reset_file;
    if(getc(fp) != 0xBB)goto reset_file;
    if(getc(fp) != 0xBF)goto reset_file;

    return;

    reset_file:
    //BOM not found, reset the file.
    fseek(fp, 0, SEEK_SET);
}

int push_new_file(string *filename)
{
    /* The FILE* fp will be pushed onto FLEX's built-in stack, while
     * filename, lineno, columnno onto our own stacks. Note that, our
     * stacks are not aligned with the FLEX's built-in stack, as the top
     * target (current file) is in the FLEX's built-in stack, but not in
     * ours. We store the top target as spared parts, ranging from:
     *     string* current_filename
     *     int yylineno
     *     int yycolumnno
     */
    FILE *fp; int error_no;
    /* Normalize the filename */
    *filename = Path::GetFullPath(*filename);
    if(recursive_inclusion(filename))
    {
        error(string("Warning: Recursive inclusion: '")+*filename+"'");
        return 1;//TODO error number
    }
    if(duplicate_inclusion(filename))
    {
        error(string("Warning: Duplicated inclusion: '")+*filename+"'");
        return 2;//TODO error number
    }
    /* Convert UTF-8 filename to UTF-16 */
    auto utf16_filename_arr = Trinity::String(filename->c_str()).ToWcharArray();
    if((error_no = _wfopen_s(&fp, utf16_filename_arr, L"r")) != 0)
    {
        error(string("Could not open input file '") + *filename + "'.");
        return error_no;
    }

    strip_utf8_bom(fp);

    auto new_buffer = yy_create_buffer ( fp, YY_BUF_SIZE ); // this should not fail
    push_new_yy_buffer_state(new_buffer, filename, fp);

    LEX_DEBUG(*current_filename << " is on stack.");
    return 0;
}

extern "C" int yywrap() { 

    bool currentBufferIsString = (*current_filename == "");

    if(!currentBufferIsString)
        fclose(yyin);
    //old yyin will be popped out
    yypop_buffer_state();
    disposed_filenames.push_back(current_filename);
    if(!YY_CURRENT_BUFFER)
    {
        LEX_DEBUG("No more files to parse");
        current_filename = NULL;
        return -1;
    }else
    {
        current_filename = filename_stack.back();
        yylineno = lineno_stack.back();
        yycolumnno = colno_stack.back();

        filename_stack.pop_back();
        lineno_stack.pop_back();
        colno_stack.pop_back();
        LEX_DEBUG("Switching back to file:"<<*current_filename);
        return 0;
    }
}

void reset_filename_stack()
{
    if(current_filename)
        /* there's still something
           on the processing stack,
           we should dispose them first. */
           while(!yywrap())
                ;
    for(string* &strPtr: disposed_filenames)
    {
        LEX_DEBUG("deleting " << *strPtr);
        delete strPtr;
    }
    disposed_filenames.clear();
    yylineno = yycolumnno = 1;
}

/* position tracking facilities */
int yycolumnno = 1;      /* yylineno will be maintained automatically */
void position_tracker()  /*position_tracker will be called upon matching of a token */
{
    /* what do we have here:
     * int yycolumnno TO BE MAINTAINED
     * int yylineno, PROVIDED BY LEX, pointed at the line AFTER CURRENT SYMBOL
     *      * If the tail of the symbol is \r or \n, yylineno will point to the line after current symbol
     * yylloc provided by the parser TO BE MAINTAINED
     * string *current_filename
     * yytext, yyleng: starting pointer of the token buffer, and
     * length.
     */

    //c_ptr: current pointer; e_ptr: end of buffer; l_ptr: the last
    //character of current symbol.
    char* c_ptr = yytext, *e_ptr = yytext + yyleng, *l_ptr = yytext + yyleng - 1;
    int no_of_newline = 0;
    bool last_is_backslash_r = false;
    yylloc.first_line = yylloc.last_line = yylineno;
    yylloc.first_column = yylloc.last_column = yycolumnno;
    yylloc.filename = current_filename;

    /* When symbol tail is \n, after a contained \r, yycolumnno will be
     * reset to 1, and we lose track of the real last column.
     * In that case, we store the real last column in tmp_columnno when 
     * we hit a \r
     * We follow the convention that, for any \r or \n, or \r\n-terminated symbol, we
     * do NOT include the extra new line in the text range of the symbol.
     */
    int tmp_columnno;

    for(;c_ptr != e_ptr;++c_ptr)
    {
        if(c_ptr == l_ptr){
            if(*l_ptr == '\n')
            {
                if(last_is_backslash_r)
                {
                    yylloc.first_line = yylineno - no_of_newline;
                    yylloc.last_line -= 1;
                    yylloc.last_column = tmp_columnno;
                }else
                {
                    yylloc.first_line = yylineno - no_of_newline - 1;
                    yylloc.last_line -= 1;
                    yylloc.last_column = yycolumnno - 1;
                }
            }else if(*l_ptr == '\r')
            {
                yylloc.first_line = yylineno - no_of_newline;
                yylloc.last_column = yycolumnno - 1;
            }else
            {
                yylloc.first_line = yylineno - no_of_newline;
                //capture col before pushing the last char
                yylloc.last_column = yycolumnno;
            }
        }
        switch(*c_ptr)
        {
            case '\r':
                ++no_of_newline; 
                tmp_columnno = yycolumnno;
                yycolumnno = 1;
                last_is_backslash_r = true;
                break;
            case '\n':
                if(!last_is_backslash_r)//otherwise handled by \r
                {
                    ++no_of_newline;
                    yycolumnno = 1;
                }
                last_is_backslash_r = false;
                break;
            default:
                ++yycolumnno;
                last_is_backslash_r = false;
                break;
        }
    }
}
#define YY_USER_ACTION position_tracker();

//free string trigger
bool sticky_WAIT_STRING = false;

%}

%s WAIT_STRING
%s WAIT_STRING_PAREN

    /* line number */
%option yylineno


SPACES              [\ \n\t\r]*
    /* FSTR_TERM: anything but the forbidden symbols+spaces */
    /*
     * Allowing " and ' in the middle
     *   FSTR_MID            (?:(?:\\.)|[^;,\{\}\[\]\:\"\'])
     */
FSTR_MID            (?:\\.|[^;,\{\}\[\]\:\\])
FSTR_TERM           (?:\\.|[^;,\{\}\[\]\:\\\"\'\ \n\t\r])
    /* PSTR: String for Array<T>(dim_str), forbids '(' and ')' 
     */
PSTR_MID            (?:\\.|[^;,\{\}\(\)\[\]\:\\])
PSTR_TERM           (?:\\.|[^;,\{\}\(\)\[\]\:\\\"\'\ \n\t\r])
%%

    /* Comments */
\/\*(?:[^\*])*(?:(?:\*[^\/])(?:[^\*])*)*\*\/         CAPTURE_COMMENT(T_COMMENT_BLOCK);
\/\/.*                                               CAPTURE_COMMENT(T_COMMENT_LINE);
    /* for closed comment, the rule below matches
     * to the end of the comment, without */
    /* thus, it will be shorter than the closed
     * comment rule (and won't take effect). */
\/\*(?:[^\*])*(?:(?:\*[^\/])(?:[^\*])*)*             CAPTURE_COMMENT(T_COMMENT_BLOCK_UNCLOSED);

    /* Strings */
\"[^\"\\\r\n]*(?:\\.[^\"\\\r\n]*)*\"                 { CAPTURE_STRING_ENCLOSED(T_STRING); }
\'[^\'\\\r\n]*(?:\\.[^\'\\\r\n]*)*\'                 { CAPTURE_STRING_ENCLOSED(T_STRING); }
\@\"([^\"]|[\r\n]|(?:\"\"))*\"                       { CAPTURE_STRING_VERBATIM(T_STRING); }
<WAIT_STRING>(?:{FSTR_TERM}{FSTR_MID}*{FSTR_TERM})|{FSTR_TERM}  {
    CAPTURE_STRING(T_STRING);
}
<WAIT_STRING_PAREN>(?:{PSTR_TERM}{PSTR_MID}*{PSTR_TERM})|{PSTR_TERM}  {
    CAPTURE_STRING(T_STRING);
}

    /* Unclosed strings */
\"[^\"\\\r\n]*(?:\\.[^\"\\\r\n]*)*                   REPORT_UNTERMINATED_STRING;
\'[^\'\\\r\n]*(?:\\.[^\'\\\r\n]*)*                   REPORT_UNTERMINATED_STRING;
\@\"([^\"]|[\r\n]|(?:\"\"))*                         REPORT_UNTERMINATED_STRING;


    /* Values, overridden by WAIT_STRING */
[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}         {
    CAPTURE_STRING(T_GUIDVALUE);
}
[0-9]*                                               CAPTURE_INTEGER(T_INTEGER);

    /* Tricky keywords and related symbols */
[iI]nclude                                          {
    BEGIN(WAIT_STRING);
    CAPTURE_TOKEN(T_INCLUDE);
}
"TrinitySettings"                                   {
    sticky_WAIT_STRING = true;
    CAPTURE_TOKEN(T_TRINITY_SETTINGS);
}
"{"                                                 {
    if(sticky_WAIT_STRING)
    {
        LEX_DEBUG("LCurly: wait ON\n");
        BEGIN(WAIT_STRING);
    }
    CAPTURE_TOKEN(T_LCURLY);
}
"}"                                                 {
    if(sticky_WAIT_STRING)
    {
        LEX_DEBUG("RCurly: wait OFF\n");
        sticky_WAIT_STRING = false;
        BEGIN(INITIAL);
    }
    CAPTURE_TOKEN(T_RCURLY);
}
"["                                                 {
    LEX_DEBUG("LSquare: wait ON\n");
    sticky_WAIT_STRING = true;
    BEGIN(WAIT_STRING);
    CAPTURE_TOKEN(T_LSQUARE);
}
"]"                                                 {
    LEX_DEBUG("RSquare: wait OFF\n");
    sticky_WAIT_STRING = false;
    BEGIN(INITIAL);
    CAPTURE_TOKEN(T_RSQUARE);
}
"("                                                 {
    LEX_DEBUG("LParen: wait ON\n");
    BEGIN(WAIT_STRING_PAREN);
    CAPTURE_TOKEN(T_LPAREN);
}
")"                                                 {
    LEX_DEBUG("RParen: wait OFF\n");
    BEGIN(INITIAL);
    CAPTURE_TOKEN(T_RPAREN);
}
";"                                                 {
    if(!sticky_WAIT_STRING)
    {
        LEX_DEBUG("Semicolon: wait OFF");
        BEGIN(INITIAL);
    }
    CAPTURE_TOKEN(T_SEMICOLON);
}
","                                                 CAPTURE_TOKEN(T_COMMA);
":"                                                 CAPTURE_TOKEN(T_COLON);

    /* Non-tricky keywords and symbols*/

        /* Macro (Yes we have that!) */
"="                                                 CAPTURE_TOKEN(T_EQUAL);
"#"                                                 CAPTURE_TOKEN(T_SHARP);

        /* Structures */
"struct"                                            CAPTURE_TOKEN(T_STRUCT);
"Struct"                                            CAPTURE_TOKEN(T_STRUCT);
"cell"                                              CAPTURE_TOKEN(T_CELL);
"Cell"                                              CAPTURE_TOKEN(T_CELL);
"protocol"                                          CAPTURE_TOKEN(T_PROTOCOL);
"Protocol"                                          CAPTURE_TOKEN(T_PROTOCOL);
"server"                                            CAPTURE_TOKEN(T_SERVER);
"Server"                                            CAPTURE_TOKEN(T_SERVER);
"proxy"                                             CAPTURE_TOKEN(T_PROXY);
"Proxy"                                             CAPTURE_TOKEN(T_PROXY);
"module"                                            CAPTURE_TOKEN(T_MODULE);
"Module"                                            CAPTURE_TOKEN(T_MODULE);
"Enum"                                              CAPTURE_TOKEN(T_ENUM);
"enum"                                              CAPTURE_TOKEN(T_ENUM);

        /* Field modifiers */
"Optional"                                          CAPTURE_TOKEN(T_OPTIONALMODIFIER);
"optional"                                          CAPTURE_TOKEN(T_OPTIONALMODIFIER);

        /* Protocol related */
"type"                                              CAPTURE_TOKEN(T_TYPE);
"Type"                                              CAPTURE_TOKEN(T_TYPE);
"syn"                                               CAPTURE_TOKEN(T_SYNCRPC);
"Syn"                                               CAPTURE_TOKEN(T_SYNCRPC);
"asyn"                                              CAPTURE_TOKEN(T_ASYNCRPC);
"Asyn"                                              CAPTURE_TOKEN(T_ASYNCRPC);
"http"                                              CAPTURE_TOKEN(T_HTTP);
"Http"                                              CAPTURE_TOKEN(T_HTTP);
"HTTP"                                              CAPTURE_TOKEN(T_HTTP);
"Request"                                           CAPTURE_TOKEN(T_REQUEST);
"request"                                           CAPTURE_TOKEN(T_REQUEST);
"Response"                                          CAPTURE_TOKEN(T_RESPONSE);
"response"                                          CAPTURE_TOKEN(T_RESPONSE);
"stream"                                            CAPTURE_TOKEN(T_STREAM);
"Stream"                                            CAPTURE_TOKEN(T_STREAM);
"void"                                              CAPTURE_TOKEN(T_VOID);

        /* Built-in atom data types */
"byte"                                              CAPTURE_TOKEN(T_BYTETYPE);
"sbyte"                                             CAPTURE_TOKEN(T_SBYTETYPE);
"bool"                                              CAPTURE_TOKEN(T_BOOLTYPE);
"char"                                              CAPTURE_TOKEN(T_CHARTYPE);
"short"                                             CAPTURE_TOKEN(T_SHORTTYPE);
"ushort"                                            CAPTURE_TOKEN(T_USHORTTYPE);
"int"                                               CAPTURE_TOKEN(T_INTTYPE);
"uint"                                              CAPTURE_TOKEN(T_UINTTYPE);
"long"                                              CAPTURE_TOKEN(T_LONGTYPE);
"ulong"                                             CAPTURE_TOKEN(T_ULONGTYPE);
"float"                                             CAPTURE_TOKEN(T_FLOATTYPE);
"double"                                            CAPTURE_TOKEN(T_DOUBLETYPE);
"decimal"                                           CAPTURE_TOKEN(T_DECIMALTYPE);
"DateTime"                                          CAPTURE_TOKEN(T_DATETIMETYPE);
"Guid"                                              CAPTURE_TOKEN(T_GUIDTYPE);
"u8string"                                          CAPTURE_TOKEN(T_U8STRINGTYPE);
"String"                                            CAPTURE_TOKEN(T_STRINGTYPE);
"string"                                            CAPTURE_TOKEN(T_STRINGTYPE);

        /* Aliases for CellID type, will be converted to long(int64) */
"CellID"                                            CAPTURE_TOKEN(T_LONGTYPE);
"CellId"                                            CAPTURE_TOKEN(T_LONGTYPE);

        /* Aliases for atom data types */
"uint8"                                             CAPTURE_TOKEN(T_BYTETYPE);
"int8"                                              CAPTURE_TOKEN(T_SBYTETYPE);
"uint16"                                            CAPTURE_TOKEN(T_USHORTTYPE);
"int16"                                             CAPTURE_TOKEN(T_SHORTTYPE);
"uint32"                                            CAPTURE_TOKEN(T_UINTTYPE);
"int32"                                             CAPTURE_TOKEN(T_INTTYPE);
"uint64"                                            CAPTURE_TOKEN(T_ULONGTYPE);
"int64"                                             CAPTURE_TOKEN(T_LONGTYPE);

        /* Built-in container data types*/
"<"                                                 CAPTURE_TOKEN(T_LANGLE);
">"                                                 CAPTURE_TOKEN(T_RANGLE);
"List"                                              CAPTURE_TOKEN(T_LISTTYPE);
"Array"                                             CAPTURE_TOKEN(T_ARRAYTYPE);
"array"                                             CAPTURE_TOKEN(T_ARRAYTYPE);


    /* Identifiers. Overridden by keywords and free-style strings.*/
[a-zA-Z_][a-zA-Z_0-9]*                              CAPTURE_STRING(T_IDENTIFIER);
    /* Spaces, which will be overridden when matching free-style strings.*/
{SPACES}                                            ;
    /* Anything else triggers an error */
.												    {
    if(!::lex_nonstop)
    {
        error("Unrecognized input sequence:"); yyterminate();
    }
}

        /* Deprecated stuff {{{
        Non-supported containers
"Set"                                               CAPTURE_TOKEN(T_SETTYPE);
"Dictionary"                                        CAPTURE_TOKEN(T_DICTIONARYTYPE);

"index"                                             CAPTURE_TOKEN(T_INDEX);
"Index"                                             CAPTURE_TOKEN(T_INDEX);

        Non-supported modifiers
"invisible"                                         CAPTURE_TOKEN(T_INVISIBLEMODIFIER);
"Invisible"                                         CAPTURE_TOKEN(T_INVISIBLEMODIFIER);
"Extern"                                            CAPTURE_TOKEN(T_EXTERNMODIFIER);
"extern"                                            CAPTURE_TOKEN(T_EXTERNMODIFIER);

         * "Fixed"                                             CAPTURE_TOKEN(T_FIXEDMODIFIER);
         * "fixed"                                             CAPTURE_TOKEN(T_FIXEDMODIFIER);
         * "Elastic"                                           CAPTURE_TOKEN(T_ELASTICMODIFIER);
         * "elastic"                                           CAPTURE_TOKEN(T_ELASTICMODIFIER);
         * "\n"                                                CAPTURE_TOKEN(T_LF);
         * "<-"                                                CAPTURE_TOKEN(T_MAPSYMBOL);
         * "Using"                                             CAPTURE_TOKEN(T_USING);
         * "using"                                             CAPTURE_TOKEN(T_USING);
         * "EntityList"                                        CAPTURE_TOKEN(T_ENTITYLIST);
         * "DataSource"                                        CAPTURE_TOKEN(T_DATASOURCE);
         * "RelationalTable"                                   CAPTURE_TOKEN(T_RELATIONALTABLE);
         * "PartitionBy"                                       CAPTURE_TOKEN(T_PARTITIONBY);
         * "SpecFile"                                          CAPTURE_TOKEN(T_SPECFILE);
         * "Entities"                                          CAPTURE_TOKEN(T_ENTITIES);
         * "ConnectionString"                                  CAPTURE_TOKEN(T_CONNECTIONSTRING);
         * "SqlServer"                                         CAPTURE_TOKEN(T_SQLSERVER);
         * "SQLServer"                                         CAPTURE_TOKEN(T_SQLSERVER);
         * "PrimaryKey"                                        CAPTURE_TOKEN(T_PRIMARYKEY);
         * "ReferencedCell"                                    CAPTURE_TOKEN(T_REFERENCEDCELL);
         * "Column"                                            CAPTURE_TOKEN(T_COLUMN);
         * "association"                                       CAPTURE_TOKEN(T_ASSOCIATION);
         * "Association"                                       CAPTURE_TOKEN(T_ASSOCIATION);
         * "RunningMode"                                       CAPTURE_TOKEN(T_RUNNINGMODE);
         * "Embedded"                                          CAPTURE_TOKEN(T_EMBEDDED);
         * "Distributed"                                       CAPTURE_TOKEN(T_DISTRIBUTED);
         * "IndexServerConnectionString"                       CAPTURE_TOKEN(T_INDEXCONNSTRING);
         * "IndexConnString"                                   CAPTURE_TOKEN(T_INDEXCONNSTRING);
         * "TQL"                                               CAPTURE_TOKEN(T_TQL);
         * "OFF"                                               CAPTURE_TOKEN(T_OFF);
         * "Off"                                               CAPTURE_TOKEN(T_OFF);
         * "ON"                                                CAPTURE_TOKEN(T_ON);
         * "On"                                                CAPTURE_TOKEN(T_ON);
         * "RDF"                                               CAPTURE_TOKEN(T_RDF);
         * "Freebase"                                          CAPTURE_TOKEN(T_FREEBASE);
         * "TSLProfile"                                        CAPTURE_TOKEN(T_TSLPROFILE);
         * "TrinityMM"                                         CAPTURE_TOKEN(T_TRINITYMM);
         * "ExtensionSuffixChar"                               CAPTURE_TOKEN(T_EXTENSIONSUFFIXCHAR);
         * "namespace"                                         CAPTURE_TOKEN(T_NAMESPACE);
         * 
         * Layout could also be dealt as a key-value pair.
         * 
         * "Layout"                                            CAPTURE_TOKEN(T_LAYOUT);
         * "Sequential"                                        CAPTURE_TOKEN(T_SEQ_LAYOUT);
         * "Auto"                                              CAPTURE_TOKEN(T_AUTO_LAYOUT);
         * 
         * Don't know what is dot for.
         * 
         * "."                                                 CAPTURE_TOKEN(T_DOT); 
         * 
         }}}*/

%%

#ifdef LEX_TEST
int main()
{
    FILE *fp;
    if(0 != fopen_s(&fp, "test.tsl","r"))
    {
        printf("Could not open the test file.\n");
        return -1;
    }
    yyin = fp;
    yylex();
    return 0;
}
#endif
