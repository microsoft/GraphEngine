// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
/***************************************************

    REFER TO [[RVA Probe]] for more details!

    NOTE: 0xFF is used as a WILDCARD in needles!
    Use it with great care!

    ***************************************************/
#include "Transition.h"
#include <io>

namespace Runtime
{
    uint64_t LazyMachStateCaptureState = 0x2F70;
    uint64_t HelperFramePush           = 0x49BC;
    uint64_t HelperFramePop            = 0x499C;
    uint64_t FrameRestore              = 0x20F0;
    uint64_t vtable                    = 0x6A5550;
    uint64_t ThreadNativeSleep         = 0x206A10;
    uint64_t GCPreempCtor              = 0x3840;
    uint64_t GCCoopCtor                = 0x3800;

#ifdef TRANSITION_DEBUG
#define DEBUG(x) printf("%s = %llX\n", #x, x)
#define DEBUG_WRITELINE(format, ...) Console::WriteLine(format, __VA_ARGS__)
#else
#define DEBUG_PRINT(x)
#define DEBUG_WRITELINE(format, ...)
#endif

    void TransitionSleep(int32_t duration)
    {
        //TRINITY_INTEROP_ENTER_UNMANAGED();
        YieldProcessor();
        //TRINITY_INTEROP_LEAVE_UNMANAGED();
    }

    int64_t build_integer(const uint8_t* base, const uint64_t size)
    {
        int64_t ret = *(int64_t*)base;
        if (size < 8)
            ret &= ((1 << (size * 8)) - 1);
        return ret;
    }

    const uint8_t* find_needle(const uint8_t* haystack, const uint8_t* needle, size_t haystack_size, size_t needle_size)
    {
        for (const uint8_t* p = haystack, *e = haystack + haystack_size - needle_size; p < e; ++p)
        {
            bool matched = true;
            for (size_t i = 0; i < needle_size; ++i)
            {
                if (needle[i] == 0xFF)
                    continue;
                if (p[i] != needle[i])
                {
                    matched = false;
                    break;
                }
            }
            if (matched)
                return p;
        }
        return NULL;
    }

