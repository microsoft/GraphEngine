#include "GraphEngine.Hosting.h"
#include <cstdio>

int main(int argc, char** argv)
{
    void* lpenv;
    TrinityErrorCode(*func)(char*, char*);
    char* apppaths[] =
    {
       //"D:\\git\\GraphEngine\\src\\GraphEngine\\trinity_host_dummy\\bin\\Debug\\netstandard2.0\\"
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\GraphEngine.FFI.Metagen\\2.0.9328\\lib\\netstandard2.0",      
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\GraphEngine.Jit\\2.0.9328\\lib\\netstandard2.0",    
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\GraphEngine.Core\\2.0.9328\\lib\\netstandard2.0",       
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\GraphEngine.FFI\\2.0.9328\\lib\\netstandard2.0",      
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\Microsoft.Extensions.ObjectPool\\2.0.0\\lib\\netstandard2.0",       
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\GraphEngine.Storage.Composite\\2.0.9328\\lib\\netstandard2.0",      
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\FSharp.Core\\4.3.4\\lib\\netstandard1.6",       
	   "C:\\Users\\v-wazhao\\.nuget\\packages\\Newtonsoft.Json\\9.0.1\\lib\\netstandard1.0"
    };
    printf("GraphEngineInit result = %d\n",
           GraphEngineInit(sizeof(apppaths) / sizeof(char*), apppaths, lpenv));
    printf("GraphEngineGetFunction result = %d\n",
           GraphEngineGetFunction(lpenv,
                                  "Trinity.FFI",
                                  "Trinity.FFI.Initializer",
                                  "Initialize",
                                  (void**)&func));

    printf("Function execution result = %d\n", 
           func("D:\\trinity.xml", "D:\\storage"));

    printf("GraphEngineUninit result = %d\n", 
           GraphEngineUninit(lpenv));
    return 0;
}