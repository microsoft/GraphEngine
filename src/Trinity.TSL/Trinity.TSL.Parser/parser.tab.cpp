/* A Bison parser, made by GNU Bison 2.7.  */

/* Bison implementation for Yacc-like parsers in C
   
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

/* C LALR(1) parser skeleton written by Richard Stallman, by
   simplifying the original so-called "semantic" parser.  */

/* All symbols defined below should begin with yy or YY, to avoid
   infringing on user name space.  This should be done even for local
   variables, as they might otherwise be expanded by user macros.
   There are some unavoidable exceptions within include files to
   define necessary library symbols; they are noted "INFRINGES ON
   USER NAME SPACE" below.  */

/* Identify Bison output.  */
#define YYBISON 1

/* Bison version.  */
#define YYBISON_VERSION "2.7"

/* Skeleton name.  */
#define YYSKELETON_NAME "yacc.c"

/* Pure parsers.  */
#define YYPURE 0

/* Push parsers.  */
#define YYPUSH 0

/* Pull parsers.  */
#define YYPULL 1




/* Copy the first part of user declarations.  */
/* Line 371 of yacc.c  */
#line 1 "parser.y"

    #include "debug.h"

/* Line 371 of yacc.c  */
#line 72 "parser.tab.cpp"

# ifndef YY_NULL
#  if defined __cplusplus && 201103L <= __cplusplus
#   define YY_NULL nullptr
#  else
#   define YY_NULL 0
#  endif
# endif

/* Enabling verbose error messages.  */
#ifdef YYERROR_VERBOSE
# undef YYERROR_VERBOSE
# define YYERROR_VERBOSE 1
#else
# define YYERROR_VERBOSE 0
#endif

/* In a future release of Bison, this section will be replaced
   by #include "parser.tab.h".  */
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
/* Line 387 of yacc.c  */
#line 5 "parser.y"

    #include "SyntaxNode.h"
    #include "error.h"
    #include "flex_bison_common.h"
    #include <string>
    using namespace std;

    #pragma warning(disable:4065) // switch statement contains 'default' but no 'case' labels


/* Line 387 of yacc.c  */
#line 115 "parser.tab.cpp"

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
/* Line 387 of yacc.c  */
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


/* Line 387 of yacc.c  */
#line 232 "parser.tab.cpp"
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

/* Copy the second part of user declarations.  */

/* Line 390 of yacc.c  */
#line 273 "parser.tab.cpp"
/* Unqualified %code blocks.  */
/* Line 391 of yacc.c  */
#line 15 "parser.y"

    NTSL        *tsl = NULL;
    YYLTYPE     lhs_loc;//To be captured in YYLLOC_DEFAULT
    #ifdef YYPRINT
    void yy_print_trap(FILE*, unsigned short, YYSTYPE);
    #endif

//	Deprecated stuff
//		index: T_INDEX T_IDENTIFIER T_LCURLY T_RCURLY
//  		                    { $$ = new NIndex(); $$->name = $2;/* TODO */}
//  		;

//  		%type   <indexDescriptor>   index
//  		|index              { tsl = new NTSL(); tsl->indexList->push_back($1);$$=tsl; }
//  		    NIndex                  *indexDescriptor;
//  		|tsl index          { $1->indexList->push_back($2); }

//  		 T_FIXEDMODIFIER    { $$ = $1; }
//  		|T_ELASTICMODIFIER  { $$ = $1; }
// T_EXTERNMODIFIER T_INVISIBLEMODIFIER
// T_EXTERNMODIFIER   { $$ = $1; }
//|T_INVISIBLEMODIFIER{ $$ = $1; }
    



/* Line 391 of yacc.c  */
#line 304 "parser.tab.cpp"

#ifdef short
# undef short
#endif

#ifdef YYTYPE_UINT8
typedef YYTYPE_UINT8 yytype_uint8;
#else
typedef unsigned char yytype_uint8;
#endif

#ifdef YYTYPE_INT8
typedef YYTYPE_INT8 yytype_int8;
#elif (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
typedef signed char yytype_int8;
#else
typedef short int yytype_int8;
#endif

#ifdef YYTYPE_UINT16
typedef YYTYPE_UINT16 yytype_uint16;
#else
typedef unsigned short int yytype_uint16;
#endif

#ifdef YYTYPE_INT16
typedef YYTYPE_INT16 yytype_int16;
#else
typedef short int yytype_int16;
#endif

#ifndef YYSIZE_T
# ifdef __SIZE_TYPE__
#  define YYSIZE_T __SIZE_TYPE__
# elif defined size_t
#  define YYSIZE_T size_t
# elif ! defined YYSIZE_T && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#  include <stddef.h> /* INFRINGES ON USER NAME SPACE */
#  define YYSIZE_T size_t
# else
#  define YYSIZE_T unsigned int
# endif
#endif

#define YYSIZE_MAXIMUM ((YYSIZE_T) -1)

#ifndef YY_
# if defined YYENABLE_NLS && YYENABLE_NLS
#  if ENABLE_NLS
#   include <libintl.h> /* INFRINGES ON USER NAME SPACE */
#   define YY_(Msgid) dgettext ("bison-runtime", Msgid)
#  endif
# endif
# ifndef YY_
#  define YY_(Msgid) Msgid
# endif
#endif

/* Suppress unused-variable warnings by "using" E.  */
#if ! defined lint || defined __GNUC__
# define YYUSE(E) ((void) (E))
#else
# define YYUSE(E) /* empty */
#endif

/* Identity function, used to suppress warnings about constant conditions.  */
#ifndef lint
# define YYID(N) (N)
#else
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static int
YYID (int yyi)
#else
static int
YYID (yyi)
    int yyi;
#endif
{
  return yyi;
}
#endif

#if ! defined yyoverflow || YYERROR_VERBOSE

/* The parser invokes alloca or malloc; define the necessary symbols.  */

# ifdef YYSTACK_USE_ALLOCA
#  if YYSTACK_USE_ALLOCA
#   ifdef __GNUC__
#    define YYSTACK_ALLOC __builtin_alloca
#   elif defined __BUILTIN_VA_ARG_INCR
#    include <alloca.h> /* INFRINGES ON USER NAME SPACE */
#   elif defined _AIX
#    define YYSTACK_ALLOC __alloca
#   elif defined _MSC_VER
#    include <malloc.h> /* INFRINGES ON USER NAME SPACE */
#    define alloca _alloca
#   else
#    define YYSTACK_ALLOC alloca
#    if ! defined _ALLOCA_H && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#     include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
      /* Use EXIT_SUCCESS as a witness for stdlib.h.  */
#     ifndef EXIT_SUCCESS
#      define EXIT_SUCCESS 0
#     endif
#    endif
#   endif
#  endif
# endif

# ifdef YYSTACK_ALLOC
   /* Pacify GCC's `empty if-body' warning.  */
#  define YYSTACK_FREE(Ptr) do { /* empty */; } while (YYID (0))
#  ifndef YYSTACK_ALLOC_MAXIMUM
    /* The OS might guarantee only one guard page at the bottom of the stack,
       and a page size can be as small as 4096 bytes.  So we cannot safely
       invoke alloca (N) if N exceeds 4096.  Use a slightly smaller number
       to allow for a few compiler-allocated temporary stack slots.  */
#   define YYSTACK_ALLOC_MAXIMUM 4032 /* reasonable circa 2006 */
#  endif
# else
#  define YYSTACK_ALLOC YYMALLOC
#  define YYSTACK_FREE YYFREE
#  ifndef YYSTACK_ALLOC_MAXIMUM
#   define YYSTACK_ALLOC_MAXIMUM YYSIZE_MAXIMUM
#  endif
#  if (defined __cplusplus && ! defined EXIT_SUCCESS \
       && ! ((defined YYMALLOC || defined malloc) \
	     && (defined YYFREE || defined free)))
#   include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#   ifndef EXIT_SUCCESS
#    define EXIT_SUCCESS 0
#   endif
#  endif
#  ifndef YYMALLOC
#   define YYMALLOC malloc
#   if ! defined malloc && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void *malloc (YYSIZE_T); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
#  ifndef YYFREE
#   define YYFREE free
#   if ! defined free && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void free (void *); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
# endif
#endif /* ! defined yyoverflow || YYERROR_VERBOSE */


#if (! defined yyoverflow \
     && (! defined __cplusplus \
	 || (defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL \
	     && defined YYSTYPE_IS_TRIVIAL && YYSTYPE_IS_TRIVIAL)))

/* A type that is properly aligned for any stack member.  */
union yyalloc
{
  yytype_int16 yyss_alloc;
  YYSTYPE yyvs_alloc;
  YYLTYPE yyls_alloc;
};

/* The size of the maximum gap between one aligned stack and the next.  */
# define YYSTACK_GAP_MAXIMUM (sizeof (union yyalloc) - 1)

/* The size of an array large to enough to hold all stacks, each with
   N elements.  */
# define YYSTACK_BYTES(N) \
     ((N) * (sizeof (yytype_int16) + sizeof (YYSTYPE) + sizeof (YYLTYPE)) \
      + 2 * YYSTACK_GAP_MAXIMUM)

# define YYCOPY_NEEDED 1

/* Relocate STACK from its old location to the new one.  The
   local variables YYSIZE and YYSTACKSIZE give the old and new number of
   elements in the stack, and YYPTR gives the new location of the
   stack.  Advance YYPTR to a properly aligned location for the next
   stack.  */
# define YYSTACK_RELOCATE(Stack_alloc, Stack)				\
    do									\
      {									\
	YYSIZE_T yynewbytes;						\
	YYCOPY (&yyptr->Stack_alloc, Stack, yysize);			\
	Stack = &yyptr->Stack_alloc;					\
	yynewbytes = yystacksize * sizeof (*Stack) + YYSTACK_GAP_MAXIMUM; \
	yyptr += yynewbytes / sizeof (*yyptr);				\
      }									\
    while (YYID (0))

#endif

#if defined YYCOPY_NEEDED && YYCOPY_NEEDED
/* Copy COUNT objects from SRC to DST.  The source and destination do
   not overlap.  */
# ifndef YYCOPY
#  if defined __GNUC__ && 1 < __GNUC__
#   define YYCOPY(Dst, Src, Count) \
      __builtin_memcpy (Dst, Src, (Count) * sizeof (*(Src)))
#  else
#   define YYCOPY(Dst, Src, Count)              \
      do                                        \
        {                                       \
          YYSIZE_T yyi;                         \
          for (yyi = 0; yyi < (Count); yyi++)   \
            (Dst)[yyi] = (Src)[yyi];            \
        }                                       \
      while (YYID (0))
#  endif
# endif
#endif /* !YYCOPY_NEEDED */

/* YYFINAL -- State number of the termination state.  */
#define YYFINAL  2
/* YYLAST -- Last index in YYTABLE.  */
#define YYLAST   308

/* YYNTOKENS -- Number of terminals.  */
#define YYNTOKENS  61
/* YYNNTS -- Number of nonterminals.  */
#define YYNNTS  61
/* YYNRULES -- Number of rules.  */
#define YYNRULES  145
/* YYNRULES -- Number of states.  */
#define YYNSTATES  237

/* YYTRANSLATE(YYLEX) -- Bison symbol number corresponding to YYLEX.  */
#define YYUNDEFTOK  2
#define YYMAXUTOK   315

#define YYTRANSLATE(YYX)						\
  ((unsigned int) (YYX) <= YYMAXUTOK ? yytranslate[YYX] : YYUNDEFTOK)

/* YYTRANSLATE[YYLEX] -- Bison symbol number corresponding to YYLEX.  */
static const yytype_uint8 yytranslate[] =
{
       0,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     1,     2,     3,     4,
       5,     6,     7,     8,     9,    10,    11,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    31,    32,    33,    34,
      35,    36,    37,    38,    39,    40,    41,    42,    43,    44,
      45,    46,    47,    48,    49,    50,    51,    52,    53,    54,
      55,    56,    57,    58,    59,    60
};

