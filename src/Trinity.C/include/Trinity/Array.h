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
        size_t _length;
    public:
        typedef Array<T> _Myt;

        Array() : _length(0), _array(NULL) {}
        Array(size_t length) : _length(length)
        {
            allocate(); initialize();
        }
        Array(size_t length, std::function<T&&(int)> generator)
            : _length(length)
        {
            allocate();
            int idx = 0;
            for (T *p = _array, *e = _array + _length; p != e; ++p)
            {
                _allocator.construct(p, generator(idx++));
            }
        }
        Array(const _Myt& arr)
        {
            _copy_from(arr._array, arr._length);
        }
        template <class _VecAlloc> Array(const std::vector<T, _VecAlloc> & vec)
        {
            _copy_from(vec.data(), vec.size());
        }
        Array(std::initializer_list<T> il)
        {
            _copy_from(il.begin(), il.size());
        }
        Array(_Myt&& arr) { _move_from(std::forward<_Myt>(arr)); }

        ~Array()
        {
            if (_array)
            {
                destroy();
                deallocate();
            }
        }

        Array& operator = (const _Myt &  arr) { deallocate(); _copy_from(arr._array, arr._length); return *this; }
        template <class _VecAlloc> Array& operator = (const std::vector<T, _VecAlloc> &  vec) { deallocate(); _copy_from(vec.data(), vec.size()); return *this; }
        Array& operator = (std::initializer_list<T>  il) { deallocate(); _copy_from(il.begin(), il.size()); return *this; }
        Array& operator = (_Myt && arr) { _move_from(std::forward<_Myt>(arr)); return *this; }

        std::vector<T> ToList() const { std::vector<T> ret; ret.insert(ret.begin(), this->begin(), this->end()); return ret; }
        std::vector<T> ToList() { std::vector<T> ret; ret.insert(ret.begin(), this->begin(), this->end()); return ret; }

        inline size_t Length() const { return _length; }

        //element access
        //T& operator [](size_t pos) { return _array[pos]; }
        //const T& operator [](size_t pos) const { return _array[pos]; }
        T* data() { return _array; }
        const T* data() const{ return _array; }
        operator T*() { return data(); }
        operator const T*() const{ return data(); }
        T&& move(size_t pos) { return std::move(_array[pos]); }
		T* detach_data() { auto ret = _array; _array = nullptr; _length = 0; return ret; }

        //since we're just simply arrays, we don't care much about the logic of iterator..
        typedef T *iterator;
        typedef const T *const_iterator;

        iterator begin() { return _array; }
        iterator end() { return _array + _length; }
        const_iterator begin() const { return _array; }
        const_iterator end() const { return _array + _length; }

        const_iterator cbegin() { return _array; }
        const_iterator cend() { return _array + _length; }


    private:

        inline void allocate() { _array = _allocator.allocate(_length); }
        inline void deallocate() { _allocator.deallocate(_array, _length); }

        inline void initialize()
        {
            if (std::is_trivial<T>::value)
                memset(_array, 0, sizeof(T) * _length);
            else/*forcibly call the constructor*/
                for (T *p = _array, *e = _array + _length; p != e; ++p)
                    _allocator.construct(p);
        }
        inline void destroy()
        {
            if ((std::is_trivial<T>::value || std::is_trivially_destructible<T>::value))
                /*do nothing*/;
            else
                for (T *p = _array, *e = _array + _length; p != e; ++p)
                    /*forcibly call destructor*/
                    _allocator.destroy(p);

            /* alternative implementation:
               std::_Destroy_range(_array, _array + _length, _allocator); */
        }

        inline void _move_from(_Myt&& arr)
        {
            _length = arr._length;
            _array = arr._array;
            arr._array = NULL;
        }

        inline void _copy_from(const T* source, size_t _len)
        {
            /* constructing from another array means deep copy */
            _length = _len;
            allocate();
            if (std::is_trivially_copyable<T>::value)
            {
                memcpy(_array, source, sizeof(T) * _length);
            }
            else
            {
                /* non trivial, have to construct one by one */
                T* target = _array;
                while (_len--)
                    /* forcibly call the copy constructor
                       (raises error when the underlying object is not copy-constructable) */
                       _allocator.construct(target++, *(source++));
            }
        }


    };
}


