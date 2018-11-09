#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <Trinity.h>
#include <Trinity/String.h>
#include <Trinity/Array.h>
using Trinity::String;
using Trinity::Array;

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

    REQUIRE(L"" == x);
}

TEST_CASE("FromWCharArray works", "[string]")
{
    Array<u16char> x(0);
    REQUIRE(String("") == String::FromWcharArray(x));
}
