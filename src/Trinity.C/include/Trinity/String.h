// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include <string>
#include <cstring>
#include <initializer_list>
#include <istream>
#include <ostream>
#include <sstream>
#include <os/os.h>
#include "Array.h"
#include "Collections/List.h"

#ifndef __cplusplus_cli
#include <thread>
#endif

#if defined(TRINITY_PLATFORM_WINDOWS)
typedef wchar_t u16char;
#define _u(x) (L##x)
#else
#include <codecvt>
#include <locale>
typedef char16_t u16char;
#define _u(x) (u##x)
#endif

using namespace Trinity::Collections;

namespace Trinity
{
    /*
        String is a wrapper around _StringType.
        For better usability, some of the original interfaces are disabled,
        while new interfaces are added.
        Compatibility with STL algorithms is maintained by retaining the iterators
        */
    class String
    {
    public:
        /* Disabled interfaces

        size_t size() const { return _string.size(); }
        void resize(size_t n, char c) { _string.resize(n, c); }
        size_t max_size() const { return _string.max_size(); }

        //  assign
        String& assign(const String& str) { _string.assign(str._string); return *this; }
        String& assign(const String& str, size_t subpos, size_t sublen) { _string.assign(str._string, subpos, sublen); return *this; }
        String& assign(const char* s) { _string.assign(s); return *this; }
        String& assign(const char* s, size_t n) { _string.assign(s, n); return *this; }
        String& assign(size_t n, char c) { _string.assign(n, c); return *this; }
        template<class InputIterator>
        String& assign(InputIterator first, InputIterator last) { _string.assign(first, last); return *this; }
        String& assign(std::initializer_list<char> il) { _string.assign(il); return *this; }

        istream& getline (istream&  is, string& str, char delim);
        istream& getline (istream&& is, string& str, char delim);

        istream& getline (istream&  is, string& str);
        istream& getline (istream&& is, string& str);
        */

