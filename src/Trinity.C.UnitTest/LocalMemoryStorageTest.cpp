#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include <cstdio>
#include <Trinity.h>
#include <Trinity/IO/Console.h>

using namespace Trinity;
using namespace Trinity::IO;

TEST_CASE("LocalMemoryStorage Disk I/O tests", "[io] [storage]")
{
    CInitialize();

    SECTION("Initialize - SaveCell - SaveStorage - LoadStorage sequence")
    {
        CInitialize();

        char content[15];

        for (int i=0; i < 4097; ++i)
            CSaveCell(i, content, 15, 255);

        CSaveStorage();
        CLoadStorage();

    }

    SECTION("Load storage")
    {
        CLoadStorage();
    }

    SECTION("Reset storage")
    {
        CResetStorage();
    }

    SECTION("CResetStorageAndSaveStorageTest")
    {
        CResetStorage();
        CSaveStorage();
    }

    SECTION("CRemoveCell_loggedTest")
    {
        CResetStorage();
        FILE* fp = fopen("log.dat", "wb");
        CSetWriteAheadLogFile(fp);

        char buf[10];

        CSaveCell(1, buf, 10, 0);

        CLoggedRemoveCell(1, CellAccessOptions::StrongLogAhead);


        fclose(fp);
    }
}