#if YYDEBUG
/* YYPRHS[YYN] -- Index of the first RHS symbol of rule number YYN in
   YYRHS.  */
static const yytype_uint16 yyprhs[] =
{
       0,     0,     3,     4,     7,    10,    13,    16,    19,    22,
      25,    28,    31,    34,    38,    45,    52,    60,    66,    71,
      77,    83,    89,    95,   101,   108,   109,   112,   115,   116,
     119,   122,   126,   127,   131,   134,   135,   138,   141,   145,
     146,   149,   152,   158,   160,   164,   168,   172,   174,   179,
     184,   189,   194,   199,   204,   209,   214,   219,   221,   222,
     225,   227,   229,   234,   242,   247,   249,   251,   253,   255,
     257,   259,   261,   263,   265,   267,   269,   271,   273,   275,
     277,   279,   281,   283,   287,   288,   291,   294,   299,   302,
     306,   308,   311,   314,   319,   321,   323,   325,   327,   329,
     331,   333,   335,   339,   344,   348,   351,   354,   357,   360,
     363,   365,   367,   369,   371,   376,   381,   386,   388,   390,
     392,   395,   397,   400,   402,   404,   406,   408,   410,   412,
     416,   420,   424,   428,   431,   434,   441,   448,   450,   452,
     454,   456,   458,   460,   463,   468
};

/* YYRHS -- A `-1'-separated list of the rules' RHS.  */
static const yytype_int8 yyrhs[] =
{
      62,     0,    -1,    -1,    62,    64,    -1,    62,    65,    -1,
      62,    66,    -1,    62,    67,    -1,    62,    68,    -1,    62,
      69,    -1,    62,    70,    -1,    62,    71,    -1,    62,    63,
      -1,    62,    94,    -1,     3,    54,    35,    -1,    73,     5,
      57,    31,    72,    32,    -1,    73,     6,    57,    31,    72,
      32,    -1,    73,     6,     5,    57,    31,    72,    32,    -1,
       4,    57,    31,    75,    32,    -1,     4,    31,    75,    32,
      -1,     7,    57,    31,    76,    32,    -1,     9,    57,    31,
      78,    32,    -1,     8,    57,    31,    78,    32,    -1,    10,
      57,    31,    78,    32,    -1,    11,    57,    31,    88,    32,
      -1,    11,    57,    31,    88,    90,    32,    -1,    -1,    72,
      79,    -1,    72,   103,    -1,    -1,    73,    74,    -1,    33,
      34,    -1,    33,    80,    34,    -1,    -1,    75,    81,    35,
      -1,    75,   111,    -1,    -1,    76,    82,    -1,    76,   112,
      -1,     7,    57,    35,    -1,    -1,    78,    77,    -1,    78,
     107,    -1,    73,    84,    85,    57,    35,    -1,    81,    -1,
      80,    36,    81,    -1,    81,    37,    54,    -1,    54,    37,
      54,    -1,    54,    -1,    45,    37,    46,    35,    -1,    45,
      37,    47,    35,    -1,    45,    37,    48,    35,    -1,    49,
      37,    57,    35,    -1,    49,    37,    51,    35,    -1,    49,
      37,    52,    35,    -1,    50,    37,    57,    35,    -1,    50,
      37,    51,    35,    -1,    50,    37,    52,    35,    -1,    44,
      -1,    -1,    84,    83,    -1,    86,    -1,    57,    -1,    85,
      33,    87,    34,    -1,    30,    40,    85,    41,    42,    87,
      43,    -1,    29,    40,    85,    41,    -1,    12,    -1,    13,
      -1,    14,    -1,    15,    -1,    16,    -1,    17,    -1,    18,
      -1,    19,    -1,    20,    -1,    21,    -1,    22,    -1,    23,
      -1,    24,    -1,    25,    -1,    26,    -1,    28,    -1,    27,
      -1,    54,    -1,    87,    36,    54,    -1,    -1,    88,    89,
      -1,    88,   118,    -1,    57,    38,    53,    36,    -1,    57,
      36,    -1,    57,    38,    53,    -1,    57,    -1,     1,    32,
      -1,     1,    35,    -1,    57,    57,     1,    35,    -1,    95,
      -1,    96,    -1,    97,    -1,    98,    -1,    99,    -1,   100,
      -1,   101,    -1,   102,    -1,    73,     5,    91,    -1,    73,
       6,     5,    91,    -1,    73,     6,    91,    -1,     7,    91,
      -1,     8,    91,    -1,    10,    91,    -1,     9,    91,    -1,
      11,    91,    -1,   104,    -1,   105,    -1,   106,    -1,    92,
      -1,    73,    84,    57,    92,    -1,    73,    84,    85,    92,
      -1,    73,    84,    85,    93,    -1,   108,    -1,   109,    -1,
     110,    -1,     7,    92,    -1,    92,    -1,     7,    93,    -1,
      92,    -1,   113,    -1,   114,    -1,   115,    -1,   116,    -1,
      92,    -1,     1,    37,    46,    -1,     1,    37,    47,    -1,
       1,    37,    48,    -1,    45,    37,    92,    -1,    49,    92,
      -1,    50,    92,    -1,    49,    37,   117,   117,     1,    35,
      -1,    50,    37,   117,   117,     1,    35,    -1,    52,    -1,
      51,    -1,    57,    -1,   119,    -1,   120,    -1,   121,    -1,
       1,    36,    -1,    57,    38,     1,    36,    -1,    57,     1,
      36,    -1
};

/* YYRLINE[YYN] -- source line where rule number YYN was defined.  */
static const yytype_uint16 yyrline[] =
{
       0,   188,   188,   189,   190,   191,   192,   193,   194,   195,
     196,   199,   200,   206,   219,   224,   227,   232,   234,   238,
     242,   246,   250,   255,   257,   263,   264,   266,   270,   271,
     276,   278,   283,   284,   286,   290,   291,   293,   296,   300,
     301,   303,   307,   317,   318,   323,   325,   327,   332,   334,
     336,   338,   340,   342,   344,   346,   348,   353,   357,   358,
     363,   364,   365,   371,   377,   384,   385,   386,   387,   388,
     389,   390,   391,   392,   393,   394,   395,   396,   397,   398,
     399,   400,   404,   406,   411,   412,   414,   418,   421,   426,
     429,   436,   437,   438,   441,   442,   443,   444,   445,   446,
     447,   448,   451,   452,   453,   454,   455,   456,   457,   458,
     461,   462,   463,   464,   468,   469,   470,   473,   474,   476,
     479,   480,   481,   484,   488,   489,   490,   491,   492,   495,
     496,   497,   498,   499,   499,   500,   502,   507,   508,   509,
     514,   515,   516,   518,   519,   520
};
#endif

#if YYDEBUG || YYERROR_VERBOSE || 0
/* YYTNAME[SYMBOL-NUM] -- String name of the symbol SYMBOL-NUM.
   First, the terminals, then, starting at YYNTOKENS, nonterminals.  */
static const char *const yytname[] =
{
  "$end", "error", "$undefined", "T_INCLUDE", "T_TRINITY_SETTINGS",
  "T_STRUCT", "T_CELL", "T_PROTOCOL", "T_SERVER", "T_PROXY", "T_MODULE",
  "T_ENUM", "T_BYTETYPE", "T_SBYTETYPE", "T_BOOLTYPE", "T_CHARTYPE",
  "T_SHORTTYPE", "T_USHORTTYPE", "T_INTTYPE", "T_UINTTYPE", "T_LONGTYPE",
  "T_ULONGTYPE", "T_FLOATTYPE", "T_DOUBLETYPE", "T_DECIMALTYPE",
  "T_DATETIMETYPE", "T_GUIDTYPE", "T_U8STRINGTYPE", "T_STRINGTYPE",
  "T_LISTTYPE", "T_ARRAYTYPE", "T_LCURLY", "T_RCURLY", "T_LSQUARE",
  "T_RSQUARE", "T_SEMICOLON", "T_COMMA", "T_COLON", "T_EQUAL", "T_SHARP",
  "T_LANGLE", "T_RANGLE", "T_LPAREN", "T_RPAREN", "T_OPTIONALMODIFIER",
  "T_TYPE", "T_SYNCRPC", "T_ASYNCRPC", "T_HTTP", "T_REQUEST", "T_RESPONSE",
  "T_STREAM", "T_VOID", "T_INTEGER", "T_STRING", "T_STRING_UNCLOSED",
  "T_GUIDVALUE", "T_IDENTIFIER", "T_COMMENT_LINE", "T_COMMENT_BLOCK",
  "T_COMMENT_BLOCK_UNCLOSED", "$accept", "tsl", "include", "struct",
  "cell", "settings", "protocol", "proxy", "server", "module", "enum",
  "field_list", "attributes", "attributes_group", "settings_kvlist",
  "protocol_property_list", "protocol_reference", "protocol_ref_list",
  "field", "attributes_kvlist", "kvpair", "protocol_property", "modifier",
  "modifier_list", "field_type", "atom_type", "array_dimension_list",
  "enum_entry_list", "enum_entry", "enum_last_entry", "error_block_end",
  "error_stmt_end", "error_toomany_name", "error_top_tier",
  "error_struct_no_name", "error_cell_no_name", "error_cell_expect_struct",
  "error_protocol_no_name", "error_server_no_name", "error_module_no_name",
  "error_proxy_no_name", "error_enum_no_name", "error_field_list",
  "error_fl_no_field_type", "error_fl_no_name", "error_fl_toomany_name",
  "error_protocol_ref_list", "error_prl_no_name",
  "error_prl_no_protocol_keyword", "error_prl_toomany_name",
  "error_settings_list", "error_protocol_property_list",
  "error_ppl_type_kw", "error_ppl_type_specifier", "error_ppl_msg",
  "error_ppl_toomany_name", "error_protocol_message_type_specifier",
  "error_enum_entry_list", "error_eel_no_name", "error_eel_invalid_value",
  "error_eel_expect_value", YY_NULL
};
#endif

# ifdef YYPRINT
/* YYTOKNUM[YYLEX-NUM] -- Internal token number corresponding to
   token YYLEX-NUM.  */
static const yytype_uint16 yytoknum[] =
{
       0,   256,   257,   258,   259,   260,   261,   262,   263,   264,
     265,   266,   267,   268,   269,   270,   271,   272,   273,   274,
     275,   276,   277,   278,   279,   280,   281,   282,   283,   284,
     285,   286,   287,   288,   289,   290,   291,   292,   293,   294,
     295,   296,   297,   298,   299,   300,   301,   302,   303,   304,
     305,   306,   307,   308,   309,   310,   311,   312,   313,   314,
     315
};
# endif

/* YYR1[YYN] -- Symbol number of symbol that rule YYN derives.  */
static const yytype_uint8 yyr1[] =
{
       0,    61,    62,    62,    62,    62,    62,    62,    62,    62,
      62,    62,    62,    63,    64,    65,    65,    66,    66,    67,
      68,    69,    70,    71,    71,    72,    72,    72,    73,    73,
      74,    74,    75,    75,    75,    76,    76,    76,    77,    78,
      78,    78,    79,    80,    80,    81,    81,    81,    82,    82,
      82,    82,    82,    82,    82,    82,    82,    83,    84,    84,
      85,    85,    85,    85,    85,    86,    86,    86,    86,    86,
      86,    86,    86,    86,    86,    86,    86,    86,    86,    86,
      86,    86,    87,    87,    88,    88,    88,    89,    89,    90,
      90,    91,    92,    93,    94,    94,    94,    94,    94,    94,
      94,    94,    95,    96,    97,    98,    99,   100,   101,   102,
     103,   103,   103,   103,   104,   105,   106,   107,   107,   107,
     108,   109,   110,   111,   112,   112,   112,   112,   112,   113,
     113,   113,   114,   115,   115,   116,   116,   117,   117,   117,
     118,   118,   118,   119,   120,   121
};

