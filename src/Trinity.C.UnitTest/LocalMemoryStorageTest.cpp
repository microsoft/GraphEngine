#define CATCH_CONFIG_MAIN
#include "catch.hpp"
#include <cstdio>
#include <Trinity.h>

using Trinity::String;

TEST_CASE("LocalMemoryStorage Disk I/O tests", "[io] [storage]")
{
    String storage_root = ".\\storage\\";
    CSetStorageRoot(storage_root);
    CInitialize();

    SECTION("Initialize - SaveCell - SaveStorage - LoadStorage sequence")
    {
        String storage_root = ".\\storage\\";
        TrinityConfig::SetStorageRoot(storage_root);
        Console::WriteLine("StorageRoot: {0}", TrinityConfig::StorageRoot());
        Storage::LocalMemoryStorage::Initialize();

        char content[15];

        for (int i=0; i < 4097; ++i)
            Storage::LocalMemoryStorage::SaveCell(i, content, 15, 255);

        Storage::LocalMemoryStorage::SaveStorage();
        Storage::LocalMemoryStorage::LoadStorage();

    }

    SECTION("Load storage")
    {
        Storage::LocalMemoryStorage::LoadStorage();
    }

    SECTION("Reset storage")
    {
        Storage::LocalMemoryStorage::ResetStorage();
    }

    SECTION("CResetStorageAndSaveStorageTest")
    {
        Storage::LocalMemoryStorage::ResetStorage();
        Storage::LocalMemoryStorage::SaveStorage();
    }

    SECTION("CRemoveCell_loggedTest")
    {
        Storage::LocalMemoryStorage::ResetStorage();
        FILE* fp = fopen("log.dat", "wb");
        Storage::LocalMemoryStorage::Logging::SetWriteAheadLogFile(fp);

        char buf[10];

        Storage::LocalMemoryStorage::SaveCell(1, buf, 10, 0);

        Storage::LocalMemoryStorage::RemoveCell(1, Storage::LocalMemoryStorage::CellAccessOptions::StrongLogAhead);


        fclose(fp);
    }
}
