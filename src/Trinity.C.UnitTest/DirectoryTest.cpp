#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include <Trinity.h>
#include <Trinity/IO/Path.h>
#include <Trinity/Environment.h>

using namespace Trinity;

TEST_CASE("Trinity::IO::Directory::Exists works", "[io]")
{
    String p = Environment::GetCurrentDirectoryW();
    String subdir = IO::Path::Combine(p, "1", "2", "3", "4", "5");

    REQUIRE(true == IO::Directory::Exists(p));
    REQUIRE(false == IO::Directory::Exists(subdir));
}
