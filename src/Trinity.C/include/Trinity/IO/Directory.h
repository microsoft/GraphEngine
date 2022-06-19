// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#include <Trinity/String.h>
namespace Trinity
{
    namespace IO
    {
        namespace Path
        {
            String GetFullPath(const String&);
            String GetDirectoryName(const String&);
            template<typename ...Args> String Combine(const Args& ...paths);
        }

        namespace Directory
        {

            inline bool Exists(const String& path)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                DWORD attrs = GetFileAttributesW(path.ToWcharArray());
                return ((attrs != INVALID_FILE_ATTRIBUTES) && (attrs & FILE_ATTRIBUTE_DIRECTORY));
#else
                if (-1 != access(path.c_str(), F_OK))
                {
                    DIR* pdir;
                    if ((pdir = opendir(path.c_str())) != NULL)
                    {
                        closedir(pdir);
                        return true;
                    }
                }
                return false;
#endif
            }

            inline bool Create(const String& path)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                auto path_str = Path::GetFullPath(path).ToWcharArray();
                return (TRUE == ::CreateDirectoryW(path_str, NULL));
#else
                return !mkdir(path.c_str(), S_IRWXU | S_IRWXG | S_IROTH | S_IXOTH);
#endif
            }

            inline bool EnsureDirectory(const String& path)
            {
                if (path == "" || Directory::Exists(path))
                    return true;
                auto parent = Path::GetDirectoryName(path);
                if (!EnsureDirectory(parent))
                    return false;
                return Create(path);
            }

#ifndef __cplusplus_cli

            inline std::vector<String> GetDirectories(const String& path)
            {
                std::vector<String> ret;

#if defined(TRINITY_PLATFORM_WINDOWS)

                WIN32_FIND_DATA find_data;
                HANDLE hfind = FindFirstFile(Path::Combine(path, "*").ToWcharArray(),
                                             &find_data);

                if (hfind == INVALID_HANDLE_VALUE)
                {
                    return ret;
                }

                do
                {
                    if (find_data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
                    {
                        auto filename = String::FromWcharArray(find_data.cFileName, String::npos);
                        if (filename != "." && filename != "..")
                        {
                            ret.push_back(Path::Combine(path, filename));
                        }
                    }
                } while (FindNextFile(hfind, &find_data) != 0);
                FindClose(hfind);

#else

                DIR* pdir = opendir(path.Data());
                if (!pdir)
                {
                    return ret;
                }

                dirent* pentry;
                while (nullptr != (pentry = readdir(pdir)))
                {
                    if (pentry->d_type & DT_DIR)
                    {
                        auto filename = String(pentry->d_name);
                        if (filename != "." && filename != "..")
                        {
                            ret.push_back(Path::Combine(path, filename));
                        }
                    }
                }

                closedir(pdir);
#endif
                return ret;
            }

            template<typename T>
            inline std::vector<String> GetFiles(const String& directory, const T& suffices)
            {
                std::vector<String> ret;

                for (const String &suffix : suffices)
                {
#if defined(TRINITY_PLATFORM_WINDOWS)
                    WIN32_FIND_DATA find_data;
                    String pattern = Path::Combine(directory, "*") + suffix;
                    HANDLE hfind = FindFirstFile(pattern.ToWcharArray(), &find_data);

                    if (hfind == INVALID_HANDLE_VALUE)
                    {
                        break;
                    }

                    do
                    {
                        if (!(find_data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
                        {
                            ret.push_back(String::FromWcharArray(find_data.cFileName, String::npos));
                        }
                    } while (FindNextFile(hfind, &find_data) != 0);
                    FindClose(hfind);
#else
                    DIR*    pdir = opendir(directory.Data());
                    dirent* pentry;
                    if (!pdir)
                    {
                        break;
                    }

                    while (nullptr != (pentry = readdir(pdir)))
                    {
                        // see: https://github.com/dotnet/coreclr/blob/master/src/coreclr/hosts/unixcoreruncommon/coreruncommon.cpp
                        switch (pentry->d_type)
                        {
                        case DT_REG:
                            break;

                            // Handle symlinks and file systems that do not support d_type
                        case DT_LNK:
                        case DT_UNKNOWN:
                        {
                            std::string fullFilename;

                            fullFilename.append(directory);
                            fullFilename.append("/");
                            ret.push_back(pentry->d_name);

                            struct stat sb;
                            if (stat(fullFilename.c_str(), &sb) == -1)
                            {
                                continue;
                            }

                            if (!S_ISREG(sb.st_mode))
                            {
                                continue;
                            }
                        }
                        break;

                        default:
                            continue;
                        }

                        String fname(pentry->d_name);
                        if (suffix.Empty() || fname.EndsWith(suffix))
                        {
                            ret.push_back(pentry->d_name);
                        }
                    }
                    closedir(pdir);
#endif
                }// foreach ext

                return ret;
            }

            inline std::vector<String> GetFiles(const String& directory)
            {
                return GetFiles(directory, std::vector<String>({ "" }));
            }
#endif // __cplusplus_cli
        }
    }
}
