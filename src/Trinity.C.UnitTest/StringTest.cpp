#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <Trinity.h>
#include <Trinity/String.h>
#include <Trinity/Array.h>

using namespace Trinity;

TEST_CASE("Replace works", "[string]")
{
    String s, t;
    s.Replace("/", "\\");

    s = "aaafffddwwwfvbbb";
    t = s;
    t.Replace("a", "c");

    REQUIRE(String("cccfffddwwwfvbbb") == t);
}

TEST_CASE("ToWCharArray works", "[string]")
{
    String s = "123";
    s.Clear();
    auto x = s.ToWcharArray();
    auto p = L"";

    REQUIRE(0 == wcscmp(x, p));
}

TEST_CASE("FromWCharArray works", "[string]")
{
    Array<u16char> x(0);
    REQUIRE(String("") == String::FromWcharArray(x));
}
