#include "GraphEngine.Hosting.h"
#include <cstdio>

int main(int argc, char** argv)
{
    void* lpenv;
    TrinityErrorCode(*func)();
    wchar_t* apppaths[] =
    {
       L"D:\\git\\GraphEngine\\src\\GraphEngine\\trinity_host_dummy\\bin\\Debug\\netstandard2.0\\"
    };
    printf("GraphEngineInit result = %d\n",
           GraphEngineInit(1, apppaths, lpenv));
    printf("GraphEngineGetFunction result = %d\n",
           GraphEngineGetFunction(lpenv,
                                  L"trinity_host_dummy",
                                  L"Trinity.Hosting.Initializer",
                                  L"Init",
                                  (void**)&func));

    printf("Function execution result = %d\n", 
           func());

    printf("GraphEngineUninit result = %d\n", 
           GraphEngineUninit(lpenv));
    return 0;
}