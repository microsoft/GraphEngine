/* A Bison parser, made by GNU Bison 2.7.  */

/* Bison interface for Yacc-like parsers in C
   
      Copyright (C) 1984, 1989-1990, 2000-2012 Free Software Foundation, Inc.
   
   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.
   
   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
   
   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.  */

/* As a special exception, you may create a larger work that contains
   part or all of the Bison parser skeleton and distribute that work
   under terms of your choice, so long as that work isn't itself a
   parser generator using the skeleton or a modified version thereof
   as a parser skeleton.  Alternatively, if you modify or redistribute
   the parser skeleton itself, you may (at your option) remove this
   special exception, which will cause the skeleton and the resulting
   Bison output files to be licensed under the GNU General Public
   License without this special exception.
   
   This special exception was added by the Free Software Foundation in
   version 2.2 of Bison.  */

#ifndef YY_YY_PARSER_TAB_H_INCLUDED
# define YY_YY_PARSER_TAB_H_INCLUDED
/* Enabling traces.  */
#ifndef YYDEBUG
# define YYDEBUG 0
#endif
#if YYDEBUG
extern int yydebug;
#endif
/* "%code requires" blocks.  */
/* Line 2058 of yacc.c  */
#line 5 "parser.y"

    #include "SyntaxNode.h"
    #include "error.h"
    #include "flex_bison_common.h"
    #include <string>
    using namespace std;

    #pragma warning(disable:4065) // switch statement contains 'default' but no 'case' labels


/* Line 2058 of yacc.c  */
#line 57 "parser.tab.h"

/* Tokens.  */
#ifndef YYTOKENTYPE
# define YYTOKENTYPE
   /* Put the tokens into the symbol table, so that GDB and other debuggers
      know about them.  */
   enum yytokentype {
     T_INCLUDE = 258,
     T_TRINITY_SETTINGS = 259,
     T_STRUCT = 260,
     T_CELL = 261,
     T_PROTOCOL = 262,
     T_SERVER = 263,
     T_PROXY = 264,
     T_MODULE = 265,
     T_ENUM = 266,
     T_BYTETYPE = 267,
     T_SBYTETYPE = 268,
     T_BOOLTYPE = 269,
     T_CHARTYPE = 270,
     T_SHORTTYPE = 271,
     T_USHORTTYPE = 272,
     T_INTTYPE = 273,
     T_UINTTYPE = 274,
     T_LONGTYPE = 275,
     T_ULONGTYPE = 276,
     T_FLOATTYPE = 277,
     T_DOUBLETYPE = 278,
     T_DECIMALTYPE = 279,
     T_DATETIMETYPE = 280,
     T_GUIDTYPE = 281,
     T_U8STRINGTYPE = 282,
     T_STRINGTYPE = 283,
     T_LISTTYPE = 284,
     T_ARRAYTYPE = 285,
     T_LCURLY = 286,
     T_RCURLY = 287,
     T_LSQUARE = 288,
     T_RSQUARE = 289,
     T_SEMICOLON = 290,
     T_COMMA = 291,
     T_COLON = 292,
     T_EQUAL = 293,
     T_SHARP = 294,
     T_LANGLE = 295,
     T_RANGLE = 296,
     T_LPAREN = 297,
     T_RPAREN = 298,
     T_OPTIONALMODIFIER = 299,
     T_TYPE = 300,
     T_SYNCRPC = 301,
     T_ASYNCRPC = 302,
     T_HTTP = 303,
     T_REQUEST = 304,
     T_RESPONSE = 305,
     T_STREAM = 306,
     T_VOID = 307,
     T_INTEGER = 308,
     T_STRING = 309,
     T_STRING_UNCLOSED = 310,
     T_GUIDVALUE = 311,
     T_IDENTIFIER = 312,
     T_COMMENT_LINE = 313,
     T_COMMENT_BLOCK = 314,
     T_COMMENT_BLOCK_UNCLOSED = 315
   };
#endif


#if ! defined YYSTYPE && ! defined YYSTYPE_IS_DECLARED
typedef union YYSTYPE
{
/* Line 2058 of yacc.c  */
#line 41 "parser.y"

    /* Terminal types */
    int                     token;
    int                     integer;
    std::string             *string;

    /* Key value types */
    vector<std::string*>        *stringList;
    NKVPair                     *kvpair;
    vector<NKVPair*>            *kvlist;

    /* Field types */
    NFieldType                  *fieldType;
    NField                      *field;
    vector<NField*>             *fieldList;
    vector<int>                 *modifierList;

    /* Protocol types */
    NProtocolProperty           *protocolProperty;
    NProtocolReference          *protocolReference;
    vector<NProtocolProperty*>  *proto_prop_list;
    vector<NProtocolReference*> *proto_ref_list;

    /* Enum types */
    NEnumEntry                  *enumEntry;
    vector<NEnumEntry*>         *enumEntryList;

    /* Top-level elements */
    NStruct                 *structDescriptor;
    NCell                   *cellDescriptor;
    NTrinitySettings        *settingsDescriptor;
    NProtocol               *protocolDescriptor;
    NProxy                  *proxyDescriptor;
    NServer                 *serverDescriptor;
    NModule                 *moduleDescriptor;
    NEnum                   *enumDescriptor;

    /* Root object */
    NTSL       *tsl;


/* Line 2058 of yacc.c  */
#line 174 "parser.tab.h"
} YYSTYPE;
# define YYSTYPE_IS_TRIVIAL 1
# define yystype YYSTYPE /* obsolescent; will be withdrawn */
# define YYSTYPE_IS_DECLARED 1
#endif

#if ! defined YYLTYPE && ! defined YYLTYPE_IS_DECLARED
typedef struct YYLTYPE
{
  int first_line;
  int first_column;
  int last_line;
  int last_column;
} YYLTYPE;
# define yyltype YYLTYPE /* obsolescent; will be withdrawn */
# define YYLTYPE_IS_DECLARED 1
# define YYLTYPE_IS_TRIVIAL 1
#endif

extern YYSTYPE yylval;
extern YYLTYPE yylloc;
#ifdef YYPARSE_PARAM
#if defined __STDC__ || defined __cplusplus
int yyparse (void *YYPARSE_PARAM);
#else
int yyparse ();
#endif
#else /* ! YYPARSE_PARAM */
#if defined __STDC__ || defined __cplusplus
int yyparse (void);
#else
int yyparse ();
#endif
#endif /* ! YYPARSE_PARAM */

#endif /* !YY_YY_PARSER_TAB_H_INCLUDED  */
