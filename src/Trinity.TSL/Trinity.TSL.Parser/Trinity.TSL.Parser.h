#pragma once
#include <vector>
#include "parser.tab.h"

namespace Trinity
{
    class String;
}

class	NTSL;
extern  void        parser_disable_warnings();
extern	NTSL*		start_parser(const std::vector<Trinity::String>& files);
extern	void		reset_parser();

struct TokenInfo
{
    yytokentype tokenType;
    YYLTYPE tokenLocation;
};

class TokenList
{
public:
    TokenList(const char* buffer);
    std::vector<TokenInfo> tokens;
};
