#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include <cstdio>
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/MTHash/MTHash.h"

TEST_CASE("MemoryTrunk is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::MemoryTrunk) == 128);
}

TEST_CASE("MTHash is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::MTHash) == 64);
}