        typedef std::string _StringType;

#pragma region Original, or modified interfaces
        //types
        typedef _StringType::iterator iterator;
        typedef _StringType::const_iterator const_iterator;
        typedef _StringType::reverse_iterator reverse_iterator;
        typedef _StringType::const_reverse_iterator const_reverse_iterator;
        //ctor
        String() : _string() {}
        String(const String& str) : _string(str._string) {}
        String(const String& str, size_t pos, size_t len) : _string(str._string, pos, len) {}
        String(const char* s) : _string(s) {}
        String(const char* s, size_t n) : _string(s, n) {}
        String(const u16char* s) { _string = std::move(String::FromWcharArray(s, -1)); }
        String(const u16char* s, size_t n) { _string = std::move(String::FromWcharArray(s, n)); }
        String(size_t n, char c) : _string(n, c) {}
        template<class InputIterator>
        String(InputIterator first, InputIterator last) : _string(first, last) {}
        String(std::initializer_list<char> il) : _string(il) {}
        String(String&& str) : _string(std::move(str._string)) {}
        String(_StringType&& str) { _string = str; }
        //dtor
        ~String()                                                         = default;
        //operator=
        String& operator = (const String& str) { _string = str._string; return *this; }
        String& operator = (const char* s) { _string = s; return *this; }
        String& operator = (char c) { _string = c; return *this; }
        String& operator = (std::initializer_list<char> il) { _string = il; return *this; }
        String& operator = (String&& str) { _string = std::move(str._string); return *this; }
        //iterators
        iterator begin() { return _string.begin(); }
        iterator end() { return _string.end(); }
        const_iterator end() const { return _string.end(); }
        const_iterator begin() const { return _string.begin(); }
        reverse_iterator rbegin() { return _string.rbegin(); }
        reverse_iterator rend() { return _string.rend(); }
        const_reverse_iterator rbegin() const { return _string.rbegin(); }
        const_reverse_iterator rend() const { return _string.rend(); }
        const_iterator cbegin() const { return _string.cbegin(); }
        const_iterator cend() const { return _string.cend(); }
        const_reverse_iterator crbegin() const { return _string.crbegin(); }
        const_reverse_iterator crend() const { return _string.crend(); }
        //capacity
        size_t  Length() const { return _string.length(); }
        void    Resize(size_t n) { _string.resize(n); }
        size_t  Capacity() const { return _string.capacity(); }
        void    Reserve(size_t n = 0) { _string.reserve(n); }
        void    Clear() { _string.clear(); }
        bool    Empty() const { return _string.empty(); }
        void    ShrinkToFit() { _string.shrink_to_fit(); }
        //element access
        char& operator [](size_t pos) { return _string[pos]; }
        char& At(size_t pos) { return _string[pos]; }
        char& Front() { return _string.front(); }
        char& Back() { return _string.back(); }
        const char& operator [](size_t pos) const { return _string[pos]; }
        const char& At(size_t pos) const { return _string[pos]; }
        const char& Front() const { return _string.front(); }
        const char& Back() const { return _string.back(); }
        //modifiers
        //  operator +=
        String& operator += (const String& str) { _string += str._string; return *this; }
        String& operator += (const char* s) { _string += s; return *this; }
        String& operator += (char c) { _string += c; return *this; }
        String& operator += (std::initializer_list<char> il) { _string += il; return *this; }
        //  append
        String& Append(const String& str) { _string.append(str._string); return *this; }
        String& Append(const String& str, size_t subpos, size_t sublen) { _string.append(str._string, subpos, sublen); return *this; }
        String& Append(const char* s) { _string.append(s); return *this; }
        String& Append(const char* s, size_t n) { _string.append(s, n); return *this; }
        String& Append(size_t n, char c) { _string.append(n, c); return *this; }
        template<class InputIterator>
        String& Append(InputIterator first, InputIterator last) { _string.append(first, last); return *this; }
        String& Append(std::initializer_list<char> il) { _string.append(il); return *this; }
        //  insert
        String& Insert(size_t pos, const String& str) { _string.insert(pos, str._string); return *this; }
        String& Insert(size_t pos, const String& str, size_t subpos, size_t sublen) { _string.insert(pos, str._string, subpos, sublen); return *this; }
        String& Insert(size_t pos, const char* s) { _string.insert(pos, s); return *this; }
        String& Insert(size_t pos, const char* s, size_t n) { _string.insert(pos, s, n); return *this; }
        String& Insert(size_t pos, size_t n, char c) { _string.insert(pos, n, c); return *this; }
        iterator Insert(const_iterator p, size_t n, char c) { return _string.insert(p, n, c); }
        iterator Insert(const_iterator p, char c) { return _string.insert(p, c); }
        template <class InputIterator>
        iterator Insert(iterator p, InputIterator first, InputIterator last) { return _string.insert(p, first, last); }
#ifdef TRINITY_PLATFORM_WINDOWS
        String& Insert(const_iterator p, std::initializer_list<char> il) { _string.insert(p, il); return *this; }
#else
        TRINITY_COMPILER_WARNING("String::Insert(iterator p, std::initializer_list<char> il)!")

