#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <Trinity.h>

TRINITY_API TrinityErrorCode  CInitialize();

TEST_CASE("Trinity::LocalMemoryStorage can be initialized correctly", "[storage]")
{
    CInitialize();
    REQUIRE(CCellCount() == 0);
}
