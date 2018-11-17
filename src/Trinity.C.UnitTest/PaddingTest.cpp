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

TEST_CASE("AddressTableEntry is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::AddressTableEntry) == 8);
}

TEST_CASE("AddressTableEndPoint is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::AddressTableEndPoint) == 8);
}

TEST_CASE("HeadGroup is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::HeadGroup) == 8);
}

TEST_CASE("CellEntry is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(CellEntry) == 8);
}

TEST_CASE("MTEntry is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(MTEntry) == 16);
}

TEST_CASE("MTHashAllocationInfo is properly cache-aligned", "[memory]")
{
    REQUIRE(sizeof(Storage::MTHashAllocationInfo) == 16);
}