        // XXX GCC5.2.1 + libstdc++-5 does not include basic_string::insert(const_iterator, initializer_list<>)
        String& Insert(iterator p, std::initializer_list<char> il){ _string.insert(p, il); return *this; }
#endif
        //  remove, renamed from erase
        String& Remove(size_t pos = 0, size_t len = npos) { _string.erase(pos, len); return *this; }
        iterator Remove(const_iterator p) { return _string.erase(p); }
        iterator Remove(const_iterator first, const_iterator last) { return _string.erase(first, last); }
        //  overwrite, renamed from replace
        String& Overwrite(size_t pos, size_t len, const String& str) { _string.replace(pos, len, str._string); return *this; }
        String& Overwrite(const_iterator i1, const_iterator i2, const String& str) { _string.replace(i1, i2, str._string); return *this; }
        String& Overwrite(size_t pos, size_t len, const String& str, size_t subpos, size_t sublen) { _string.replace(pos, len, str._string, subpos, sublen); return *this; }
        String& Overwrite(size_t pos, size_t len, const char* s) { _string.replace(pos, len, s); return *this; }
        String& Overwrite(const_iterator i1, const_iterator i2, const char* s) { _string.replace(i1, i2, s); return *this; }
        String& Overwrite(size_t pos, size_t len, const char* s, size_t n) { _string.replace(pos, len, s, n); return *this; }
        String& Overwrite(const_iterator i1, const_iterator i2, const char* s, size_t n) { _string.replace(i1, i2, s, n); return *this; }
        String& Overwrite(size_t pos, size_t len, size_t n, char c) { _string.replace(pos, len, n, c); return *this; }
        String& Overwrite(const_iterator i1, const_iterator i2, size_t n, char c) { _string.replace(i1, i2, n, c); return *this; }
        template <class InputIterator> String& Overwrite(const_iterator i1, const_iterator i2,
                                                         InputIterator first, InputIterator last)
        {
            _string.replace(i1, i2, first, last); return *this;
        }
        String& Overwrite(const_iterator i1, const_iterator i2, std::initializer_list<char> il) { _string.replace(i1, i2, il); return *this; }
        //  swap
        void Swap(String& str) { _string.swap(str._string); }
        //  pop_back
        void PopBack() { _string.pop_back(); }
        void PushBack(char c) { _string.push_back(c); }
        //string operations
        const char* Data() const { return _string.data(); }
        const char* c_str() const { return _string.c_str(); }
#pragma warning(push)
#pragma warning(disable:4996)
        size_t Copy(char* s, size_t len, size_t pos = 0) const { return _string.copy(s, len, pos); }
#pragma warning(pop)
        //  IndexOf, renamed from find
        size_t IndexOf(const String& str, size_t pos = 0) const { return _string.find(str._string, pos); }
        size_t IndexOf(const char* s, size_t pos = 0) const { return _string.find(s, pos); }
        size_t IndexOf(const char* s, size_t pos, size_t n) const { return _string.find(s, pos, n); }
        size_t IndexOf(char c, size_t pos = 0) const { return _string.find(c, pos); }
        //  IndexOfLast, renamed from rfind
        size_t IndexOfLast(const String& str, size_t pos = npos) const { return _string.rfind(str._string, pos); }
        size_t IndexOfLast(const char* s, size_t pos = npos) const { return _string.rfind(s, pos); }
        size_t IndexOfLast(const char* s, size_t pos, size_t n) const { return _string.rfind(s, pos, n); }
        size_t IndexOfLast(char c, size_t pos = npos) const { return _string.rfind(c, pos); }
        //  FindFirstOf, renamed from find_first_of
        size_t FindFirstOf(const String& str, size_t pos = 0) const { return _string.find_first_of(str._string, pos); }
        size_t FindFirstOf(const char* s, size_t pos = 0) const { return _string.find_first_of(s, pos); }
        size_t FindFirstOf(const char* s, size_t pos, size_t n) const { return _string.find_first_of(s, pos, n); }
        size_t FindFirstOf(char c, size_t pos = 0) const { return _string.find_first_of(c, pos); }
        //  FindLastOf, renamed from find_last_of
        size_t FindLastOf(const String& str, size_t pos = npos) const { return _string.find_last_of(str._string, pos); }
        size_t FindLastOf(const char* s, size_t pos = npos) const { return _string.find_last_of(s, pos); }
        size_t FindLastOf(const char* s, size_t pos, size_t n) const { return _string.find_last_of(s, pos, n); }
        size_t FindLastOf(char c, size_t pos = npos) const { return _string.find_last_of(c, pos); }
        //  FindFirstNotOf, renamed from find_first_not_of
        size_t FindFirstNotOf(const String& str, size_t pos = 0) const { return _string.find_first_not_of(str._string, pos); }
        size_t FindFirstNotOf(const char* s, size_t pos = 0) const { return _string.find_first_not_of(s, pos); }
        size_t FindFirstNotOf(const char* s, size_t pos, size_t n) const { return _string.find_first_not_of(s, pos, n); }
        size_t FindFirstNotOf(char c, size_t pos = 0) const { return _string.find_first_not_of(c, pos); }
        //  FindLastNotOf, renamed from find_last_not_of
        size_t FindLastNotOf(const String& str, size_t pos = npos) const { return _string.find_last_not_of(str._string, pos); }
        size_t FindLastNotOf(const char* s, size_t pos = npos) const { return _string.find_last_not_of(s, pos); }
        size_t FindLastNotOf(const char* s, size_t pos, size_t n) const { return _string.find_last_not_of(s, pos, n); }
        size_t FindLastNotOf(char c, size_t pos = npos) const { return _string.find_last_not_of(c, pos); }
        //  Substring, renamed from substr
        String Substring(size_t pos = 0, size_t len = npos) const { return String(_string.substr(pos, len)); }
        //  Compare, renamed from compare
        int Compare(const String& str) const { return _string.compare(str._string); }
        int Compare(size_t pos, size_t len, const String& str) const { return _string.compare(pos, len, str._string); }
        int Compare(size_t pos, size_t len, const String& str,
                    size_t subpos, size_t sublen) const
        {
            return _string.compare(pos, len, str._string, subpos, sublen);
        }
        int Compare(const char* s) const { return _string.compare(s); }
        int Compare(size_t pos, size_t len, const char* s) const { return _string.compare(pos, len, s); }
        int Compare(size_t pos, size_t len, const char* s, size_t n) const { return _string.compare(pos, len, s, n); }

