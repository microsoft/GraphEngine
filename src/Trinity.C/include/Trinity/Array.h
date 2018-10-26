// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstring>
#include <vector>
#include <functional>
namespace Trinity
{
    template<typename T> class Array
    {
        T* _array;
        std::allocator<T> _allocator;
        size_t _count;
    public:
        typedef Array<T> _Myt;

        Array() : _array(NULL), _count(0) {}

        Array(size_t count)
        {
            allocate(count); 
            initialize();
        }

        Array(size_t count, std::function<T&&(int)> generator)
        {
            allocate(count);
            int idx = 0;
            for (T *p = _array, *e = _array + _count; p != e; ++p)
            {
                _allocator.construct(p, generator(idx++));
            }
        }

        Array(const _Myt& arr)
        {
            _copy_from(arr._array, arr._count);
        }

        template <class _VecAlloc> Array(const std::vector<T, _VecAlloc> & vec)
        {
            _copy_from(vec.data(), vec.size());
        }

        Array(std::initializer_list<T> il)
        {
            _copy_from(il.begin(), il.size());
        }

        Array(_Myt&& arr) 
        { 
            _move_from(std::forward<_Myt>(arr)); 
        }

        ~Array()
        {
            destroy();
            deallocate();
        }

        Array& operator = (const _Myt &  arr) 
        {
            destroy();
            deallocate(); 
            _copy_from(arr._array, arr._count); 
            return *this; 
        }

        template <class _VecAlloc> Array& operator = (const std::vector<T, _VecAlloc> &  vec) 
        { 
            destroy();
            deallocate();
            _copy_from(vec.data(), vec.size());
            return *this;
        }

        Array& operator = (std::initializer_list<T>  il) 
        { 
            destroy();
            deallocate();
            _copy_from(il.begin(), il.size()); 
            return *this; 
        }

        Array& operator = (_Myt && arr) 
        {
            destroy();
            deallocate(); 
            _move_from(std::forward<_Myt>(arr));
            return *this; 
        }

        std::vector<T> ToList() const { std::vector<T> ret; ret.insert(ret.begin(), this->begin(), this->end()); return ret; }
        std::vector<T> ToList() { std::vector<T> ret; ret.insert(ret.begin(), this->begin(), this->end()); return ret; }

        inline size_t Length() const { return _count; }

        //element access
        //T& operator [](size_t pos) { return _array[pos]; }
        //const T& operator [](size_t pos) const { return _array[pos]; }
        T* data() { return _array; }
        const T* data() const{ return _array; }
        operator T*() { return data(); }
        operator const T*() const{ return data(); }
        T&& move(size_t pos) { return std::move(_array[pos]); }
        //!  Since our array buffer is produced with an allocator,
        //   it does not make sense to pass out the raw pointer
        //   as the allocator information would be lost.
        //   As our primary usage is interop, it doesn't make sense
        //   either to return std::unique_ptr.
        //   Therefore the best thing to do is to malloc a new buffer, and move the elements
        //   into the new buffer. 
        T* detach_data() 
        { 
            auto len = _count * sizeof(T);
            auto ret = (T*)malloc(len);

            std::move(_array, _array + _count, ret);

            //! deallocate, not destroy
            deallocate();
            return ret; 
        }

        //since we're just simply arrays, we don't care much about the logic of iterator..
        typedef T *iterator;
        typedef const T *const_iterator;

        iterator begin() { return _array; }
        iterator end() { return _array + _count; }
        const_iterator begin() const { return _array; }
        const_iterator end() const { return _array + _count; }

        const_iterator cbegin() { return _array; }
        const_iterator cend() { return _array + _count; }


    private:

        inline void allocate(size_t count) 
        { 
            _array = _allocator.allocate(count); 
            _count = count;
        }

        inline void deallocate() 
        { 
            _allocator.deallocate(_array, _count); 
            _array = nullptr; 
            _count = 0; 
        }

        inline void initialize()
        {
            if constexpr(std::is_trivial<T>::value)
                memset(_array, 0, sizeof(T) * _count);
            else
            {
                /*forcibly call the constructor*/
                for (T *p = _array, *e = _array + _count; p != e; ++p)
                    _allocator.construct(p);
            }
        }

        inline void destroy()
        {
            if constexpr ((std::is_trivial<T>::value || std::is_trivially_destructible<T>::value))
            {
                /*do nothing*/
            }
            else
            {
                /*forcibly call destructor*/
                for (T *p = _array, *e = _array + _count; p != e; ++p)
                {
                    _allocator.destroy(p);
                }
            }
        }

        inline void _move_from(_Myt&& arr)
        {
            _count = arr._count;
            _array = arr._array;
            arr._array = nullptr;
            arr._count = 0;
        }

        inline void _copy_from(const T* source, size_t cnt)
        {
            /* constructing from another array means deep copy */
            allocate(cnt);
            if constexpr(std::is_trivially_copyable<T>::value)
            {
                memcpy(_array, source, sizeof(T) * _count);
            }
            else
            {
                /* non trivial, have to construct one by one */
                T* target = _array;
                while (cnt--)
                {
                    /* forcibly call the copy constructor
                       (raises error when the underlying object is not copy-constructable) */
                    _allocator.construct(target++, *(source++));
                }
            }
        }
    };
}


