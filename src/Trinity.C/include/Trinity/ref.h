// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "NonReferencable.h"
#include "NonCopyable.h"
#include <atomic>
using std::atomic_int;
namespace Trinity
{
    //Trait needed for referenced types
    template<typename T>struct ReferencedTypeTrait
    {
    public:
        static bool isCopyable() { return true; }
        static bool isReferencable() { return true; }
    };
    template<> inline bool ReferencedTypeTrait<NonCopyable>::isCopyable() { return false; }
    template<> inline bool ReferencedTypeTrait<NonReferencable>::isReferencable() { return false; }

    /*
    ReferencePointer provides safe usage of pointers at the cost of synchronization
    */
    template<typename T>class ReferencePointer : NonReferencable
    {
    public:
        ReferencePointer(T* const rawPointer){ _ptr = rawPointer;  _ref_count = new atomic_int();  _ref_count->store(1, std::memory_order_release); }
        ReferencePointer(const ReferencePointer<T>& reference) { _ptr = reference._ptr;  _ref_count = reference._ref_count; _ref_count->fetch_add(1, std::memory_order_seq_cst); }
        ReferencePointer(ReferencePointer<T>&& rval){ _ptr = rval._ptr; _ref_count = rval._ref_count; rval._ref_count = NULL; }

        ReferencePointer& operator = (const T*){}
        ReferencePointer& operator = (const ReferencePointer&);
        ReferencePointer& operator = (ReferencePointer<T>&&);

        T* operator->() { return _ptr; }
        T& operator*() { return *_ptr; }
        T* Pointer() { return _ptr; }
        //operator T*() { return _ptr; }

        ~ReferencePointer()
        {
            if (_ref_count && (1 == _ref_count->fetch_sub(1, std::memory_order_seq_cst)))
            {
                //TODO lazy deletion
                delete _ptr;
            }
        }
    private:
        T* _ptr;
        atomic_int *_ref_count;
    };

    template<typename T> inline ReferencePointer<T> ref(T* const rawPointer)
    {
        return ReferencePointer<T>(rawPointer);
    }
    template<typename T, typename ...Args> inline ReferencePointer<T> refnew(Args... arguments)
    {
        return ReferencePointer<T>(new T(arguments...));
    }
}