        friend String operator+ (const String& lhs, const String& rhs);
        friend String operator+ (String&&      lhs, String&&      rhs);
        friend String operator+ (String&&      lhs, const String& rhs);
        friend String operator+ (const String& lhs, String&&      rhs);

        friend String operator+ (const String& lhs, const char*   rhs);
        friend String operator+ (String&&      lhs, const char*   rhs);
        friend String operator+ (const char*   lhs, const String& rhs);
        friend String operator+ (const char*   lhs, String&&      rhs);

        friend String operator+ (const String& lhs, char          rhs);
        friend String operator+ (String&&      lhs, char          rhs);
        friend String operator+ (char          lhs, const String& rhs);
        friend String operator+ (char          lhs, String&&      rhs);

        friend bool operator== (const String& lhs, const String& rhs);
        friend bool operator== (const char*   lhs, const String& rhs);
        friend bool operator== (const String& lhs, const char*   rhs);

        friend bool operator!= (const String& lhs, const String& rhs);
        friend bool operator!= (const char*   lhs, const String& rhs);
        friend bool operator!= (const String& lhs, const char*   rhs);

        friend bool operator<  (const String& lhs, const String& rhs);
        friend bool operator<  (const char*   lhs, const String& rhs);
        friend bool operator<  (const String& lhs, const char*   rhs);

        friend bool operator<= (const String& lhs, const String& rhs);
        friend bool operator<= (const char*   lhs, const String& rhs);
        friend bool operator<= (const String& lhs, const char*   rhs);

        friend bool operator>  (const String& lhs, const String& rhs);
        friend bool operator>  (const char*   lhs, const String& rhs);
        friend bool operator>  (const String& lhs, const char*   rhs);

        friend bool operator>= (const String& lhs, const String& rhs);
        friend bool operator>= (const char*   lhs, const String& rhs);
        friend bool operator>= (const String& lhs, const char*   rhs);

        friend std::istream& operator>>(std::istream& is, String& str);
        friend std::ostream& operator<<(std::ostream& os, const String& str);
        friend std::wistream& operator>>(std::wistream& is, String& str);
        friend std::wostream& operator<<(std::wostream& os, const String& str);

        friend void swap(String& x, String& y);
        static const size_t npos = -1;
#pragma endregion

#pragma region New interfaces
        bool Contains(const String& str) const { return _string.find(str._string) != npos; }
        bool Contains(const char* s) const { return _string.find(s) != npos; }
        bool Contains(const char* s, size_t size) const { return _string.find(s, size) != npos; }
        bool Contains(char c) const { return _string.find(c) != npos; }

        bool StartsWith(const String& str) const
        {
            if (str.Length() > Length()) return false;
            auto search_iterator = begin();
            for (auto c : str)
                if (c != *search_iterator++)
                    return false;
            return true;
        }
        bool StartsWith(const char* s) const
        {
            if (strlen(s) > Length()) return false;
            auto search_iterator = begin();
            for (char* p = (char*)s; *p; ++p)
                if (*p != *search_iterator++)
                    return false;
            return true;
        }
        bool StartsWith(const char* s, size_t size) const
        {
            if (size > Length()) return false;
            const char* end = s + size;
            auto search_iterator = begin();
            for (char* p = (char*)s; p != end; ++p)
                if (*p != *search_iterator++)
                    return false;
            return true;
        }
        bool StartsWith(char c) const
        {
            return (!Empty() && _string.front() == c);
        }