/* YYR2[YYN] -- Number of symbols composing right hand side of rule YYN.  */
static const yytype_uint8 yyr2[] =
{
       0,     2,     0,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     3,     6,     6,     7,     5,     4,     5,
       5,     5,     5,     5,     6,     0,     2,     2,     0,     2,
       2,     3,     0,     3,     2,     0,     2,     2,     3,     0,
       2,     2,     5,     1,     3,     3,     3,     1,     4,     4,
       4,     4,     4,     4,     4,     4,     4,     1,     0,     2,
       1,     1,     4,     7,     4,     1,     1,     1,     1,     1,
       1,     1,     1,     1,     1,     1,     1,     1,     1,     1,
       1,     1,     1,     3,     0,     2,     2,     4,     2,     3,
       1,     2,     2,     4,     1,     1,     1,     1,     1,     1,
       1,     1,     3,     4,     3,     2,     2,     2,     2,     2,
       1,     1,     1,     1,     4,     4,     4,     1,     1,     1,
       2,     1,     2,     1,     1,     1,     1,     1,     1,     3,
       3,     3,     3,     2,     2,     6,     6,     1,     1,     1,
       1,     1,     1,     2,     4,     3
};

/* YYDEFACT[STATE-NAME] -- Default reduction number in state STATE-NUM.
   Performed when YYTABLE doesn't specify something else to do.  Zero
   means the default is an error.  */
static const yytype_uint8 yydefact[] =
{
       2,    28,     1,     0,     0,     0,     0,     0,     0,     0,
      11,     3,     4,     5,     6,     7,     8,     9,    10,     0,
      12,    94,    95,    96,    97,    98,    99,   100,   101,     0,
      32,     0,     0,     0,   105,     0,   106,     0,   108,     0,
     107,     0,   109,     0,     0,     0,    29,    13,     0,    32,
      91,    35,    39,    39,    39,    84,     0,   102,     0,     0,
     104,    30,    47,     0,    43,     0,    18,     0,   123,    34,
       0,     0,     0,     0,     0,     0,    25,     0,   103,    25,
       0,    31,     0,     0,    92,    33,    17,     0,    19,     0,
       0,     0,    36,   128,    37,   124,   125,   126,   127,     0,
      21,    40,   121,    41,   117,   118,   119,    20,    22,     0,
      23,     0,    85,     0,    86,   140,   141,   142,     0,    25,
       0,    46,    44,    45,     0,     0,     0,   133,     0,   134,
       0,   120,   122,   143,     0,    88,     0,    24,    14,    58,
      26,   113,    27,   110,   111,   112,     0,    15,   129,   130,
     131,     0,     0,     0,   132,   138,   137,   139,     0,   138,
     137,   139,     0,    38,     0,   145,     0,    89,     0,    16,
      48,    49,    50,    52,    53,    51,   138,   137,   139,     0,
      55,    56,    54,     0,     0,   144,    87,    65,    66,    67,
      68,    69,    70,    71,    72,    73,    74,    75,    76,    77,
      78,    79,    81,    80,     0,     0,    57,    61,    59,     0,
      60,     0,     0,    93,     0,     0,   114,     0,     0,   115,
     116,   135,   136,    61,     0,     0,    82,     0,    42,    64,
       0,    62,     0,     0,    83,     0,    63
};

/* YYDEFGOTO[NTERM-NUM].  */
static const yytype_int16 yydefgoto[] =
{
      -1,     1,    10,    11,    12,    13,    14,    15,    16,    17,
      18,   118,   139,    46,    48,    71,   101,    72,   140,    63,
      67,    92,   208,   168,   209,   210,   227,    75,   112,   113,
      34,   102,   132,    20,    21,    22,    23,    24,    25,    26,
      27,    28,   142,   143,   144,   145,   103,   104,   105,   106,
      69,    94,    95,    96,    97,    98,   158,   114,   115,   116,
     117
};

/* YYPACT[STATE-NUM] -- Index in YYTABLE of the portion describing
   STATE-NUM.  */
#define YYPACT_NINF -109
static const yytype_int16 yypact[] =
{
    -109,   190,  -109,   -22,   -16,     3,     5,     6,     7,     8,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,   116,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,    15,
    -109,    47,    80,    92,  -109,   123,  -109,   124,  -109,   131,
    -109,   135,  -109,     9,     0,   -14,  -109,  -109,    17,  -109,
    -109,  -109,  -109,  -109,  -109,  -109,   164,  -109,    10,   165,
    -109,  -109,   110,   114,   173,   178,  -109,   121,  -109,  -109,
      20,   119,    29,   112,   204,     1,  -109,   184,  -109,  -109,
     185,  -109,   187,   188,  -109,  -109,  -109,   128,  -109,   203,
      11,    18,  -109,  -109,  -109,  -109,  -109,  -109,  -109,    12,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,   208,
    -109,    72,  -109,   213,  -109,  -109,  -109,  -109,    67,  -109,
     113,  -109,   173,  -109,    -2,    55,   155,  -109,   157,  -109,
     -18,  -109,  -109,  -109,   210,  -109,    23,  -109,  -109,   214,
    -109,  -109,  -109,  -109,  -109,  -109,   159,  -109,  -109,  -109,
    -109,   215,   216,   217,  -109,   218,   219,   220,   186,   221,
     222,   223,   186,  -109,   247,  -109,   224,   225,   205,  -109,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,   258,
    -109,  -109,  -109,   281,   248,  -109,  -109,  -109,  -109,  -109,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,
    -109,  -109,  -109,  -109,   244,   245,  -109,  -109,  -109,     2,
    -109,   252,   253,  -109,   251,   251,  -109,   232,   -10,  -109,
    -109,  -109,  -109,  -109,    -7,    -4,  -109,   168,  -109,  -109,
     249,  -109,   235,   232,  -109,    -5,  -109
};

/* YYPGOTO[NTERM-NUM].  */
static const yytype_int16 yypgoto[] =
{
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,
    -109,   -65,   289,  -109,   243,  -109,  -109,   -26,  -109,  -109,
     -29,  -109,  -109,  -109,  -108,  -109,    60,  -109,  -109,  -109,
     109,   -48,    85,  -109,  -109,  -109,  -109,  -109,  -109,  -109,
    -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,  -109,
    -109,  -109,  -109,  -109,  -109,  -109,   -53,  -109,  -109,  -109,
    -109
};

/* YYTABLE[YYPACT[STATE-NUM]].  What to do in state STATE-NUM.  If
   positive, shift that token.  If negative, reduce the rule which
   number is the opposite.  If YYTABLE_NINF, syntax error.  */
#define YYTABLE_NINF -91
static const yytype_int16 yytable[] =
{
      68,    32,   109,    65,    32,    58,    32,    32,    32,    32,
      32,    32,    65,    65,   120,    30,    64,   163,    65,    65,
      61,    65,    68,    93,   166,   228,   217,    73,    74,   217,
      65,   232,    29,   110,   229,   217,    99,   230,   236,   164,
      62,    31,   127,   129,   148,   149,   150,   164,   126,    66,
      47,   131,    86,   122,   146,   128,    65,    59,   111,   218,
      33,   100,    35,    37,    39,    41,    56,    77,    65,   130,
     141,    62,   141,   134,    62,   162,   167,   154,    49,   -28,
     -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,
     -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   141,   138,
     -28,   151,   152,   153,   -90,   179,   224,   225,   135,   183,
     136,   -28,    50,    65,    65,    36,    38,    40,    42,    99,
      87,    43,    44,    51,   -28,   -28,   -28,   -28,   -28,   -28,
     -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,
     -28,   -28,   -28,   -28,   107,   147,   -28,    80,    81,    45,
      82,    88,    57,    60,    52,    53,    85,   -28,    83,   216,
      65,   219,    54,    84,    89,   124,    55,    78,    90,    91,
     -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,
     -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,   -28,
       2,   169,   -28,     3,     4,    76,    79,     5,     6,     7,
       8,     9,   231,   -28,   232,    65,   155,   156,   159,   160,
      83,    99,   157,    84,   161,   119,   -28,   187,   188,   189,
     190,   191,   192,   193,   194,   195,   196,   197,   198,   199,
     200,   201,   202,   203,   204,   205,   108,   176,   177,   121,
     125,    62,   123,   178,   133,   137,   165,    45,   184,   206,
     170,   171,   172,   173,   174,   175,   180,   181,   182,   211,
     185,   186,   207,   187,   188,   189,   190,   191,   192,   193,
     194,   195,   196,   197,   198,   199,   200,   201,   202,   203,
     204,   205,   212,   213,   214,   215,   226,   221,   222,   234,
      19,   233,    70,   235,   220,     0,     0,     0,     0,     0,
       0,     0,     0,     0,     0,     0,     0,     0,   223
};

#define yypact_value_is_default(Yystate) \
  (!!((Yystate) == (-109)))

#define yytable_value_is_error(Yytable_value) \
  YYID (0)

static const yytype_int16 yycheck[] =
{
      48,     1,     1,     1,     1,     5,     1,     1,     1,     1,
       1,     1,     1,     1,    79,    31,    45,    35,     1,     1,
      34,     1,    70,    71,     1,    35,    33,    53,    54,    33,
       1,    36,    54,    32,    41,    33,     7,    41,    43,    57,
      54,    57,    90,    91,    46,    47,    48,    57,    37,    32,
      35,    99,    32,    82,   119,    37,     1,    57,    57,    57,
      57,    32,    57,    57,    57,    57,    57,    57,     1,    57,
     118,    54,   120,     1,    54,   128,    53,   125,    31,    12,
      13,    14,    15,    16,    17,    18,    19,    20,    21,    22,
      23,    24,    25,    26,    27,    28,    29,    30,   146,    32,
      33,    46,    47,    48,    32,   158,   214,   215,    36,   162,
      38,    44,    32,     1,     1,     6,     7,     8,     9,     7,
       1,     5,     6,    31,    57,    12,    13,    14,    15,    16,
      17,    18,    19,    20,    21,    22,    23,    24,    25,    26,
      27,    28,    29,    30,    32,    32,    33,    37,    34,    33,
      36,    32,    43,    44,    31,    31,    35,    44,    37,   207,
       1,   209,    31,    35,    45,    37,    31,    58,    49,    50,
      57,    12,    13,    14,    15,    16,    17,    18,    19,    20,
      21,    22,    23,    24,    25,    26,    27,    28,    29,    30,
       0,    32,    33,     3,     4,    31,    31,     7,     8,     9,
      10,    11,    34,    44,    36,     1,    51,    52,    51,    52,
      37,     7,    57,    35,    57,    31,    57,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    32,    51,    52,    54,
      37,    54,    54,    57,    36,    32,    36,    33,     1,    44,
      35,    35,    35,    35,    35,    35,    35,    35,    35,     1,
      36,    36,    57,    12,    13,    14,    15,    16,    17,    18,
      19,    20,    21,    22,    23,    24,    25,    26,    27,    28,
      29,    30,     1,    35,    40,    40,    54,    35,    35,    54,
       1,    42,    49,   233,   209,    -1,    -1,    -1,    -1,    -1,
      -1,    -1,    -1,    -1,    -1,    -1,    -1,    -1,    57
};

/* YYSTOS[STATE-NUM] -- The (internal number of the) accessing
   symbol of state STATE-NUM.  */
