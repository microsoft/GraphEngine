#include <iostream>
#include <string>
#include "error.h"
#include "SyntaxNode.h"
#include "flex_bison_common.h" // for yylloc
#include <cstdarg>
#include <cstdio>
#include <Trinity/String.h>
using namespace std;

int error_count = 0;
bool error_disable_warnings = false;

//all other overloads will finally come down to this one
//put "Warning:" anywhere in the message to turn an error into a warning.
void error(string locationString, string msg)
{
    Trinity::String t_loc(locationString.c_str());
    Trinity::String t_msg(msg.c_str());
    if (locationString != "")
    {
        wprintf(L"%ls : ", t_loc.ToWcharArray().data());
    }

    if (t_msg.IndexOf("Warning:") != t_msg.npos)
    {
        if (!error_disable_warnings)
        {
            t_msg.Replace("Warning:", "");
            // XXX warning as info message
            wprintf(L"%ls\n", t_msg.ToWcharArray().data());
        }
    }
    else
    {
        wprintf(L"Error: %ls\n", t_msg.ToWcharArray().data());
        ++error_count;
    }
}

//===================== overloads and routing routines

void error(Node* node, string msg)
{
    node->error(msg);
}
void error(YYLTYPE loc, string msg)
{
    yyerror(loc, msg.c_str());
}
void error(string msg)
{
    yyerror(msg.c_str());
}
void error(const char* msg)
{
    yyerror(msg);
}

#define ERROR_BUFFER_SIZE 1024
char error_buffer[ERROR_BUFFER_SIZE];
char loc_buffer[ERROR_BUFFER_SIZE];

static void __snprintf(const char *buf, size_t max_size, char *s, ...)
{
    va_list vlist;
    va_start(vlist, s);
    vsnprintf_s((char*)buf, max_size, max_size, s, vlist);
    va_end(vlist);
}

static string YYLTYPE2str(YYLTYPE loc)
{
    if (loc.filename)
    {
        __snprintf(loc_buffer, sizeof(loc_buffer), "%s(%d,%d,%d,%d)",
                   loc.filename->c_str(),
                   loc.first_line,
                   loc.first_column,
                   loc.last_line,
                   loc.last_column);
        return string(loc_buffer);
    }
    return "";
}

//yyerror always call error
static void __yyerror_impl(YYLTYPE loc, const char *s, va_list args)
{
    vsnprintf_s(error_buffer, sizeof(error_buffer), sizeof(error_buffer), s, args);
    string loc_str = YYLTYPE2str(loc);
    error(loc_str, string(error_buffer));
}

void yyerror(YYLTYPE loc, const char* s, ...)
{
    va_list vlist;
    va_start(vlist, s);
    __yyerror_impl(loc, s, vlist);
    va_end(vlist);
}

//this overload use yylloc as the current location
void yyerror(const char *s, ...)
{
    va_list vlist;
    va_start(vlist, s);
    __yyerror_impl(yylloc, s, vlist);
    va_end(vlist);
}