#pragma once
#include <string>
using namespace std;

class Node;
struct YYLTYPE;

extern int error_count;
extern bool error_disable_warnings;

//all other overloads will be finally routed to this one
void error(std::string locationString, std::string msg);

//reports an error message with location range covering the whole Node
void error(Node*, std::string msg);
//reports an error message on the given location range
void error(YYLTYPE, std::string msg);
//reports an error message on current yylloc
void error(std::string msg);


// flex-bison related facilities
void yyerror(const char *s, ...);
void yyerror(YYLTYPE, const char *s, ...);