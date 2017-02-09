// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <Trinity/String.h>
#include <Trinity/Array.h>
#include <os/os.h>
#include "Directory.h"

#if !defined(TRINITY_PLATFORM_WINDOWS)
#include <cstdlib>
#endif

/**
 *  For Windows platform, we always convert all Unix directory separators '/'
 *  to Windows directory separators '\', but for *nix platforms, as Windows
 *  separator '\' is a valid character for paths, we make no substitutions.
 */

namespace Trinity
{
    namespace IO
    {
        namespace Path
        {
            using Trinity::String;
            using Trinity::Array;
            //XXX WindowsDirectorySeparator might not be \ on Japan/Korea OS
#pragma warning(push)
#pragma warning(disable:4114)
            static const char WindowsDirectorySeparator = '\\';
            static const char UnixDirectorySeparator = '/';
            static const char *DirectorySeparators = "/\\";
#pragma warning(pop)
            inline String ChangeExtension(const String& path);
            inline bool IsUncPath(const String& path)
            {
                if (path.Contains(UnixDirectorySeparator))
                    return false;
                if (path.StartsWith(String(2, WindowsDirectorySeparator)))
                    return true;
                return false;
            }
            inline bool IsPathRooted(const String& path)
            {
                /**
                 * Current supported cases:
                 * 1. Classical Windows fullpath, e.g.
                 * c:/foo/bar
                 * 2. UNC path, e.g.
                 * \\graph006
                 * 3. Unix fullpath, e.g.
                 * /foo/bar
                 */
#if defined(TRINITY_PLATFORM_WINDOWS)
                if (path.Length() > 2 && isalpha(path[0]) && path[1] == ':' &&
                    (path[2] == WindowsDirectorySeparator || path[2] == UnixDirectorySeparator))
                    return true;
                if (IsUncPath(path))
                    return true;
#else
                if (path[0] == UnixDirectorySeparator)
                    return true;
#endif
                return false;
            }
            /*
            True if and only if the path is root
            Note that for UNC paths, root path is of form:
            \\endpoint\sharename
            */
            inline bool IsPathRootOnly(const String& path)
            {
                if (!IsPathRooted(path))
                    return false;
                if (IsUncPath(path))
                {
                    return path.CountChar(WindowsDirectorySeparator) == 3;
                }
                else if (path[0] == UnixDirectorySeparator)
                {
                    //unix fullpath
                    return path.Length() == 1;
                }
                else
                {
                    //windows full path
                    return path.Length() == 3;
                }
            }
            //Expand and normalize a path
            /*
            according to http://msdn.microsoft.com/en-us/library/windows/desktop/aa364963(v=vs.85).aspx
            GetFullPathName and {Get, Set}CurrentDirectory are not safe under
            multithreaded env

            TODO resource guard for these routines
            */
            inline String GetFullPath(const String& path)
            {
#if defined (TRINITY_PLATFORM_WINDOWS)
                auto wpath = path.ToWcharArray();
                DWORD path_size = GetFullPathNameW(wpath, 0, NULL, NULL);
                if (path_size == 0)
                    return "";//TODO exception
                Array<u16char> wfull_path(path_size);
                path_size = GetFullPathNameW(wpath, path_size, wfull_path, NULL);
                if (path_size == 0)
                    return "";

                return String::FromWcharArray(wfull_path).Replace(UnixDirectorySeparator, WindowsDirectorySeparator);
#else// other platforms

                char* full_path_buf = new char[PATH_MAX];
                char* full_path = realpath(path.c_str(), full_path_buf);
                if (full_path == NULL) 
                {
                    // There are several cases where realpath returns NULL
                    // due to some errors, but the full path is completed
                    // and placed into our buffer. See the man pages for
                    // realpath for details.
                    auto error = errno;
                    do
                    {
                        if(error == EACCES)  break;
                        if(error == EIO)     break;
                        if(error == ENOENT)  break;
                        if(error == ENOTDIR) break;
                        full_path_buf[0] = 0; 
                    }while (0);
                }
                String ret(full_path_buf);
                delete[] full_path_buf;
                return ret;
#endif
            }

            //  Forward declare Combine, definition follows closely.
            template<typename ...Args> inline String Combine(const Args& ...paths);

            inline String __combine_impl()
            {
                return "";
            }