        bool EndsWith(const String& str) const
        {
            size_t sLen = str.Length();
            if (sLen > Length())
                return false;
            auto search_iterator = end() - sLen;
            for (auto c : str)
                if (c != *search_iterator++)
                    return false;
            return true;
        }
        bool EndsWith(const char* s) const
        {
            size_t sLen = strlen(s);
            if (sLen > Length())
                return false;
            auto search_iterator = end() - sLen;
            for (char* p = (char*)s; *p; ++p)
                if (*p != *search_iterator++)
                    return false;
            return true;
        }
        bool EndsWith(const char* s, size_t n) const
        {
            if (n > Length())
                return false;
            auto search_iterator = end() - n;
            const char* end = s + n;
            for (char* p = (char*)s; p != end; ++p)
                if (*p != *search_iterator++)
                    return false;
            return true;
        }
        bool EndsWith(char c) const
        {
            return (!Empty() && _string.back() == c);
        }

        //The decision is to declare all the interfaces to mutate the original string instead of generating a new one
        //And if immutable strings are desirable, just allocate a new string first.
        String& Append(char c) { _string.push_back(c); return *this; }
        String& Replace(char from, char to)
        {
            for (char &c : _string)
                if (c == from)
                    c = to;
            return *this;
        }
        //String Replace(utf8_char from, utf8_char to); TODO
        String& Replace(const String& from, const String& to)
        {
            size_t current_index = 0, from_len = from.Length(), to_len = to.Length();
            const char* from_ptr = from.Data();
            const char* to_ptr = to.Data();
            while (npos != (current_index = _string.find(from_ptr, current_index)))
            {
                _string.replace(current_index, from_len, to_ptr);
                current_index += to_len;
            }
            return *this;
        }
        String& PadLeft(size_t totalWidth, char paddingChar = ' ')
        {
            //XXX totalWidth boundary check
            size_t width = _width();
            if (width < totalWidth)
                return Insert(0, (totalWidth - width), paddingChar);
            return *this;//XXX padding failed should not ignore
        }
        String& PadRight(size_t totalWidth, char paddingChar = ' ')
        {
            //XXX totalWidth boundary check
            size_t width = _width();
            if (width < totalWidth)
                return Insert(Length(), (totalWidth - width), paddingChar);
            return *this;//XXX padding failed should not ignore
        }
        String& ToLower()
        {
            //XXX UTF-8 support
            for (char &c : _string)
                if (isupper(c))
                    c = tolower(c);
            return *this;
        }
        String& ToUpper()
        {
            for (char&c : _string)
                if (islower(c))
                    c = toupper(c);
            return *this;
        }
        String& TrimStart()
        {
            //XXX UTF-8 support
            iterator it = begin();
            while (it != end() && isspace(*it))
                ++it;
            _string.replace(begin(), it, "");
            return *this;
        }
        String& TrimEnd()
        {
            //XXX UTF-8 support
            reverse_iterator it = rbegin();
            while (it != rend() && isspace(*it))
                ++it;
            _string.replace(it.base(), rbegin().base(), "");
            return *this;
        }
        String& Trim()
        {
            TrimStart();
            TrimEnd();
            return *this;
        }
        template<typename... Args> static String Format(const String& format, Args... arguments)
        {
            String ret(format);
            List<String> vec;
            _to_strings(vec, arguments...);

            /* Find all templates for formatting */
            for (size_t template_begin = ret.FindFirstOf('{'), template_end = ret.FindFirstOf('}');
                 template_begin != String::npos && template_end != String::npos;)
            {
                if (template_begin + 1 >= template_end)
                    break;

                String tpl = ret.Substring(template_begin + 1, template_end - template_begin - 1);

                size_t tpl_index;
                int    tpl_padding = 0;

                if (!tpl.TryParse(tpl_index))
                    break;

                if (tpl.Contains(':'))
                    tpl.Split(":")[1].TryParse(tpl_padding);

                String substitution = vec[tpl_index];
                if (tpl_padding > 0)
                {
                    substitution.PadRight(tpl_padding);
                }
                else if (tpl_padding < 0)
                {
                    substitution.PadLeft(-tpl_padding);
                }

                ret.Replace("{" + tpl + "}", substitution);

                template_end = template_begin + substitution.Length();

                template_begin = ret.FindFirstOf('{', template_end);
                if (template_begin != String::npos)
                    template_end = ret.FindFirstOf('}', template_begin);
            }

            return ret;
        }

