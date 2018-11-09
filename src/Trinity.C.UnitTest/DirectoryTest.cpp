#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <Trinity.h>
#include <Trinity/IO/Path.h>
#include <Trinity/Environment.h>

using Trinity::String;

TEST_CASE("Trinity::IO::Directory::Exists works", "[io]")
{
    String p = Trinity::Environment::GetCurrentDirectoryW();
    String subdir = Trinity::IO::Path::Combine(p, "1", "2", "3", "4", "5");

    REQUIRE(true == Trinity::IO::Directory::Exists(p));
    REQUIRE(false == Trinity::IO::Directory::Exists(subdir));
}
