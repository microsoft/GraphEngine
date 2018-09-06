#include "Trinity.TSL.Parser.h"
#include "SyntaxNode.h"
#include "error.h"
#include "debug.h"
#include "flex_bison_common.h"
#include "Trinity/String.h"
using namespace std;
using Trinity::String;

#if YYDEBUG
extern int yydebug;
#endif

void parser_disable_warnings() { error_disable_warnings = true; }

void reset_parser()
{
    delete tsl;
    tsl = NULL;
    reset_filename_stack();
}

NTSL* start_parser(const std::vector<String>& filelist)
{
    reset_parser();
    set_lex_nonstop_mode(false);
    for (auto &f : filelist)
    {
        if (push_new_file(new std::string(f)) != 0)
            return NULL;
    }
#if YYDEBUG
    yydebug = 1;
#endif
    //reset error count
    error_count = 0;
    int parser_result = yyparse();
    NTSL* ret = tsl;
    if (tsl != NULL)
    {
        tsl->semantic_check();
    }

    if (error_count != 0)
    {
        ret = NULL;
        reset_parser();
    }
    return ret;
}

TokenList::TokenList(const char* buffer)
{
    reset_parser();
    set_lex_nonstop_mode(true);
    push_new_buffer(buffer);
    yytokentype token;
    while (0 < (token = (yytokentype)yylex()))
    {
        tokens.push_back({
            token,
            yylloc
        });
    }
    reset_parser();
}