    void SearchForThreadNativeSleep(uint8_t* base, size_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x48, 0x81, 0xec, 0x40, 0x01, 0x00, 0x00,
            0x48, 0xc7, 0x44, 0x24, 0x48, 0xfe, 0xff, 0xff, 0xff,
            0x48, 0x89, 0x58, 0x10,
            0x48, 0x89, 0x70, 0x18,
            0x48, 0x89, 0x78, 0x20,
            0x8b, /* WILDCARD */ 0xFF,
        };
        const uint8_t* target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3009.");
            abort();
        }

        DEBUG_WRITELINE("Matched: ThreadNative::Sleep = {0}", (int*)(target - base));

        ThreadNativeSleep = (uint64_t)target;
    }

    void SearchForHelperMethodFrame_vftable(uint8_t* base, uint64_t size)
    {
        uint8_t* haystack = (uint8_t*)base;
        /* Find lea rax, [clr!HelperMethodFrame::`vftable'] */

        const uint8_t needle[] ={ 0x48, 0x8d, 0x05, };

        /* The size of the original ThreadNative::Sleep routine is roughly 0x100 */

        const uint8_t* vtable_target = find_needle(haystack, needle, size, sizeof(needle));

        if (!vtable_target)
        {
            Console::WriteLine("Internal error T3010.");
            abort();
        }

        vtable_target += sizeof(needle);

        uint64_t vtable_pos_to_ip = build_integer(vtable_target, 3);
        vtable_target += 4; /*vtable_target is now effectively RIP*/

        DEBUG_WRITELINE("Matched: vftable = {0}", (int*)(vtable_target + vtable_pos_to_ip));

        vtable = (uint64_t)(vtable_target + vtable_pos_to_ip);
    }

    void SearchForLazyMachStateCaptureState(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x48, 0x8b, 0x14, 0x24,
            0x48, 0x89, 0x79, 0x10,
            0x48, 0x89, 0x71, 0x18,
            0x48, 0x89, 0x59, 0x20,
            0x48, 0x89, 0x69, 0x28,
            0x4c, 0x89, 0x61, 0x30,
            0x4c, 0x89, 0x69, 0x38,
            0x4c, 0x89, 0x71, 0x40,
            0x4c, 0x89, 0x79, 0x48,
            0x48, 0xc7, 0x81, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x48, 0x89, 0x91, 0x98, 0x00, 0x00, 0x00,
            0x48, 0x89, 0xa1, 0xa0, 0x00, 0x00, 0x00,
            0xc3,
        };
        const uint8_t* target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3011");
            abort();
        }

        DEBUG_WRITELINE("Matched: LazyMachStateCaptureState = {0}", (int*)target);

        LazyMachStateCaptureState = (uint64_t)target;
    }

    void SearchForHelperMethodFramePush(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x53,
            0x48, 0x83, 0xec, 0x20,
            0x48, 0x8b, 0x05,
        };
        const uint8_t* target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3012");
            abort();
        }

        DEBUG_WRITELINE("Matched: HelperMethodFrame::Push = {0}", (int*)target);

        HelperFramePush = (uint64_t)target;
    }

    void SearchForHelperMethodFramePop(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0xf6, 0x41, 0x18, 0x40,
            0x48, 0x8b, 0x51, 0x20,
            0x75, 0x0d,
            0x8b, 0x42, 0x08,
            0x0f, 0xba, 0xe0, 0x1c,
            0x0f, 0x82, /*! This is an absolute jb, may differ in runtimes! */0xFF, 0xFF, 0xFF, /* WILDCARD END */ 0x00,
            0x48, 0x8b, 0x41, 0x08,
            0x48, 0x89, 0x42, 0x10,
            0xc3,
        };

        const uint8_t* target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3013");
            abort();
        }

        //if (target != haystack + HelperFramePop)
        //{
        //	Console::WriteLine("ERROR FIND HelperFramePop");
        //	abort();
        //}

        DEBUG_WRITELINE("Matched: HelperMethodFrame::Pop = {0}", (int*)target);

        HelperFramePop = (uint64_t)target;
    }

    void SearchForGCPreemp(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x48, 0x89, 0x03,
            0x48, 0x85, 0xc0,
            0x0f, 0x84, /* absolute je */ 0xFF, 0xFF, 0xFF, /* WILDCARD END */ 0x00,
            0x8b, 0x40, 0x0c,
            0x89, 0x43, 0x08,
            0x85, 0xc0,
        };

        const uint8_t* target;

        target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3014");
            abort();
        }

        /* The needle is GCPreemp::GCPreemp + 0x0d, twist it back now. */

        target -= 0x0d;

        DEBUG_WRITELINE("Matched: GCPreemp::GCPreemp = {0}", (int*)target);

        GCPreempCtor = (uint64_t)target;
    }

    void SearchForGCCoop(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x48, 0x89, 0x03,
            0x8b, 0x50, 0x0c,
            0x89, 0x53, 0x08,
            0x85, 0xd2,
        };

        const uint8_t needle2[] =
        {
            0x48, 0x8b, 0x0b,
            0xc7, 0x41, 0x0c, 0x01, 0x00, 0x00, 0x00,
            0x8b, 0x05, 0xff, 0xff, 0xff, 0xff,
            0x85, 0xc0,
        };

        const uint8_t* target, *target2;
        while (NULL != (target = find_needle(haystack, needle, size, sizeof(needle))))
        {
            size -= (target - haystack + sizeof(needle));
            haystack = target + sizeof(needle);

            /* Limit search range to 0x20 from test edx,edx (0x85, 0xd2) */
            target2 = find_needle(haystack, needle2, 0x20, sizeof(needle2));
            if (target2 != NULL)
                break;
        }

        if (target == NULL)
        {
            Console::WriteLine("Internal error T3015");
            abort();
        }

        /* The needle is GCCoop::GCCoop + 0x0d, twist it back now. */

        target -= 0x0d;

        DEBUG_WRITELINE("Matched: GCCoop::GCCoop = {0}", (int*)target);

        GCCoopCtor = (uint64_t)target;
    }

    void SearchForHelperMethodFrameRestoreState(uint8_t* base, uint64_t size)
    {
        const uint8_t* haystack = (const uint8_t*)base;
        const uint8_t needle[] =
        {
            0x33, 0xc0,
            0x48, 0x39, 0x81, 0x90, 0x00, 0x00, 0x00,
            0x75, 0x02,
            0xf3, 0xc3,
            0x48, 0x8d, 0x41, 0x10,
            0x48, 0x8b, 0x51, 0x50,
            0x48, 0x3b, 0xc2,
            0x48, 0x0f, 0x44, 0x38,
            0x48, 0x8d, 0x41, 0x18,
            0x48, 0x8b, 0x51, 0x58,
            0x48, 0x3b, 0xc2,
            0x48, 0x0f, 0x44, 0x30,
            0x48, 0x8d, 0x41, 0x20,
            0x48, 0x8b, 0x51, 0x60,
            0x48, 0x3b, 0xc2,
        };

        const uint8_t* target = find_needle(haystack, needle, size, sizeof(needle));
        if (target == NULL)
        {
            Console::WriteLine("Internal error T3016");
            abort();
        }

        DEBUG_WRITELINE("Matched: HelperMethodFrameRestoreState = {0}", (int*)target);

        FrameRestore = (uint64_t)target;
    }

    void ProbeCLRMethodAddresses()
    {
        uint8_t* base;
        uint64_t size;

#ifdef TRINITY_PLATFORM_WINDOWS
        HMODULE CLRModuleBase;
        MODULEINFO CLRModuleInfo;

        if (!GetModuleHandleExW(NULL, L"clr", &CLRModuleBase))
        {
            Console::WriteLine("Cannot get clr module base.");
            abort();
        }

        if (!GetModuleInformation(GetCurrentProcess(), CLRModuleBase, &CLRModuleInfo, sizeof(CLRModuleInfo)))
        {
            Console::WriteLine("Cannot retrieve clr module information.");
            abort();
        }

        base = (uint8_t*)CLRModuleBase;
        size = CLRModuleInfo.SizeOfImage;
#else
TRINITY_COMPILER_WARNING("ProbeCLRMethodAddresses: unsupported platform")
#endif

        DEBUG_PRINT((int*)base);
        SearchForThreadNativeSleep(base, size);
        SearchForHelperMethodFrame_vftable((uint8_t*)ThreadNativeSleep, 0x100);
        SearchForLazyMachStateCaptureState(base, size);
        SearchForHelperMethodFramePush(base, size);

        SearchForHelperMethodFramePop(base, size);
        SearchForGCPreemp(base, size);
        SearchForGCCoop(base, size);

        SearchForHelperMethodFrameRestoreState(base, size);
    }

#ifdef TRINITY_OPTIONAL_PREEMPTIVE
    bool __transition_enabled = false;
#endif

}