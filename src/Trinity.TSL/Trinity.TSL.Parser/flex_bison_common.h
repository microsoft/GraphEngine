#pragma once
#include <string>

class NTSL;

/* Defined in Lex */
extern int              push_new_file(std::string *filename);
extern void				push_new_buffer(const char* buffer);
extern void             reset_filename_stack();
extern void				set_lex_nonstop_mode(bool);

extern int              yylex();
extern NTSL             *tsl;
extern int              yycolumnno;
extern int              yylineno;

extern FILE				*yyin;

/* Defined in Parser*/
extern int				yyparse();
#if YYDEBUG
extern int				yydebug;
#endif

typedef struct YYLTYPE {
    int first_line = 1;
    int first_column = 1;
    int last_line = 1;
    int last_column = 1;
    std::string *filename = NULL;
    bool from_blank_rule = false;

    ~YYLTYPE() {}
} YYLTYPE;

# define YYLLOC_DEFAULT(Current, Rhs, N)                                    \
do {                                                                        \
    if (YYID(N))                                                            \
    {                                                                       \
        int first_symbol = 1, last_symbol = N;                              \
        for (first_symbol = 1; first_symbol < N; ++first_symbol)            \
        {                                                                   \
            if ((YYRHSLOC(Rhs, first_symbol).from_blank_rule) == false)     \
                break;                                                      \
        }                                                                   \
        for (last_symbol = N; last_symbol > first_symbol; --last_symbol)    \
        {                                                                   \
            if ((YYRHSLOC(Rhs, last_symbol).from_blank_rule) == false)      \
                break;                                                      \
        }                                                                   \
        (Current).first_line = YYRHSLOC(Rhs, first_symbol).first_line;      \
        (Current).first_column = YYRHSLOC(Rhs, first_symbol).first_column;  \
        (Current).last_line = YYRHSLOC(Rhs, last_symbol).last_line;         \
        (Current).last_column = YYRHSLOC(Rhs, last_symbol).last_column;     \
        (Current).filename = YYRHSLOC(Rhs, first_symbol).filename;          \
        (Current).from_blank_rule = false;                                  \
    }                                                                       \
    else                                                                    \
    {                                                                       \
        (Current).first_line = (Current).last_line =                        \
            YYRHSLOC(Rhs, 0).last_line;                                     \
        (Current).first_column = (Current).last_column =                    \
            YYRHSLOC(Rhs, 0).last_column;                                   \
        (Current).filename = NULL;                                          \
        (Current).from_blank_rule = true;                                   \
    }                                                                       \
    lhs_loc = (Current);                                                    \
} while (YYID(0))

extern YYLTYPE			yylloc;
extern YYLTYPE			lhs_loc;

#define YYLTYPE_IS_DECLARED
#undef YYLTYPE_IS_TRIVIAL