        template<typename... Args> static inline String Join(const String& separator, Args... items)
        {
            return Join(separator.Data(), items...);
        }
        template<typename T>static inline String Join(const String& separator, const List<T> &items)
        {
            return Join(separator.Data(), items);
        }
        template<typename T>static inline String Join(const String& separator, const Array<T> &items)
        {
            return Join(separator.Data(), items);
        }
        template<typename... Args> static String Join(const char* separator, Args... items)
        {
            List<String> vec;
            _to_strings(vec, items...);
            return _join<List<String>, String>(separator, vec);
        }
        template<typename T> static inline String Join(const char* separator, const List<T> &items)
        {
            return _join<List<T>, T>(separator, items);
        }
        template<typename T> static inline String Join(const char* separator, const Array<T> &items)
        {
            return _join<Array<T>, T>(separator, items);
        }

        size_t CountChar(char value) const
        {
            int count = 0;
            for (char c : _string)
                if (value == c)
                    ++count;
            return count;
        }

        enum class StringSplitOptions
        {
            RemoveEmptyEntries,
            PreserveEmptyEntries,
        };
        Array<String> Split(const char* seperators, StringSplitOptions option = StringSplitOptions::RemoveEmptyEntries)
        {
            List<String> vec;
            size_t split_head = 0;
            size_t search_idx = 0;
            bool non_empty_matched = false;//is the entry before last search_idx non-empty
            while (true)
            {
                split_head = FindFirstNotOf(seperators, search_idx);
                bool split_head_found = (npos != split_head);

                if (option == StringSplitOptions::PreserveEmptyEntries)
                {
                    size_t empty_entry_count;
                    if (!non_empty_matched && !split_head_found)//everything in the string is a separator
                        empty_entry_count = Length() + 1;
                    else if (!non_empty_matched && split_head_found)
                        empty_entry_count = split_head;
                    else if (non_empty_matched && !split_head_found)//everything after last search_idx
                        empty_entry_count = Length() - search_idx - 1;
                    else//empty entries between last search_idx and current split_head
                        empty_entry_count = split_head - search_idx - 1;
                    for (size_t i = 0; i < empty_entry_count; ++i)
                        vec.emplace_back("");
                }

                if (npos != split_head)
                    non_empty_matched = true;

                if (npos == split_head)
                    break;
                search_idx = FindFirstOf(seperators, split_head);
                if (npos == search_idx)
                {
                    vec.emplace_back(Substring(split_head, Length() - split_head));
                    break;
                }
                else
                {
                    vec.emplace_back(Substring(split_head, search_idx - split_head));
                }
            }
            Array<String> ret(vec.size());
            size_t idx = 0;
            for (auto &s : vec)
                ret[idx++] = std::move(s);
            return ret;
        }
#pragma endregion

#pragma region conversions
        operator _StringType() { return _string; }
        operator const _StringType() const { return _string; }
        String& operator =(const _StringType& str) { _string = str; return *this; }
        String(const _StringType& str) { _string = str; }

        //overrides should be provided like template<> static String ToString<type> (type value)
        template<typename T>static String
            ToString(const T& value) { return String(std::to_string(value)); }
        static String ToString(const bool& value) { return value ? "true" : "false"; }
        template<typename T>static String
            ToString(T* ptr)
        {
            const int buffer_size = sizeof(uint64_t) * 2;
            char buf[buffer_size + 1];//every byte needs two chars to represent in hex
#if defined(TRINITY_PLATFORM_WINDOWS)
            sprintf_s(buf, buffer_size + 1, "%llX", (uint64_t)ptr);
#else
            sprintf(buf, "%llX", (uint64_t)ptr);
#endif
            String ret(buf);
            ret.PadLeft(buffer_size, '0');
            return "0x" + ret;
        }
        static String ToString(const String& value) { return value; }
        static String ToString(const std::string& value) { return value; }
        static String ToString(const std::string* value) { return *value; }
        static String ToString(const char* value) { return String(value); }
        static String ToString(const u16char* value) { return String::FromWcharArray(value, -1); }
        static String ToString(char value) { return String(1, value); }
#ifndef __cplusplus_cli
        static String ToString(const std::thread::id &thread_id) { std::stringstream stream; stream << thread_id; return stream.str(); }
#endif

