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
                    if((pdir = opendir(path.c_str())) != NULL)
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
        }
    }
}