static const yytype_uint8 yystos[] =
{
       0,    62,     0,     3,     4,     7,     8,     9,    10,    11,
      63,    64,    65,    66,    67,    68,    69,    70,    71,    73,
      94,    95,    96,    97,    98,    99,   100,   101,   102,    54,
      31,    57,     1,    57,    91,    57,    91,    57,    91,    57,
      91,    57,    91,     5,     6,    33,    74,    35,    75,    31,
      32,    31,    31,    31,    31,    31,    57,    91,     5,    57,
      91,    34,    54,    80,    81,     1,    32,    81,    92,   111,
      75,    76,    78,    78,    78,    88,    31,    57,    91,    31,
      37,    34,    36,    37,    35,    35,    32,     1,    32,    45,
      49,    50,    82,    92,   112,   113,   114,   115,   116,     7,
      32,    77,    92,   107,   108,   109,   110,    32,    32,     1,
      32,    57,    89,    90,   118,   119,   120,   121,    72,    31,
      72,    54,    81,    54,    37,    37,    37,    92,    37,    92,
      57,    92,    93,    36,     1,    36,    38,    32,    32,    73,
      79,    92,   103,   104,   105,   106,    72,    32,    46,    47,
      48,    46,    47,    48,    92,    51,    52,    57,   117,    51,
      52,    57,   117,    35,    57,    36,     1,    53,    84,    32,
      35,    35,    35,    35,    35,    35,    51,    52,    57,   117,
      35,    35,    35,   117,     1,    36,    36,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    44,    57,    83,    85,
      86,     1,     1,    35,    40,    40,    92,    33,    57,    92,
      93,    35,    35,    57,    85,    85,    54,    87,    35,    41,
      41,    34,    36,    42,    54,    87,    43
};

#define yyerrok		(yyerrstatus = 0)
#define yyclearin	(yychar = YYEMPTY)
#define YYEMPTY		(-2)
#define YYEOF		0

#define YYACCEPT	goto yyacceptlab
#define YYABORT		goto yyabortlab
#define YYERROR		goto yyerrorlab


/* Like YYERROR except do call yyerror.  This remains here temporarily
   to ease the transition to the new meaning of YYERROR, for GCC.
   Once GCC version 2 has supplanted version 1, this can go.  However,
   YYFAIL appears to be in use.  Nevertheless, it is formally deprecated
   in Bison 2.4.2's NEWS entry, where a plan to phase it out is
   discussed.  */

#define YYFAIL		goto yyerrlab
#if defined YYFAIL
  /* This is here to suppress warnings from the GCC cpp's
     -Wunused-macros.  Normally we don't worry about that warning, but
     some users do, and we want to make it easy for users to remove
     YYFAIL uses, which will produce warnings from Bison 2.5.  */
#endif

#define YYRECOVERING()  (!!yyerrstatus)

#define YYBACKUP(Token, Value)                                  \
do                                                              \
  if (yychar == YYEMPTY)                                        \
    {                                                           \
      yychar = (Token);                                         \
      yylval = (Value);                                         \
      YYPOPSTACK (yylen);                                       \
      yystate = *yyssp;                                         \
      goto yybackup;                                            \
    }                                                           \
  else                                                          \
    {                                                           \
      yyerror (YY_("syntax error: cannot back up")); \
      YYERROR;							\
    }								\
while (YYID (0))

/* Error token number */
#define YYTERROR	1
#define YYERRCODE	256


/* YYLLOC_DEFAULT -- Set CURRENT to span from RHS[1] to RHS[N].
   If N is 0, then set CURRENT to the empty location which ends
   the previous symbol: RHS[0] (always defined).  */

#ifndef YYLLOC_DEFAULT
# define YYLLOC_DEFAULT(Current, Rhs, N)                                \
    do                                                                  \
      if (YYID (N))                                                     \
        {                                                               \
          (Current).first_line   = YYRHSLOC (Rhs, 1).first_line;        \
          (Current).first_column = YYRHSLOC (Rhs, 1).first_column;      \
          (Current).last_line    = YYRHSLOC (Rhs, N).last_line;         \
          (Current).last_column  = YYRHSLOC (Rhs, N).last_column;       \
        }                                                               \
      else                                                              \
        {                                                               \
          (Current).first_line   = (Current).last_line   =              \
            YYRHSLOC (Rhs, 0).last_line;                                \
          (Current).first_column = (Current).last_column =              \
            YYRHSLOC (Rhs, 0).last_column;                              \
        }                                                               \
    while (YYID (0))
#endif

#define YYRHSLOC(Rhs, K) ((Rhs)[K])


/* YY_LOCATION_PRINT -- Print the location on the stream.
   This macro was not mandated originally: define only if we know
   we won't break user code: when these are the locations we know.  */

#ifndef __attribute__
/* This feature is available in gcc versions 2.5 and later.  */
# if (! defined __GNUC__ || __GNUC__ < 2 \
      || (__GNUC__ == 2 && __GNUC_MINOR__ < 5))
#  define __attribute__(Spec) /* empty */
# endif
#endif

#ifndef YY_LOCATION_PRINT
# if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL

/* Print *YYLOCP on YYO.  Private, do not rely on its existence. */

__attribute__((__unused__))
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static unsigned
yy_location_print_ (FILE *yyo, YYLTYPE const * const yylocp)
#else
static unsigned
yy_location_print_ (yyo, yylocp)
    FILE *yyo;
    YYLTYPE const * const yylocp;
#endif
{
  unsigned res = 0;
  int end_col = 0 != yylocp->last_column ? yylocp->last_column - 1 : 0;
  if (0 <= yylocp->first_line)
    {
      res += fprintf (yyo, "%d", yylocp->first_line);
      if (0 <= yylocp->first_column)
        res += fprintf (yyo, ".%d", yylocp->first_column);
    }
  if (0 <= yylocp->last_line)
    {
      if (yylocp->first_line < yylocp->last_line)
        {
          res += fprintf (yyo, "-%d", yylocp->last_line);
          if (0 <= end_col)
            res += fprintf (yyo, ".%d", end_col);
        }
      else if (0 <= end_col && yylocp->first_column < end_col)
        res += fprintf (yyo, "-%d", end_col);
    }
  return res;
 }

#  define YY_LOCATION_PRINT(File, Loc)          \
  yy_location_print_ (File, &(Loc))

# else
#  define YY_LOCATION_PRINT(File, Loc) ((void) 0)
# endif
#endif


/* YYLEX -- calling `yylex' with the right arguments.  */
#ifdef YYLEX_PARAM
# define YYLEX yylex (YYLEX_PARAM)
#else
# define YYLEX yylex ()
#endif

/* Enable debugging if requested.  */
#if YYDEBUG

# ifndef YYFPRINTF
#  include <stdio.h> /* INFRINGES ON USER NAME SPACE */
#  define YYFPRINTF fprintf
# endif

# define YYDPRINTF(Args)			\
do {						\
  if (yydebug)					\
    YYFPRINTF Args;				\
} while (YYID (0))

# define YY_SYMBOL_PRINT(Title, Type, Value, Location)			  \
do {									  \
  if (yydebug)								  \
    {									  \
      YYFPRINTF (stderr, "%s ", Title);					  \
      yy_symbol_print (stderr,						  \
		  Type, Value, Location); \
      YYFPRINTF (stderr, "\n");						  \
    }									  \
} while (YYID (0))


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_value_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp)
#else
static void
yy_symbol_value_print (yyoutput, yytype, yyvaluep, yylocationp)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
    YYLTYPE const * const yylocationp;
#endif
{
  FILE *yyo = yyoutput;
  YYUSE (yyo);
  if (!yyvaluep)
    return;
  YYUSE (yylocationp);
# ifdef YYPRINT
  if (yytype < YYNTOKENS)
    YYPRINT (yyoutput, yytoknum[yytype], *yyvaluep);
# else
  YYUSE (yyoutput);
# endif
  switch (yytype)
    {
      default:
        break;
    }
}


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp)
#else
static void
yy_symbol_print (yyoutput, yytype, yyvaluep, yylocationp)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
    YYLTYPE const * const yylocationp;
#endif
{
  if (yytype < YYNTOKENS)
    YYFPRINTF (yyoutput, "token %s (", yytname[yytype]);
  else
    YYFPRINTF (yyoutput, "nterm %s (", yytname[yytype]);

  YY_LOCATION_PRINT (yyoutput, *yylocationp);
  YYFPRINTF (yyoutput, ": ");
  yy_symbol_value_print (yyoutput, yytype, yyvaluep, yylocationp);
  YYFPRINTF (yyoutput, ")");
}

/*------------------------------------------------------------------.
| yy_stack_print -- Print the state stack from its BOTTOM up to its |
| TOP (included).                                                   |
`------------------------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_stack_print (yytype_int16 *yybottom, yytype_int16 *yytop)
#else
static void
yy_stack_print (yybottom, yytop)
    yytype_int16 *yybottom;
    yytype_int16 *yytop;
#endif
{
  YYFPRINTF (stderr, "Stack now");
  for (; yybottom <= yytop; yybottom++)
    {
      int yybot = *yybottom;
      YYFPRINTF (stderr, " %d", yybot);
    }
  YYFPRINTF (stderr, "\n");
}

# define YY_STACK_PRINT(Bottom, Top)				\
do {								\
  if (yydebug)							\
    yy_stack_print ((Bottom), (Top));				\
} while (YYID (0))


/*------------------------------------------------.
| Report that the YYRULE is going to be reduced.  |
`------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_reduce_print (YYSTYPE *yyvsp, YYLTYPE *yylsp, int yyrule)
#else
static void
yy_reduce_print (yyvsp, yylsp, yyrule)
    YYSTYPE *yyvsp;
    YYLTYPE *yylsp;
    int yyrule;
#endif
{
  int yynrhs = yyr2[yyrule];
  int yyi;
  unsigned long int yylno = yyrline[yyrule];
  YYFPRINTF (stderr, "Reducing stack by rule %d (line %lu):\n",
	     yyrule - 1, yylno);
  /* The symbols being reduced.  */
  for (yyi = 0; yyi < yynrhs; yyi++)
    {
      YYFPRINTF (stderr, "   $%d = ", yyi + 1);
      yy_symbol_print (stderr, yyrhs[yyprhs[yyrule] + yyi],
		       &(yyvsp[(yyi + 1) - (yynrhs)])
		       , &(yylsp[(yyi + 1) - (yynrhs)])		       );
      YYFPRINTF (stderr, "\n");
    }
}

# define YY_REDUCE_PRINT(Rule)		\
do {					\
  if (yydebug)				\
    yy_reduce_print (yyvsp, yylsp, Rule); \
} while (YYID (0))

/* Nonzero means print parse trace.  It is left uninitialized so that
   multiple parsers can coexist.  */
int yydebug;
#else /* !YYDEBUG */
# define YYDPRINTF(Args)
# define YY_SYMBOL_PRINT(Title, Type, Value, Location)
# define YY_STACK_PRINT(Bottom, Top)
# define YY_REDUCE_PRINT(Rule)
#endif /* !YYDEBUG */


/* YYINITDEPTH -- initial size of the parser's stacks.  */
#ifndef	YYINITDEPTH
# define YYINITDEPTH 200
#endif

/* YYMAXDEPTH -- maximum size the stacks can grow to (effective only
   if the built-in stack extension method is used).

   Do not make this value too large; the results are undefined if
   YYSTACK_ALLOC_MAXIMUM < YYSTACK_BYTES (YYMAXDEPTH)
   evaluated with infinite-precision integer arithmetic.  */

#ifndef YYMAXDEPTH
# define YYMAXDEPTH 10000
#endif


#if YYERROR_VERBOSE

# ifndef yystrlen
#  if defined __GLIBC__ && defined _STRING_H
#   define yystrlen strlen
#  else
/* Return the length of YYSTR.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static YYSIZE_T
yystrlen (const char *yystr)
#else
static YYSIZE_T
yystrlen (yystr)
    const char *yystr;
#endif
{
  YYSIZE_T yylen;
  for (yylen = 0; yystr[yylen]; yylen++)
    continue;
  return yylen;
}
#  endif
# endif

# ifndef yystpcpy
#  if defined __GLIBC__ && defined _STRING_H && defined _GNU_SOURCE
#   define yystpcpy stpcpy
#  else
/* Copy YYSRC to YYDEST, returning the address of the terminating '\0' in
   YYDEST.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static char *
yystpcpy (char *yydest, const char *yysrc)
#else
static char *
yystpcpy (yydest, yysrc)
    char *yydest;
    const char *yysrc;
#endif
{
  char *yyd = yydest;
  const char *yys = yysrc;

  while ((*yyd++ = *yys++) != '\0')
    continue;

  return yyd - 1;
}
#  endif
# endif

# ifndef yytnamerr
/* Copy to YYRES the contents of YYSTR after stripping away unnecessary
   quotes and backslashes, so that it's suitable for yyerror.  The
   heuristic is that double-quoting is unnecessary unless the string
   contains an apostrophe, a comma, or backslash (other than
   backslash-backslash).  YYSTR is taken from yytname.  If YYRES is
   null, do not copy; instead, return the length of what the result
   would have been.  */