            inline String __combine_impl(const String& parent, const String& child)
            {
                //TODO platform
                if (IsPathRooted(child))
                    return GetFullPath(child);

                char separator;

#if defined(TRINITY_PLATFORM_WINDOWS)
                separator = WindowsDirectorySeparator;
#else
                separator = UnixDirectorySeparator;
#endif

                String ret = parent;

#if defined(TRINITY_PLATFORM_WINDOWS)
                if (!ret.EndsWith(WindowsDirectorySeparator) && !ret.EndsWith(UnixDirectorySeparator))//no separators at the tail of parent
#else
                if (!ret.EndsWith(separator) && ret.Length() != 0)//no separators at the tail of parent
#endif
                {
                    ret.PushBack(separator);
                }
#if defined(TRINITY_PLATFORM_WINDOWS)
                size_t child_start_idx = child.FindFirstNotOf(DirectorySeparators);
#else
                size_t child_start_idx = child.FindFirstNotOf(separator);
#endif

                if (child_start_idx != String::npos)
                    ret.Append(child.begin() + child_start_idx, child.end());

#if defined(TRINITY_PLATFORM_WINDOWS)
                ret.Replace(UnixDirectorySeparator, WindowsDirectorySeparator);
#endif
                return ret;
            }

            template<typename ...Args>
            inline String
                __combine_impl(const String& path_1, const Args& ...rest)
            {
                if (sizeof...(rest) == 0)
                {
#if defined(TRINITY_PLATFORM_WINDOWS)
                    String ret = path_1;
                    return ret.Replace(UnixDirectorySeparator, WindowsDirectorySeparator);
#else
                    return path_1;
#endif
                }
                else
                {
                    String path = Combine(rest...);
                    path = __combine_impl(path_1, path);
                    return path;
                }
            }

            template<typename ...Args>
            inline String
                Combine(const Args& ...paths)
            {
                return __combine_impl(paths...);
            }


            inline String GetDirectoryName(const String& path)
            {
                if (IsPathRootOnly(path))
                    return "";
                size_t idx = path.FindLastOf(DirectorySeparators);
                if (idx == String::npos)
                    return "";
                String ret = path.Substring(0, idx + 1);

                if (IsPathRootOnly(ret))
                    return ret;
                ret.PopBack();
                return ret;
            }

            inline String GetFileName(const String& path)
            {
                size_t len = path.Length();
                size_t idx = len;
                while (--idx != String::npos)
                    if (path[idx] == WindowsDirectorySeparator || path[idx] == UnixDirectorySeparator)
                        return path.Substring(idx + 1);
                return path;
            }

            inline String GetExtension(String path)
            {
                path = GetFileName(path);
                size_t last_dot = path.IndexOfLast('.');
                if (last_dot == String::npos) return "";
                else return path.Substring(last_dot);
            }

            inline String GetFileNameWithoutExtension(String path)
            {
                path = GetFileName(path);
                size_t last_dot = path.IndexOfLast('.');
                if (last_dot == String::npos) return path;
                else return path.Substring(0, last_dot);
            }

            inline Array<char> GetInvalidFileNameChars();
            inline Array<char> GetInvalidPathChars();
            inline String GetPathRoot(String path)
            {
                //TODO check if Path is valid
                path = GetFullPath(path);
                while (!IsPathRootOnly(path))
                    path = GetDirectoryName(path);
                return path;
            }

            inline String CompletePath(String path, bool create_nonexistent)
            {
                if (path.Empty() || Path::IsPathRootOnly(path))
                {
                    return path;
                }

                path = Path::GetFullPath(path);

#if defined(TRINITY_PLATFORM_WINDOWS)
                if (path.Back() == WindowsDirectorySeparator || path.Back() == UnixDirectorySeparator)
                    path.PopBack();
#else
                if (path.Back() == UnixDirectorySeparator)
                    path.PopBack();
#endif

                if (create_nonexistent)
                {
                    String parent_dir = Path::GetDirectoryName(path);

                    if (!Directory::Exists(parent_dir))
                    {
                        CompletePath(parent_dir, true);
                    }

                    if (!Directory::Exists(path))
                    {
                        Directory::Create(path);
                    }
                }

                return path;
            }

            //see [[pop macro]]
            inline String GetRandomFileName();//P2
            inline String GetTempFileName();//P2
            inline String GetTempPath();//P2
            inline bool HasExtension(const String& path);//P2

            inline String MyAssemblyPath()
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                Array<u16char> lpFilename(1024);
                HMODULE        hmodule = GetModuleHandleW(L"Trinity.C.dll");
                /* If Trinity.C.dll is absent, we default to the executing assembly (sending NULL into the API) */
                GetModuleFileNameW(hmodule, lpFilename, static_cast<DWORD>(lpFilename.Length()));
                lpFilename[lpFilename.Length() - 1] = 0;
                return GetDirectoryName(GetFullPath(String::FromWcharArray(lpFilename, -1)));
#elif defined(TRINITY_PLATFORM_LINUX)
                // XXX does not make sense if MyAssemblyPath always point to the mono host...
                char* filename_buf    = new char[1024];
                int filename_buf_size = readlink("/proc/self/exe", filename_buf, 1024);
                if (filename_buf_size < 0) { filename_buf_size = 0; }
                filename_buf[filename_buf_size] = 0;
                String ret(filename_buf);
                delete[] filename_buf;
                return GetDirectoryName(ret);
#else
#error Not supported
#endif
            }
        }
    }
}
