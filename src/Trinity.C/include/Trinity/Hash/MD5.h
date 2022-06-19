// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <Trinity/String.h>
#include <os/os.h>
#include <stdint.h>
#include <locale>
#include <ctime>

#if defined(TRINITY_PLATFORM_WINDOWS)
#include <windows.h>
#include <Wincrypt.h>
#else
#include <openssl/md5.h>
#endif

#include <sstream>

#define MD5HASH_LEN  16

namespace Trinity
{
    namespace Hash
    {
#if defined(TRINITY_PLATFORM_WINDOWS)
        typedef struct
        {
            HCRYPTPROV crypto_provider_handle;
            HCRYPTHASH crypto_hash_handle;
        }MD5_CTX;
#endif

        class MD5
        {
            bool m_good;
            MD5_CTX md5_ctx;

        public:
            MD5()
            {
                m_good = GetHashHandles(md5_ctx);
            }

            bool good()
            {
                return m_good;
            }

            ~MD5()
            {
                if (m_good)
                {
                    ReleaseHandles(md5_ctx);
                }
            }

            void hash(const char* buffer, const uint32_t length)
            {
                Hash(md5_ctx, buffer, length);
            }

            void getValue(OUT char* hash_buffer)
            {
                FillHashBuffer(md5_ctx, hash_buffer);
            }

            static inline std::string GetHashString(char* hash_buffer)
            {
                BYTE* buffer = (BYTE*)hash_buffer;
                CHAR hash_digits[] = "0123456789abcdef";

                std::stringstream ss;

                for (DWORD i = 0; i < MD5HASH_LEN; i++)
                {
                    ss << hash_digits[buffer[i] >> 4] << hash_digits[buffer[i] & 0xf];
                }
                return ss.str();
            }

            static inline void PrintHash(char* hash_buffer)
            {
                Console::WriteLine(GetHashString(hash_buffer));
            }
        private:
#if defined(TRINITY_PLATFORM_WINDOWS)
            static inline bool GetHashHandles(MD5_CTX& md5_ctx)
            {
                if (!CryptAcquireContext(&md5_ctx.crypto_provider_handle, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
                    return false;

                if (!CryptCreateHash(md5_ctx.crypto_provider_handle, CALG_MD5, 0, 0, &md5_ctx.crypto_hash_handle))
                {
                    CryptReleaseContext(md5_ctx.crypto_provider_handle, 0);
                    return false;
                }
                return true;
            }

            static inline bool Hash(MD5_CTX& ctx, const char * buffer, const uint32_t length)
            {
                if (!CryptHashData(ctx.crypto_hash_handle, (BYTE*)buffer, length, 0))
                    return false;
                return true;
            }

            static inline bool FillHashBuffer(MD5_CTX& ctx, char* hash_buffer)
            {
                DWORD hash_len = MD5HASH_LEN;
                if (!CryptGetHashParam(ctx.crypto_hash_handle, HP_HASHVAL, (BYTE*)hash_buffer, &hash_len, 0))
                    return false;
                return true;
            }

            static inline void ReleaseHandles(MD5_CTX& ctx)
            {
                CryptDestroyHash(ctx.crypto_hash_handle);
                CryptReleaseContext(ctx.crypto_provider_handle, 0);
            }
#else
            static inline bool GetHashHandles(MD5_CTX& md5_ctx)
            {
                return MD5_Init(&md5_ctx);
            }

            static inline bool Hash(MD5_CTX& ctx, const char * buffer, const uint32_t length)
            {
                return MD5_Update(&ctx, buffer, length);
            }

            static inline bool FillHashBuffer(MD5_CTX& ctx, char* hash_buffer)
            {
                return MD5_Final((BYTE*)hash_buffer, &ctx);
            }

            static inline void ReleaseHandles(MD5_CTX& ctx)
            {
                /* Not needed */
            }
#endif

        };
    }
}