static YYSIZE_T
yytnamerr (char *yyres, const char *yystr)
{
  if (*yystr == '"')
    {
      YYSIZE_T yyn = 0;
      char const *yyp = yystr;

      for (;;)
	switch (*++yyp)
	  {
	  case '\'':
	  case ',':
	    goto do_not_strip_quotes;

	  case '\\':
	    if (*++yyp != '\\')
	      goto do_not_strip_quotes;
	    /* Fall through.  */
	  default:
	    if (yyres)
	      yyres[yyn] = *yyp;
	    yyn++;
	    break;

	  case '"':
	    if (yyres)
	      yyres[yyn] = '\0';
	    return yyn;
	  }
    do_not_strip_quotes: ;
    }

  if (! yyres)
    return yystrlen (yystr);

  return yystpcpy (yyres, yystr) - yyres;
}
# endif

/* Copy into *YYMSG, which is of size *YYMSG_ALLOC, an error message
   about the unexpected token YYTOKEN for the state stack whose top is
   YYSSP.

   Return 0 if *YYMSG was successfully written.  Return 1 if *YYMSG is
   not large enough to hold the message.  In that case, also set
   *YYMSG_ALLOC to the required number of bytes.  Return 2 if the
   required number of bytes is too large to store.  */
static int
yysyntax_error (YYSIZE_T *yymsg_alloc, char **yymsg,
                yytype_int16 *yyssp, int yytoken)
{
  YYSIZE_T yysize0 = yytnamerr (YY_NULL, yytname[yytoken]);
  YYSIZE_T yysize = yysize0;
  enum { YYERROR_VERBOSE_ARGS_MAXIMUM = 5 };
  /* Internationalized format string. */
  const char *yyformat = YY_NULL;
  /* Arguments of yyformat. */
  char const *yyarg[YYERROR_VERBOSE_ARGS_MAXIMUM];
  /* Number of reported tokens (one for the "unexpected", one per
     "expected"). */
  int yycount = 0;

  /* There are many possibilities here to consider:
     - Assume YYFAIL is not used.  It's too flawed to consider.  See
       <http://lists.gnu.org/archive/html/bison-patches/2009-12/msg00024.html>
       for details.  YYERROR is fine as it does not invoke this
       function.
     - If this state is a consistent state with a default action, then
       the only way this function was invoked is if the default action
       is an error action.  In that case, don't check for expected
       tokens because there are none.
     - The only way there can be no lookahead present (in yychar) is if
       this state is a consistent state with a default action.  Thus,
       detecting the absence of a lookahead is sufficient to determine
       that there is no unexpected or expected token to report.  In that
       case, just report a simple "syntax error".
     - Don't assume there isn't a lookahead just because this state is a
       consistent state with a default action.  There might have been a
       previous inconsistent state, consistent state with a non-default
       action, or user semantic action that manipulated yychar.
     - Of course, the expected token list depends on states to have
       correct lookahead information, and it depends on the parser not
       to perform extra reductions after fetching a lookahead from the
       scanner and before detecting a syntax error.  Thus, state merging
       (from LALR or IELR) and default reductions corrupt the expected
       token list.  However, the list is correct for canonical LR with
       one exception: it will still contain any token that will not be
       accepted due to an error action in a later state.
  */
  if (yytoken != YYEMPTY)
    {
      int yyn = yypact[*yyssp];
      yyarg[yycount++] = yytname[yytoken];
      if (!yypact_value_is_default (yyn))
        {
          /* Start YYX at -YYN if negative to avoid negative indexes in
             YYCHECK.  In other words, skip the first -YYN actions for
             this state because they are default actions.  */
          int yyxbegin = yyn < 0 ? -yyn : 0;
          /* Stay within bounds of both yycheck and yytname.  */
          int yychecklim = YYLAST - yyn + 1;
          int yyxend = yychecklim < YYNTOKENS ? yychecklim : YYNTOKENS;
          int yyx;

          for (yyx = yyxbegin; yyx < yyxend; ++yyx)
            if (yycheck[yyx + yyn] == yyx && yyx != YYTERROR
                && !yytable_value_is_error (yytable[yyx + yyn]))
              {
                if (yycount == YYERROR_VERBOSE_ARGS_MAXIMUM)
                  {
                    yycount = 1;
                    yysize = yysize0;
                    break;
                  }
                yyarg[yycount++] = yytname[yyx];
                {
                  YYSIZE_T yysize1 = yysize + yytnamerr (YY_NULL, yytname[yyx]);
                  if (! (yysize <= yysize1
                         && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
                    return 2;
                  yysize = yysize1;
                }
              }
        }
    }

  switch (yycount)
    {
# define YYCASE_(N, S)                      \
      case N:                               \
        yyformat = S;                       \
      break
      YYCASE_(0, YY_("syntax error"));
      YYCASE_(1, YY_("syntax error, unexpected %s"));
      YYCASE_(2, YY_("syntax error, unexpected %s, expecting %s"));
      YYCASE_(3, YY_("syntax error, unexpected %s, expecting %s or %s"));
      YYCASE_(4, YY_("syntax error, unexpected %s, expecting %s or %s or %s"));
      YYCASE_(5, YY_("syntax error, unexpected %s, expecting %s or %s or %s or %s"));
# undef YYCASE_
    }

  {
    YYSIZE_T yysize1 = yysize + yystrlen (yyformat);
    if (! (yysize <= yysize1 && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
      return 2;
    yysize = yysize1;
  }

  if (*yymsg_alloc < yysize)
    {
      *yymsg_alloc = 2 * yysize;
      if (! (yysize <= *yymsg_alloc
             && *yymsg_alloc <= YYSTACK_ALLOC_MAXIMUM))
        *yymsg_alloc = YYSTACK_ALLOC_MAXIMUM;
      return 1;
    }

  /* Avoid sprintf, as that infringes on the user's name space.
     Don't have undefined behavior even if the translation
     produced a string with the wrong number of "%s"s.  */
  {
    char *yyp = *yymsg;
    int yyi = 0;
    while ((*yyp = *yyformat) != '\0')
      if (*yyp == '%' && yyformat[1] == 's' && yyi < yycount)
        {
          yyp += yytnamerr (yyp, yyarg[yyi++]);
          yyformat += 2;
        }
      else
        {
          yyp++;
          yyformat++;
        }
  }
  return 0;
}
#endif /* YYERROR_VERBOSE */

/*-----------------------------------------------.
| Release the memory associated to this symbol.  |
`-----------------------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yydestruct (const char *yymsg, int yytype, YYSTYPE *yyvaluep, YYLTYPE *yylocationp)
#else
static void
yydestruct (yymsg, yytype, yyvaluep, yylocationp)
    const char *yymsg;
    int yytype;
    YYSTYPE *yyvaluep;
    YYLTYPE *yylocationp;
#endif
{
  YYUSE (yyvaluep);
  YYUSE (yylocationp);

  if (!yymsg)
    yymsg = "Deleting";
  YY_SYMBOL_PRINT (yymsg, yytype, yyvaluep, yylocationp);

  switch (yytype)
    {

      default:
        break;
    }
}




/* The lookahead symbol.  */
int yychar;


#ifndef YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
# define YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
# define YY_IGNORE_MAYBE_UNINITIALIZED_END
#endif
#ifndef YY_INITIAL_VALUE
# define YY_INITIAL_VALUE(Value) /* Nothing. */
#endif

/* The semantic value of the lookahead symbol.  */
YYSTYPE yylval YY_INITIAL_VALUE(yyval_default);

/* Location data for the lookahead symbol.  */
YYLTYPE yylloc
# if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL
  = { 1, 1, 1, 1 }
# endif
;


/* Number of syntax errors so far.  */
int yynerrs;


/*----------.
| yyparse.  |
`----------*/

#ifdef YYPARSE_PARAM
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (void *YYPARSE_PARAM)
#else
int
yyparse (YYPARSE_PARAM)
    void *YYPARSE_PARAM;
#endif
#else /* ! YYPARSE_PARAM */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (void)
#else
int
yyparse ()

#endif
#endif
{
    int yystate;
    /* Number of tokens to shift before error messages enabled.  */
    int yyerrstatus;

    /* The stacks and their tools:
       `yyss': related to states.
       `yyvs': related to semantic values.
       `yyls': related to locations.

       Refer to the stacks through separate pointers, to allow yyoverflow
       to reallocate them elsewhere.  */

    /* The state stack.  */
    yytype_int16 yyssa[YYINITDEPTH];
    yytype_int16 *yyss;
    yytype_int16 *yyssp;

    /* The semantic value stack.  */
    YYSTYPE yyvsa[YYINITDEPTH];
    YYSTYPE *yyvs;
    YYSTYPE *yyvsp;

    /* The location stack.  */
    YYLTYPE yylsa[YYINITDEPTH];
    YYLTYPE *yyls;
    YYLTYPE *yylsp;

    /* The locations where the error started and ended.  */
    YYLTYPE yyerror_range[3];

    YYSIZE_T yystacksize;

  int yyn;
  int yyresult;
  /* Lookahead token as an internal (translated) token number.  */
  int yytoken = 0;
  /* The variables used to return semantic value and location from the
     action routines.  */
  YYSTYPE yyval;
  YYLTYPE yyloc;

#if YYERROR_VERBOSE
  /* Buffer for error messages, and its allocated size.  */
  char yymsgbuf[128];
  char *yymsg = yymsgbuf;
  YYSIZE_T yymsg_alloc = sizeof yymsgbuf;
#endif

#define YYPOPSTACK(N)   (yyvsp -= (N), yyssp -= (N), yylsp -= (N))

  /* The number of symbols on the RHS of the reduced rule.
     Keep to zero when no symbol should be popped.  */
  int yylen = 0;

  yyssp = yyss = yyssa;
  yyvsp = yyvs = yyvsa;
  yylsp = yyls = yylsa;
  yystacksize = YYINITDEPTH;

  YYDPRINTF ((stderr, "Starting parse\n"));

  yystate = 0;
  yyerrstatus = 0;
  yynerrs = 0;
  yychar = YYEMPTY; /* Cause a token to be read.  */
  yylsp[0] = yylloc;
  goto yysetstate;

/*------------------------------------------------------------.
| yynewstate -- Push a new state, which is found in yystate.  |
`------------------------------------------------------------*/
 yynewstate:
  /* In all cases, when you get here, the value and location stacks
     have just been pushed.  So pushing a state here evens the stacks.  */
  yyssp++;

 yysetstate:
  *yyssp = yystate;

  if (yyss + yystacksize - 1 <= yyssp)
    {
      /* Get the current used size of the three stacks, in elements.  */
      YYSIZE_T yysize = yyssp - yyss + 1;

#ifdef yyoverflow
      {
	/* Give user a chance to reallocate the stack.  Use copies of
	   these so that the &'s don't force the real ones into
	   memory.  */
	YYSTYPE *yyvs1 = yyvs;
	yytype_int16 *yyss1 = yyss;
	YYLTYPE *yyls1 = yyls;

	/* Each stack pointer address is followed by the size of the
	   data in use in that stack, in bytes.  This used to be a
	   conditional around just the two extra args, but that might
	   be undefined if yyoverflow is a macro.  */
	yyoverflow (YY_("memory exhausted"),
		    &yyss1, yysize * sizeof (*yyssp),
		    &yyvs1, yysize * sizeof (*yyvsp),
		    &yyls1, yysize * sizeof (*yylsp),
		    &yystacksize);

	yyls = yyls1;
	yyss = yyss1;
	yyvs = yyvs1;
      }
#else /* no yyoverflow */
# ifndef YYSTACK_RELOCATE
      goto yyexhaustedlab;
# else
      /* Extend the stack our own way.  */
      if (YYMAXDEPTH <= yystacksize)
	goto yyexhaustedlab;
      yystacksize *= 2;
      if (YYMAXDEPTH < yystacksize)
	yystacksize = YYMAXDEPTH;

      {
	yytype_int16 *yyss1 = yyss;
	union yyalloc *yyptr =
	  (union yyalloc *) YYSTACK_ALLOC (YYSTACK_BYTES (yystacksize));
	if (! yyptr)
	  goto yyexhaustedlab;
	YYSTACK_RELOCATE (yyss_alloc, yyss);
	YYSTACK_RELOCATE (yyvs_alloc, yyvs);
	YYSTACK_RELOCATE (yyls_alloc, yyls);
#  undef YYSTACK_RELOCATE
	if (yyss1 != yyssa)
	  YYSTACK_FREE (yyss1);
      }
# endif
#endif /* no yyoverflow */

      yyssp = yyss + yysize - 1;
      yyvsp = yyvs + yysize - 1;
      yylsp = yyls + yysize - 1;

      YYDPRINTF ((stderr, "Stack size increased to %lu\n",
		  (unsigned long int) yystacksize));

      if (yyss + yystacksize - 1 <= yyssp)
	YYABORT;
    }

  YYDPRINTF ((stderr, "Entering state %d\n", yystate));

  if (yystate == YYFINAL)
    YYACCEPT;

  goto yybackup;

/*-----------.
| yybackup.  |
`-----------*/
yybackup:

  /* Do appropriate processing given the current state.  Read a
     lookahead token if we need one and don't already have one.  */

  /* First try to decide what to do without reference to lookahead token.  */
  yyn = yypact[yystate];
  if (yypact_value_is_default (yyn))
    goto yydefault;

  /* Not known => get a lookahead token if don't already have one.  */

  /* YYCHAR is either YYEMPTY or YYEOF or a valid lookahead symbol.  */
  if (yychar == YYEMPTY)
    {
      YYDPRINTF ((stderr, "Reading a token: "));
      yychar = YYLEX;
    }

  if (yychar <= YYEOF)
    {
      yychar = yytoken = YYEOF;
      YYDPRINTF ((stderr, "Now at end of input.\n"));
    }
  else
    {
      yytoken = YYTRANSLATE (yychar);
      YY_SYMBOL_PRINT ("Next token is", yytoken, &yylval, &yylloc);
    }

  /* If the proper action on seeing token YYTOKEN is to reduce or to
     detect an error, take that action.  */
  yyn += yytoken;
  if (yyn < 0 || YYLAST < yyn || yycheck[yyn] != yytoken)
    goto yydefault;
  yyn = yytable[yyn];
  if (yyn <= 0)
    {
      if (yytable_value_is_error (yyn))
        goto yyerrlab;
      yyn = -yyn;
      goto yyreduce;
    }

  /* Count tokens shifted since error; after three, turn off error
     status.  */
  if (yyerrstatus)
    yyerrstatus--;

  /* Shift the lookahead token.  */
  YY_SYMBOL_PRINT ("Shifting", yytoken, &yylval, &yylloc);

  /* Discard the shifted token.  */
  yychar = YYEMPTY;

  yystate = yyn;
  YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
  *++yyvsp = yylval;
  YY_IGNORE_MAYBE_UNINITIALIZED_END
  *++yylsp = yylloc;
  goto yynewstate;


/*-----------------------------------------------------------.
| yydefault -- do the default action for the current state.  |
`-----------------------------------------------------------*/
yydefault:
  yyn = yydefact[yystate];
  if (yyn == 0)
    goto yyerrlab;
  goto yyreduce;


/*-----------------------------.
| yyreduce -- Do a reduction.  |
`-----------------------------*/
yyreduce:
  /* yyn is the number of a rule to reduce with.  */
  yylen = yyr2[yyn];

  /* If YYLEN is nonzero, implement the default value of the action:
     `$$ = $1'.

     Otherwise, the following line sets YYVAL to garbage.
     This behavior is undocumented and Bison
     users should not rely upon it.  Assigning to YYVAL
     unconditionally makes the parser a bit smaller, and it avoids a
     GCC warning that YYVAL may be used uninitialized.  */
  yyval = yyvsp[1-yylen];

  /* Default location.  */
  YYLLOC_DEFAULT (yyloc, (yylsp - yylen), yylen);
  YY_REDUCE_PRINT (yyn);
  switch (yyn)
    {
        case 2:
/* Line 1792 of yacc.c  */
#line 188 "parser.y"
    { tsl = new NTSL(); (yyval.tsl) = tsl; }
    break;

  case 3:
/* Line 1792 of yacc.c  */
#line 189 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->structList->push_back((yyvsp[(2) - (2)].structDescriptor)); }
    break;

  case 4:
/* Line 1792 of yacc.c  */
#line 190 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->cellList->push_back((yyvsp[(2) - (2)].cellDescriptor)); }
    break;

  case 5:
/* Line 1792 of yacc.c  */
#line 191 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->settingsList->push_back((yyvsp[(2) - (2)].settingsDescriptor)); }
    break;

  case 6:
/* Line 1792 of yacc.c  */
#line 192 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->protocolList->push_back((yyvsp[(2) - (2)].protocolDescriptor)); }
    break;

  case 7:
/* Line 1792 of yacc.c  */
#line 193 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->proxyList->push_back((yyvsp[(2) - (2)].proxyDescriptor)); }
    break;

  case 8:
/* Line 1792 of yacc.c  */
#line 194 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->serverList->push_back((yyvsp[(2) - (2)].serverDescriptor)); }
    break;

  case 9:
/* Line 1792 of yacc.c  */
#line 195 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->moduleList->push_back((yyvsp[(2) - (2)].moduleDescriptor)); }
    break;

  case 10:
/* Line 1792 of yacc.c  */
#line 196 "parser.y"
    { (yyvsp[(1) - (2)].tsl)->enumList->push_back((yyvsp[(2) - (2)].enumDescriptor)); }
    break;

  case 11:
/* Line 1792 of yacc.c  */
#line 199 "parser.y"
    { }
    break;

  case 12:
/* Line 1792 of yacc.c  */
#line 200 "parser.y"
    { }
    break;

  case 13:
/* Line 1792 of yacc.c  */
#line 207 "parser.y"
    {push_new_file((yyvsp[(2) - (3)].string));}
    break;

  case 14:
/* Line 1792 of yacc.c  */
#line 220 "parser.y"
    { (yyval.structDescriptor) = new NStruct(); (yyval.structDescriptor)->fieldList = (yyvsp[(5) - (6)].fieldList); (yyval.structDescriptor)->attributes = (yyvsp[(1) - (6)].kvlist); (yyval.structDescriptor)->name = (yyvsp[(3) - (6)].string);}
    break;

  case 15:
/* Line 1792 of yacc.c  */
#line 225 "parser.y"
    { (yyval.cellDescriptor) = new NCell(); (yyval.cellDescriptor)->fieldList = (yyvsp[(5) - (6)].fieldList); (yyval.cellDescriptor)->attributes = (yyvsp[(1) - (6)].kvlist); (yyval.cellDescriptor)->name = (yyvsp[(3) - (6)].string);}
    break;

  case 16:
/* Line 1792 of yacc.c  */
#line 228 "parser.y"
    { (yyval.cellDescriptor) = new NCell(); (yyval.cellDescriptor)->fieldList = (yyvsp[(6) - (7)].fieldList); (yyval.cellDescriptor)->attributes = (yyvsp[(1) - (7)].kvlist); (yyval.cellDescriptor)->name = (yyvsp[(4) - (7)].string);}
    break;

  case 17:
/* Line 1792 of yacc.c  */
#line 233 "parser.y"
    { (yyval.settingsDescriptor) = new NTrinitySettings(); (yyval.settingsDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.settingsDescriptor)->settings = (yyvsp[(4) - (5)].kvlist); }
    break;

  case 18:
/* Line 1792 of yacc.c  */
#line 235 "parser.y"
    { (yyval.settingsDescriptor) = new NTrinitySettings(); (yyval.settingsDescriptor)->name = new std::string(); (yyval.settingsDescriptor)->settings = (yyvsp[(3) - (4)].kvlist); }
    break;

  case 19:
/* Line 1792 of yacc.c  */
#line 239 "parser.y"
    { (yyval.protocolDescriptor) = new NProtocol(); (yyval.protocolDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.protocolDescriptor)->protocolPropertyList = (yyvsp[(4) - (5)].proto_prop_list); }
    break;

  case 20:
/* Line 1792 of yacc.c  */
#line 243 "parser.y"
    { (yyval.proxyDescriptor) = new NProxy(); (yyval.proxyDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.proxyDescriptor)->protocolList = (yyvsp[(4) - (5)].proto_ref_list); }
    break;

  case 21:
/* Line 1792 of yacc.c  */
#line 247 "parser.y"
    { (yyval.serverDescriptor) = new NServer(); (yyval.serverDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.serverDescriptor)->protocolList = (yyvsp[(4) - (5)].proto_ref_list); }
    break;

  case 22:
/* Line 1792 of yacc.c  */
#line 251 "parser.y"
    { (yyval.moduleDescriptor) = new NModule(); (yyval.moduleDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.moduleDescriptor)->protocolList = (yyvsp[(4) - (5)].proto_ref_list); }
    break;

  case 23:
/* Line 1792 of yacc.c  */
#line 256 "parser.y"
    { (yyval.enumDescriptor) = new NEnum(); (yyval.enumDescriptor)->name = (yyvsp[(2) - (5)].string); (yyval.enumDescriptor)->enumEntryList = (yyvsp[(4) - (5)].enumEntryList); }
    break;

  case 24:
/* Line 1792 of yacc.c  */
#line 258 "parser.y"
    { (yyval.enumDescriptor) = new NEnum(); (yyval.enumDescriptor)->name = (yyvsp[(2) - (6)].string); (yyval.enumDescriptor)->enumEntryList = (yyvsp[(4) - (6)].enumEntryList); (yyval.enumDescriptor)->enumEntryList->push_back((yyvsp[(5) - (6)].enumEntry)); }
    break;

  case 25:
/* Line 1792 of yacc.c  */
#line 263 "parser.y"
    {(yyval.fieldList) = new vector<NField*>(); (yyloc).from_blank_rule = true;}
    break;

  case 26:
/* Line 1792 of yacc.c  */
#line 265 "parser.y"
    {(yyvsp[(1) - (2)].fieldList)->push_back((yyvsp[(2) - (2)].field));}
    break;

  case 28:
/* Line 1792 of yacc.c  */
#line 270 "parser.y"
    {(yyval.kvlist) = new vector<NKVPair*>(); (yyloc).from_blank_rule = true;}
    break;

  case 29:
/* Line 1792 of yacc.c  */
#line 272 "parser.y"
    {(yyvsp[(1) - (2)].kvlist)->insert((yyvsp[(1) - (2)].kvlist)->end(), (yyvsp[(2) - (2)].kvlist)->begin(), (yyvsp[(2) - (2)].kvlist)->end()); delete (yyvsp[(2) - (2)].kvlist); (yyval.kvlist) = (yyvsp[(1) - (2)].kvlist);}
    break;

  case 30:
/* Line 1792 of yacc.c  */
#line 277 "parser.y"
    {(yyval.kvlist) = new vector<NKVPair*>(); }
    break;

  case 31:
/* Line 1792 of yacc.c  */
#line 279 "parser.y"
    {(yyval.kvlist) = (yyvsp[(2) - (3)].kvlist);}
    break;

  case 32:
/* Line 1792 of yacc.c  */
#line 283 "parser.y"
    {(yyval.kvlist) = new vector<NKVPair*>(); (yyloc).from_blank_rule = true;}
    break;

  case 33:
/* Line 1792 of yacc.c  */
#line 285 "parser.y"
    {(yyvsp[(1) - (3)].kvlist)->push_back((yyvsp[(2) - (3)].kvpair));}
    break;

  case 35:
/* Line 1792 of yacc.c  */
#line 290 "parser.y"
    {(yyval.proto_prop_list) = new vector<NProtocolProperty*>();(yyloc).from_blank_rule = true;}
    break;

  case 36:
/* Line 1792 of yacc.c  */
#line 292 "parser.y"
    {(yyvsp[(1) - (2)].proto_prop_list)->push_back((yyvsp[(2) - (2)].protocolProperty));}
    break;

  case 38:
/* Line 1792 of yacc.c  */
#line 297 "parser.y"
    { (yyval.protocolReference) = new NProtocolReference(); (yyval.protocolReference)->name = (yyvsp[(2) - (3)].string);}
    break;

  case 39:
/* Line 1792 of yacc.c  */
#line 300 "parser.y"
    { (yyval.proto_ref_list) = new vector<NProtocolReference*>(); (yyloc).from_blank_rule = true;}
    break;

  case 40:
/* Line 1792 of yacc.c  */
#line 302 "parser.y"
    { (yyvsp[(1) - (2)].proto_ref_list)->push_back((yyvsp[(2) - (2)].protocolReference));}
    break;

  case 42:
/* Line 1792 of yacc.c  */
#line 308 "parser.y"
    { (yyval.field) = new NField();
                      (yyval.field)->attributes = (yyvsp[(1) - (5)].kvlist);
                      (yyval.field)->modifiers = (yyvsp[(2) - (5)].modifierList);
                      (yyval.field)->fieldType = (yyvsp[(3) - (5)].fieldType);
                      (yyval.field)->name = (yyvsp[(4) - (5)].string);
                      (yyvsp[(3) - (5)].fieldType)->field = (yyval.field);
                    }
    break;

  case 43:
/* Line 1792 of yacc.c  */
#line 317 "parser.y"
    { (yyval.kvlist) = new vector<NKVPair*>(); (yyval.kvlist)->push_back((yyvsp[(1) - (1)].kvpair)); }
    break;

  case 44:
/* Line 1792 of yacc.c  */
#line 319 "parser.y"
    { (yyvsp[(1) - (3)].kvlist)->push_back((yyvsp[(3) - (3)].kvpair)); }
    break;

  case 45:
/* Line 1792 of yacc.c  */
#line 324 "parser.y"
    { (yyvsp[(1) - (3)].kvpair)->value->append(*(yyvsp[(3) - (3)].string)); delete (yyvsp[(3) - (3)].string); }
    break;

  case 46:
/* Line 1792 of yacc.c  */
#line 326 "parser.y"
    { (yyval.kvpair) = new NKVPair(); (yyval.kvpair)->key = (yyvsp[(1) - (3)].string); (yyval.kvpair)->value = (yyvsp[(3) - (3)].string); }
    break;

  case 47:
/* Line 1792 of yacc.c  */
#line 328 "parser.y"
    { (yyval.kvpair) = new NKVPair(); (yyval.kvpair)->key = (yyvsp[(1) - (1)].string); (yyval.kvpair)->value = new std::string(""); }
    break;

  case 48:
/* Line 1792 of yacc.c  */
#line 333 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_SYN; }
    break;

  case 49:
/* Line 1792 of yacc.c  */
#line 335 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_ASYN;}
    break;

  case 50:
/* Line 1792 of yacc.c  */
#line 337 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_HTTP;}
    break;

  case 51:
/* Line 1792 of yacc.c  */
#line 339 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_STRUCT_REQUEST; (yyval.protocolProperty)->data = (yyvsp[(3) - (4)].string);}
    break;

  case 52:
/* Line 1792 of yacc.c  */
#line 341 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_STREAM_REQUEST;}
    break;

  case 53:
/* Line 1792 of yacc.c  */
#line 343 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_VOID_REQUEST;}
    break;

  case 54:
/* Line 1792 of yacc.c  */
#line 345 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_STRUCT_RESPONSE; (yyval.protocolProperty)->data = (yyvsp[(3) - (4)].string);}
    break;

  case 55:
/* Line 1792 of yacc.c  */
#line 347 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_STREAM_RESPONSE;}
    break;

  case 56:
/* Line 1792 of yacc.c  */
#line 349 "parser.y"
    { (yyval.protocolProperty) = new NProtocolProperty(); (yyval.protocolProperty)->propertyType = PT_VOID_RESPONSE;}
    break;

  case 57:
/* Line 1792 of yacc.c  */
#line 353 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 58:
/* Line 1792 of yacc.c  */
#line 357 "parser.y"
    { (yyval.modifierList) = new vector<int>(); (yyloc).from_blank_rule = true;}
    break;

  case 59:
/* Line 1792 of yacc.c  */
#line 359 "parser.y"
    { (yyvsp[(1) - (2)].modifierList)->push_back((yyvsp[(2) - (2)].token)); }
    break;

  case 60:
/* Line 1792 of yacc.c  */
#line 363 "parser.y"
    { (yyval.fieldType) = new NFieldType(); (yyval.fieldType)->fieldType = FT_ATOM; (yyval.fieldType)->atom_token = (yyvsp[(1) - (1)].token); }
    break;

  case 61:
/* Line 1792 of yacc.c  */
#line 364 "parser.y"
    { (yyval.fieldType) = new NFieldType(); (yyval.fieldType)->fieldType = FT_REFERENCE; (yyval.fieldType)->referencedTypeName = (yyvsp[(1) - (1)].string); }
    break;

  case 62:
/* Line 1792 of yacc.c  */
#line 366 "parser.y"
    { (yyval.fieldType) = new NFieldType();
                      (yyval.fieldType)->fieldType = FT_ARRAY;
                      (yyval.fieldType)->arrayInfo.arrayElement = (yyvsp[(1) - (4)].fieldType);
                      (yyval.fieldType)->arrayInfo.array_dimension_list = (yyvsp[(3) - (4)].stringList); 
                      (yyvsp[(1) - (4)].fieldType)->field = NULL;}
    break;

  case 63:
/* Line 1792 of yacc.c  */
#line 372 "parser.y"
    { (yyval.fieldType) = new NFieldType();
                      (yyval.fieldType)->fieldType = FT_ARRAY;
                      (yyval.fieldType)->arrayInfo.arrayElement = (yyvsp[(3) - (7)].fieldType);
                      (yyval.fieldType)->arrayInfo.array_dimension_list = (yyvsp[(6) - (7)].stringList); 
                      (yyvsp[(3) - (7)].fieldType)->field = NULL;}
    break;

  case 64:
/* Line 1792 of yacc.c  */
#line 378 "parser.y"
    { (yyval.fieldType) = new NFieldType();
                      (yyval.fieldType)->listElementType = (yyvsp[(3) - (4)].fieldType);
                      (yyval.fieldType)->fieldType = FT_LIST; 
                      (yyvsp[(3) - (4)].fieldType)->field = NULL; }
    break;

  case 65:
/* Line 1792 of yacc.c  */
#line 384 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 66:
/* Line 1792 of yacc.c  */
#line 385 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 67:
/* Line 1792 of yacc.c  */
#line 386 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 68:
/* Line 1792 of yacc.c  */
#line 387 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 69:
/* Line 1792 of yacc.c  */
#line 388 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 70:
/* Line 1792 of yacc.c  */
#line 389 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 71:
/* Line 1792 of yacc.c  */
#line 390 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 72:
/* Line 1792 of yacc.c  */
#line 391 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 73:
/* Line 1792 of yacc.c  */
#line 392 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 74:
/* Line 1792 of yacc.c  */
#line 393 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 75:
/* Line 1792 of yacc.c  */
#line 394 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 76:
/* Line 1792 of yacc.c  */
#line 395 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 77:
/* Line 1792 of yacc.c  */
#line 396 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 78:
/* Line 1792 of yacc.c  */
#line 397 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 79:
/* Line 1792 of yacc.c  */
#line 398 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 80:
/* Line 1792 of yacc.c  */
#line 399 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 81:
/* Line 1792 of yacc.c  */
#line 400 "parser.y"
    { (yyval.token) = (yyvsp[(1) - (1)].token); }
    break;

  case 82:
/* Line 1792 of yacc.c  */
#line 404 "parser.y"
    { (yyval.stringList) = new std::vector<std::string*>(); 
                      (yyval.stringList)->push_back((yyvsp[(1) - (1)].string)); }
    break;

  case 83:
/* Line 1792 of yacc.c  */
#line 407 "parser.y"
    { (yyvsp[(1) - (3)].stringList)->push_back((yyvsp[(3) - (3)].string)); }
    break;

  case 84:
/* Line 1792 of yacc.c  */
#line 411 "parser.y"
    {(yyval.enumEntryList) = new vector<NEnumEntry*>(); (yyloc).from_blank_rule = true;}
    break;

  case 85:
/* Line 1792 of yacc.c  */
#line 413 "parser.y"
    {(yyvsp[(1) - (2)].enumEntryList)->push_back((yyvsp[(2) - (2)].enumEntry));}
    break;

  case 87:
/* Line 1792 of yacc.c  */
#line 419 "parser.y"
    {(yyval.enumEntry) = new NEnumEntry(); (yyval.enumEntry)->name = (yyvsp[(1) - (4)].string); 
                     (yyval.enumEntry)->value = (yyvsp[(3) - (4)].integer); (yyval.enumEntry)->value_assigned = true;}
    break;

  case 88:
/* Line 1792 of yacc.c  */
#line 422 "parser.y"
    {(yyval.enumEntry) = new NEnumEntry(); (yyval.enumEntry)->name = (yyvsp[(1) - (2)].string); 
                     (yyval.enumEntry)->value = 0; (yyval.enumEntry)->value_assigned = false;}
    break;

  case 89:
/* Line 1792 of yacc.c  */
#line 427 "parser.y"
    {(yyval.enumEntry) = new NEnumEntry(); (yyval.enumEntry)->name = (yyvsp[(1) - (3)].string); 
                     (yyval.enumEntry)->value = (yyvsp[(3) - (3)].integer); (yyval.enumEntry)->value_assigned = true;}
    break;

  case 90:
/* Line 1792 of yacc.c  */
#line 430 "parser.y"
    {(yyval.enumEntry) = new NEnumEntry(); (yyval.enumEntry)->name = (yyvsp[(1) - (1)].string); 
                     (yyval.enumEntry)->value = 0; (yyval.enumEntry)->value_assigned = false;}
    break;

  case 91:
/* Line 1792 of yacc.c  */
#line 436 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 92:
/* Line 1792 of yacc.c  */
#line 437 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 93:
/* Line 1792 of yacc.c  */
#line 438 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 94:
/* Line 1792 of yacc.c  */
#line 441 "parser.y"
    {error((yylsp[(1) - (1)]), "struct: expecting an identifier.");}
    break;

  case 95:
/* Line 1792 of yacc.c  */
#line 442 "parser.y"
    {error((yylsp[(1) - (1)]), "cell: expecting an identifier.");}
    break;

  case 96:
/* Line 1792 of yacc.c  */
#line 443 "parser.y"
    {error((yylsp[(1) - (1)]), "cell: expecting keyword \"struct\".");}
    break;

  case 97:
/* Line 1792 of yacc.c  */
#line 444 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: expecting an identifier.");}
    break;

  case 98:
/* Line 1792 of yacc.c  */
#line 445 "parser.y"
    {error((yylsp[(1) - (1)]), "server: expecting an identifier.");}
    break;

  case 99:
/* Line 1792 of yacc.c  */
#line 446 "parser.y"
    {error((yylsp[(1) - (1)]), "module: expecting an identifier.");}
    break;

  case 100:
/* Line 1792 of yacc.c  */
#line 447 "parser.y"
    {error((yylsp[(1) - (1)]), "proxy: expecting an identifier.");}
    break;

  case 101:
/* Line 1792 of yacc.c  */
#line 448 "parser.y"
    {error((yylsp[(1) - (1)]), "enum: expecting an identifier.");}
    break;

  case 102:
/* Line 1792 of yacc.c  */
#line 451 "parser.y"
    {(yyval.token) = (yyvsp[(2) - (3)].token);}
    break;

  case 103:
/* Line 1792 of yacc.c  */
#line 452 "parser.y"
    {(yyval.token) = (yyvsp[(2) - (4)].token);}
    break;

  case 104:
/* Line 1792 of yacc.c  */
#line 453 "parser.y"
    {(yyval.token) = (yyvsp[(2) - (3)].token);}
    break;

  case 110:
/* Line 1792 of yacc.c  */
#line 461 "parser.y"
    {error((yylsp[(1) - (1)]), "field: type not specified.");}
    break;

  case 111:
/* Line 1792 of yacc.c  */
#line 462 "parser.y"
    {error((yylsp[(1) - (1)]), "field: expecting an identifier.");}
    break;

  case 112:
/* Line 1792 of yacc.c  */
#line 463 "parser.y"
    {error((yylsp[(1) - (1)]), "field: too many identifiers.");}
    break;

  case 113:
/* Line 1792 of yacc.c  */
#line 464 "parser.y"
    {error((yylsp[(1) - (1)]), "field: syntax error.");}
    break;

  case 114:
/* Line 1792 of yacc.c  */
#line 468 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 115:
/* Line 1792 of yacc.c  */
#line 469 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 116:
/* Line 1792 of yacc.c  */
#line 470 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 117:
/* Line 1792 of yacc.c  */
#line 473 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol references: specify a protocol.");}
    break;

  case 118:
/* Line 1792 of yacc.c  */
#line 475 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol references: expecting a \"protocol\" keyword.");}
    break;

  case 119:
/* Line 1792 of yacc.c  */
#line 476 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol references: too many identifiers.");}
    break;

  case 123:
/* Line 1792 of yacc.c  */
#line 484 "parser.y"
    {error((yylsp[(1) - (1)]), "settings: specify settings with format \"key:value;\".");}
    break;

  case 124:
/* Line 1792 of yacc.c  */
#line 488 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: expecting keyword \"type\".");}
    break;

  case 125:
/* Line 1792 of yacc.c  */
#line 489 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: expecting type specifier (syn/asyn).");}
    break;

  case 126:
/* Line 1792 of yacc.c  */
#line 490 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: expecting message type.");}
    break;

  case 127:
/* Line 1792 of yacc.c  */
#line 491 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: too many message type specifiers.");}
    break;

  case 128:
/* Line 1792 of yacc.c  */
#line 492 "parser.y"
    {error((yylsp[(1) - (1)]), "protocol: syntax error.");}
    break;

  case 129:
/* Line 1792 of yacc.c  */
#line 495 "parser.y"
    {(yyval.token)=0;}
    break;

  case 130:
/* Line 1792 of yacc.c  */
#line 496 "parser.y"
    {(yyval.token)=0;}
    break;

  case 131:
/* Line 1792 of yacc.c  */
#line 497 "parser.y"
    {(yyval.token)=0;}
    break;

  case 139:
/* Line 1792 of yacc.c  */
#line 509 "parser.y"
    {(yyval.token) = 0;}
    break;

  case 140:
/* Line 1792 of yacc.c  */
#line 514 "parser.y"
    {error((yylsp[(1) - (1)]), "enum: epxecting an identifier.");}
    break;

  case 141:
/* Line 1792 of yacc.c  */
#line 515 "parser.y"
    {error((yylsp[(1) - (1)]), "enum: invalid value.");}
    break;

  case 142:
/* Line 1792 of yacc.c  */
#line 516 "parser.y"
    {error((yylsp[(1) - (1)]), "enum: expecting a value, or ','.");}
    break;

  case 143:
/* Line 1792 of yacc.c  */
#line 518 "parser.y"
    {(yyval.token)=0;}
    break;

  case 144:
/* Line 1792 of yacc.c  */
#line 519 "parser.y"
    {(yyval.token)=0;}
    break;

  case 145:
/* Line 1792 of yacc.c  */
#line 520 "parser.y"
    {(yyval.token)=0;}
    break;


/* Line 1792 of yacc.c  */
#line 2647 "parser.tab.cpp"
      default: break;
    }
  /* User semantic actions sometimes alter yychar, and that requires
     that yytoken be updated with the new translation.  We take the
     approach of translating immediately before every use of yytoken.
     One alternative is translating here after every semantic action,
     but that translation would be missed if the semantic action invokes
     YYABORT, YYACCEPT, or YYERROR immediately after altering yychar or
     if it invokes YYBACKUP.  In the case of YYABORT or YYACCEPT, an
     incorrect destructor might then be invoked immediately.  In the
     case of YYERROR or YYBACKUP, subsequent parser actions might lead
     to an incorrect destructor call or verbose syntax error message
     before the lookahead is translated.  */
  YY_SYMBOL_PRINT ("-> $$ =", yyr1[yyn], &yyval, &yyloc);

  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);

  *++yyvsp = yyval;
  *++yylsp = yyloc;

  /* Now `shift' the result of the reduction.  Determine what state
     that goes to, based on the state we popped back to and the rule
     number reduced by.  */

  yyn = yyr1[yyn];

  yystate = yypgoto[yyn - YYNTOKENS] + *yyssp;
  if (0 <= yystate && yystate <= YYLAST && yycheck[yystate] == *yyssp)
    yystate = yytable[yystate];
  else
    yystate = yydefgoto[yyn - YYNTOKENS];

  goto yynewstate;


/*------------------------------------.
| yyerrlab -- here on detecting error |
`------------------------------------*/
yyerrlab:
  /* Make sure we have latest lookahead translation.  See comments at
     user semantic actions for why this is necessary.  */
  yytoken = yychar == YYEMPTY ? YYEMPTY : YYTRANSLATE (yychar);

  /* If not already recovering from an error, report this error.  */
  if (!yyerrstatus)
    {
      ++yynerrs;
#if ! YYERROR_VERBOSE
      yyerror (YY_("syntax error"));
#else
# define YYSYNTAX_ERROR yysyntax_error (&yymsg_alloc, &yymsg, \
                                        yyssp, yytoken)
      {
        char const *yymsgp = YY_("syntax error");
        int yysyntax_error_status;
        yysyntax_error_status = YYSYNTAX_ERROR;
        if (yysyntax_error_status == 0)
          yymsgp = yymsg;
        else if (yysyntax_error_status == 1)
          {
            if (yymsg != yymsgbuf)
              YYSTACK_FREE (yymsg);
            yymsg = (char *) YYSTACK_ALLOC (yymsg_alloc);
            if (!yymsg)
              {
                yymsg = yymsgbuf;
                yymsg_alloc = sizeof yymsgbuf;
                yysyntax_error_status = 2;
              }
            else
              {
                yysyntax_error_status = YYSYNTAX_ERROR;
                yymsgp = yymsg;
              }
          }
        yyerror (yymsgp);
        if (yysyntax_error_status == 2)
          goto yyexhaustedlab;
      }
# undef YYSYNTAX_ERROR
#endif
    }

  yyerror_range[1] = yylloc;

  if (yyerrstatus == 3)
    {
      /* If just tried and failed to reuse lookahead token after an
	 error, discard it.  */

      if (yychar <= YYEOF)
	{
	  /* Return failure if at end of input.  */
	  if (yychar == YYEOF)
	    YYABORT;
	}
      else
	{
	  yydestruct ("Error: discarding",
		      yytoken, &yylval, &yylloc);
	  yychar = YYEMPTY;
	}
    }

  /* Else will try to reuse lookahead token after shifting the error
     token.  */
  goto yyerrlab1;


/*---------------------------------------------------.
| yyerrorlab -- error raised explicitly by YYERROR.  |
`---------------------------------------------------*/
yyerrorlab:

  /* Pacify compilers like GCC when the user code never invokes
     YYERROR and the label yyerrorlab therefore never appears in user
     code.  */
  if (/*CONSTCOND*/ 0)
     goto yyerrorlab;

  yyerror_range[1] = yylsp[1-yylen];
  /* Do not reclaim the symbols of the rule which action triggered
     this YYERROR.  */
  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);
  yystate = *yyssp;
  goto yyerrlab1;


/*-------------------------------------------------------------.
| yyerrlab1 -- common code for both syntax error and YYERROR.  |
`-------------------------------------------------------------*/
yyerrlab1:
  yyerrstatus = 3;	/* Each real token shifted decrements this.  */

  for (;;)
    {
      yyn = yypact[yystate];
      if (!yypact_value_is_default (yyn))
	{
	  yyn += YYTERROR;
	  if (0 <= yyn && yyn <= YYLAST && yycheck[yyn] == YYTERROR)
	    {
	      yyn = yytable[yyn];
	      if (0 < yyn)
		break;
	    }
	}

      /* Pop the current state because it cannot handle the error token.  */
      if (yyssp == yyss)
	YYABORT;

      yyerror_range[1] = *yylsp;
      yydestruct ("Error: popping",
		  yystos[yystate], yyvsp, yylsp);
      YYPOPSTACK (1);
      yystate = *yyssp;
      YY_STACK_PRINT (yyss, yyssp);
    }

  YY_IGNORE_MAYBE_UNINITIALIZED_BEGIN
  *++yyvsp = yylval;
  YY_IGNORE_MAYBE_UNINITIALIZED_END

  yyerror_range[2] = yylloc;
  /* Using YYLLOC is tempting, but would change the location of
     the lookahead.  YYLOC is available though.  */
  YYLLOC_DEFAULT (yyloc, yyerror_range, 2);
  *++yylsp = yyloc;

  /* Shift the error token.  */
  YY_SYMBOL_PRINT ("Shifting", yystos[yyn], yyvsp, yylsp);

  yystate = yyn;
  goto yynewstate;


/*-------------------------------------.
| yyacceptlab -- YYACCEPT comes here.  |
`-------------------------------------*/
yyacceptlab:
  yyresult = 0;
  goto yyreturn;

/*-----------------------------------.
| yyabortlab -- YYABORT comes here.  |
`-----------------------------------*/
yyabortlab:
  yyresult = 1;
  goto yyreturn;

#if !defined yyoverflow || YYERROR_VERBOSE
/*-------------------------------------------------.
| yyexhaustedlab -- memory exhaustion comes here.  |
`-------------------------------------------------*/
yyexhaustedlab:
  yyerror (YY_("memory exhausted"));
  yyresult = 2;
  /* Fall through.  */
#endif

yyreturn:
  if (yychar != YYEMPTY)
    {
      /* Make sure we have latest lookahead translation.  See comments at
         user semantic actions for why this is necessary.  */
      yytoken = YYTRANSLATE (yychar);
      yydestruct ("Cleanup: discarding lookahead",
                  yytoken, &yylval, &yylloc);
    }
  /* Do not reclaim the symbols of the rule which action triggered
     this YYABORT or YYACCEPT.  */
  YYPOPSTACK (yylen);
  YY_STACK_PRINT (yyss, yyssp);
  while (yyssp != yyss)
    {
      yydestruct ("Cleanup: popping",
		  yystos[*yyssp], yyvsp, yylsp);
      YYPOPSTACK (1);
    }
#ifndef yyoverflow
  if (yyss != yyssa)
    YYSTACK_FREE (yyss);
#endif
#if YYERROR_VERBOSE
  if (yymsg != yymsgbuf)
    YYSTACK_FREE (yymsg);
#endif
  /* Make sure YYID is used.  */
  return YYID (yyresult);
}


