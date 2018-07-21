#include "GraphEngine.Hosting.h"
#include <cstdio>

int main(int argc, char** argv)
{
    void* lpenv;
    printf("GraphEngineInit result = %d\n",
           GraphEngineInit(0, nullptr,
                           L"D:\git\GraphEngine\src\GraphEngine\trinity_host_dummy\bin\Debug\netstandard2.0\trinity_host_dummy.dll",
                           L"Trinity.Hosting.Initializer",
                           L"Init",
                           lpenv));
    printf("GraphEngineUninit result = %d\n", GraphEngineUninit(lpenv));
    return 0;
}