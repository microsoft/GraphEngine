#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include <Trinity.h>
#include <Trinity/IO/Console.h>

TEST_CASE("Console::WriteLine does not crash the program", "[console]")
{
    Trinity::IO::Console::WriteLine("hello!");
}

TEST_CASE("Console::WriteLine properly output wide characters", "[console]")
{
    Trinity::IO::Console::WriteLine(L"你好!");
}
