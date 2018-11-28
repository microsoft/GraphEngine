#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include <cstdio>
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/MTHash/MTHash.h"
#include <Trinity/IO/Directory.h>
#include <Trinity/IO/Path.h>

using namespace Trinity;
using namespace Trinity::IO;

TEST_CASE("Trinity::IO::Path::Combine works", "[io]")
{
#if defined(TRINITY_PLATFORM_WINDOWS)
    REQUIRE("D:\\hello\\world\\yeah.cs" == Path::Combine(String("D:/hello"), String("world/"), String("yeah.cs")));
    REQUIRE("D:\\hello" == Path::Combine(String("D:/hello")));
    REQUIRE("D:\\hello\\Hey" == Path::Combine(String("D:/hello"), String("Hey")));
    REQUIRE("D:\\hello\\Hey\\You.cs\\There" == Path::Combine(String("D:/hello"), String("Hey"), String("You.cs"), String("There")));
    REQUIRE("D:\\hello\\Hey\\You.cs\\There" == Path::Combine(String("D:/hello"), String("/Hey\\"), String("You.cs"), String("There")));
    REQUIRE("" == Path::Combine());
#else
    REQUIRE("D:/hello/world/yeah.cs" == Path::Combine(String("D:/hello"), String("world\\"), String("yeah.cs")));
    REQUIRE("D:/hello" == Path::Combine(String("D:/hello")));
    REQUIRE("D:/hello/Hey" == Path::Combine(String("D:/hello"), String("Hey")));
    REQUIRE("D:/hello/Hey/You.cs/There" == Path::Combine(String("D:/hello"), String("Hey"), String("You.cs"), String("There")));
    REQUIRE("D:/hello/Hey/You.cs/There" == Path::Combine(String("D:/hello"), String("/Hey\\"), String("You.cs"), String("There")));
    REQUIRE("" == Path::Combine());
#endif

}

TEST_CASE("Trinity::IO::Path::GetDirectoryName works", "[io]")
{
#if defined(TRINITY_PLATFORM_WINDOWS)
    String p = "\\foo\\bar\\baz";
    p = Path::GetDirectoryName(p);
    REQUIRE("\\foo\\bar" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("\\foo" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("" == p);
#else
    String p = "/foo/bar/baz";
    p = Path::GetDirectoryName(p);
    REQUIRE("/foo/bar" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("/foo" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("/" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("" == p);
    p = Path::GetDirectoryName(p);
    REQUIRE("" == p);
#endif
}

TEST_CASE("Trinity::IO::Path::GetFullPath works", "[io]")
{
    REQUIRE(Path::GetFullPath("D:\\") == Path::GetFullPath("D:/"));
    REQUIRE(Path::GetFullPath("D:\\\\\\") == Path::GetFullPath("D:/\\/\\"));
    REQUIRE(Path::GetFullPath("D:\\a\\b\\") == String("D:\\a\\b\\"));

    auto my_path = Environment::GetCurrentDirectory();

    REQUIRE(Path::GetFullPath(".") == Path::GetFullPath(my_path));
    REQUIRE(Path::GetFullPath("..") == Path::GetFullPath(Path::Combine(Path::GetFullPath(my_path), "..")));
    REQUIRE(Path::GetFullPath("..") == Path::GetFullPath(Path::GetFullPath(my_path) + "/.."));
}

TEST_CASE("Trinity::IO::Path::CompletePath works", "[io]")
{
    String p      = Environment::GetCurrentDirectory();
    String subdir = Path::CompletePath(Path::Combine(p, "foo", "bar", "baz"), true);
    REQUIRE(Directory::Exists(subdir));
}
