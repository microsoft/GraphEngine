#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <cstdio>
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include "Storage/MTHash/MTHash.h"
#include <Trinity/IO/Directory.h>
#include <Trinity/IO/Path.h>

using namespace Trinity::IO;

TEST_CASE("LOG_RECORD_HEADER should be a POD with size 15", "[static]")
{
    REQUIRE(std::is_pod<Storage::LocalMemoryStorage::Logging::LOG_RECORD_HEADER>::value);
    REQUIRE(sizeof(Storage::LocalMemoryStorage::Logging::LOG_RECORD_HEADER) == 15);
}
