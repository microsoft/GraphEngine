#pragma once
struct CellDescriptor
{
    char* Name;
    void* Handle;
};

struct FieldDescriptor
{
    char* Name;
    char* TypeName;
    void* Handle;
};