        //TODO more TryParse types
        bool TryParse(String& value)
        {
            value = *this;
            return true;
        }

        bool TryParse(uint64_t& value)
        {
            try
            {
                value = std::stoull(_string);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }

        bool TryParse(int64_t& value)
        {
            try
            {
                value = std::stoll(_string);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }

        bool TryParse(int32_t& value)
        {
            try
            {
                value = std::stoi(_string);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }

        bool TryParse(uint32_t& value)
        {
            try
            {
                value = std::stoul(_string);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }

        bool TryParse(double& value)
        {
            try
            {
                value = std::stod(_string);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }

        // len is ignored on non-windows
        static Array<u16char> Utf8ToUtf16(const char* str, size_t len)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            int wchar_count;

            if (len == 0)
            {
                Array<u16char> empty_arr(1);
                empty_arr[0] = 0;
                return empty_arr;
            }

            if (len > INT_MAX)
                return Array<u16char>(0);

            wchar_count = MultiByteToWideChar(CP_UTF8, 0, str, static_cast<int>(len), NULL, 0);
            if (wchar_count == 0)
                return Array<u16char>(0);

            Array<u16char> wchar_arr(wchar_count + 1);//add another unit for storing <NUL>
            if (0 == MultiByteToWideChar(CP_UTF8, 0, str, static_cast<int>(len), wchar_arr, wchar_count))
                return Array<u16char>(0);
            //Manually set the terminating <NUL>

            wchar_arr[wchar_count] = 0;
            return wchar_arr;
#else
            std::wstring_convert<std::codecvt_utf8_utf16<u16char>, u16char> converter;
            auto wstr = converter.from_bytes(str);
            int wchar_count = wstr.length();
            Array<u16char> wchar_arr(wchar_count + 1);
            memcpy(wchar_arr, wstr.c_str(), wchar_count * sizeof(u16char));
            wchar_arr[wchar_count] = 0;

            return wchar_arr;
#endif
        }

        //converts a utf-8 string to a UTF-16LE (Windows Unicode) array, <NUL> terminated.
        Array<u16char> ToWcharArray() const
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            return Utf8ToUtf16(c_str(), Length());
#else
            return Utf8ToUtf16(c_str(), 0);
#endif
        }

        static String FromWcharArray(const Array<u16char> &arr)
        {
            return FromWcharArray(arr, arr.Length());
        }

        //converts a UTF-16LE (Windows Unicode) array to a utf-8 string.
        //length is the count of wchars in the string.
        //length can be with or without the (if exists) <NUL> termination symbol
        //set length to -1 to search for termination symbol
        static String FromWcharArray(const u16char* ptr, size_t length)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            if (length > INT_MAX && length != SIZE_MAX)//TODO exception
                return "";

            if (length == 0)
                return "";

            int byte_count = WideCharToMultiByte(CP_UTF8, 0, ptr, static_cast<int>(length), NULL, 0, NULL, NULL);
            if (byte_count == 0)//TODO exception
                return "";

            String ret(byte_count, 0);
            // XXX writing to c_str()
            byte_count = WideCharToMultiByte(CP_UTF8, 0, ptr, static_cast<int>(length), (char*)ret.c_str(), byte_count, NULL, NULL);
            if (byte_count == 0)
                return "";

            while (ret.Length() && ret.Back() == 0)
                ret.PopBack();

            return ret;
#else
            std::wstring_convert<std::codecvt_utf8_utf16<u16char>, u16char> converter;
            if(length == npos)
            {
                return converter.to_bytes(ptr);
            }
            else
            {
                return converter.to_bytes(ptr, ptr + length);
            }
#endif
        }

#pragma endregion

    private:

#pragma region Private methods
        static void _to_strings(List<String>& vec) { (void*)(&vec); }
        template<typename T, typename... Args> static void _to_strings(List<String>& vec, T firstArgument, Args... restArguments)
        {
            vec.emplace_back(ToString(firstArgument));
            _to_strings(vec, restArguments...);
        }
        //Obtain the on-screen width (in the sense of fixed-width-alpha-equivalent) of the string
        size_t _width()
        {
            //XXX unicode support
            return Length();
        }

        template<class C, typename T>
        inline static
            typename std::enable_if<std::is_convertible<T, String>::value, String>::type
            _join(const char* separator, const C &container)
        {
            bool first = true;
            String ret;
            for (const T& val : container)
            {
                if (!first)
                    ret.Append(separator);
                else
                    first = false;

                ret.Append(val);
            }
            return ret;
        }

        template<class C, typename T>
        inline static
            typename std::enable_if <!std::is_convertible<T, String>::value, String>::type
            _join(const char* separator, const C &container)
        {
            bool first = true;
            String ret;
            for (const T& val : container)
            {
                if (!first)
                    ret.Append(separator);
                else
                    first = false;

                ret.Append(ToString(val));
            }
            return ret;
        }
#pragma endregion
        _StringType _string;
    };

#pragma region friends
    inline String operator+ (const String& lhs, const String& rhs) { return lhs._string + rhs._string; }
    inline String operator+ (String&&      lhs, String&&      rhs) { return std::move(lhs._string) + std::move(rhs._string); }
    inline String operator+ (String&&      lhs, const String& rhs) { return std::move(lhs._string) + rhs._string; }
    inline String operator+ (const String& lhs, String&&      rhs) { return lhs._string + std::move(rhs._string); }

    inline String operator+ (const String& lhs, const char*   rhs) { return lhs._string + rhs; }
    inline String operator+ (String&&      lhs, const char*   rhs) { return std::move(lhs._string) + rhs; }
    inline String operator+ (const char*   lhs, const String& rhs) { return lhs + rhs._string; }
    inline String operator+ (const char*   lhs, String&&      rhs) { return lhs + std::move(rhs._string); }

    inline String operator+ (const String& lhs, char          rhs) { return lhs._string + rhs; }
    inline String operator+ (String&&      lhs, char          rhs) { return std::move(lhs._string) + rhs; }
    inline String operator+ (char          lhs, const String& rhs) { return lhs + rhs._string; }
    inline String operator+ (char          lhs, String&&      rhs) { return lhs + std::move(rhs._string); }

    inline bool operator== (const String& lhs, const String& rhs) { return lhs._string == rhs._string; }
    inline bool operator== (const char*   lhs, const String& rhs) { return lhs == rhs._string; }
    inline bool operator== (const String& lhs, const char*   rhs) { return lhs._string == rhs; }

    inline bool operator!= (const String& lhs, const String& rhs) { return lhs._string != rhs._string; }
    inline bool operator!= (const char*   lhs, const String& rhs) { return lhs != rhs._string; }
    inline bool operator!= (const String& lhs, const char*   rhs) { return lhs._string != rhs; }

    inline bool operator<  (const String& lhs, const String& rhs) { return lhs._string < rhs._string; }
    inline bool operator<  (const char*   lhs, const String& rhs) { return lhs < rhs._string; }
    inline bool operator<  (const String& lhs, const char*   rhs) { return lhs._string < rhs; }

    inline bool operator<= (const String& lhs, const String& rhs) { return lhs._string <= rhs._string; }
    inline bool operator<= (const char*   lhs, const String& rhs) { return lhs <= rhs._string; }
    inline bool operator<= (const String& lhs, const char*   rhs) { return lhs._string <= rhs; }

    inline bool operator>(const String& lhs, const String& rhs) { return lhs._string > rhs._string; }
    inline bool operator>(const char*   lhs, const String& rhs) { return lhs > rhs._string; }
    inline bool operator>(const String& lhs, const char*   rhs) { return lhs._string > rhs; }

    inline bool operator>= (const String& lhs, const String& rhs) { return lhs._string >= rhs._string; }
    inline bool operator>= (const char*   lhs, const String& rhs) { return lhs >= rhs._string; }
    inline bool operator>= (const String& lhs, const char*   rhs) { return lhs._string >= rhs; }

    inline std::istream& operator>>(std::istream& is, String& str) { is >> str._string; return is; }
    inline std::ostream& operator<<(std::ostream& os, const String& str) { os << str._string; return os; }

    inline void swap(String& x, String& y) { swap(x._string, y._string); }
#pragma endregion

    typedef std::reference_wrapper<const String> cstring_ref;

}
