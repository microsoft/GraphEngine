// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include "os/os.h"

namespace Runtime
{

    extern uint64_t LazyMachStateCaptureState;
    extern uint64_t HelperFramePush;
    extern uint64_t HelperFramePop;
    //clr!HelperMethodFrameRestoreState
    extern uint64_t FrameRestore;
    extern uint64_t vtable;

    extern uint64_t ThreadNativeSleep;

    extern uint64_t GCPreempCtor;
    extern uint64_t GCCoopCtor;

    void ProbeCLRMethodAddresses();

    enum FrameAttribs
    {
        FRAME_ATTR_NONE = 0,
        FRAME_ATTR_EXCEPTION = 1,           // This frame caused an exception
        FRAME_ATTR_OUT_OF_LINE = 2,         // The exception out of line (IP of the frame is not correct)
        FRAME_ATTR_FAULTED = 4,             // Exception caused by Win32 fault
        FRAME_ATTR_RESUMABLE = 8,           // We may resume from this frame
        FRAME_ATTR_CAPTURE_DEPTH_2 = 0x10,  // This is a helperMethodFrame and the capture occurred at depth 2
        FRAME_ATTR_EXACT_DEPTH = 0x20,      // This is a helperMethodFrame and a jit helper, but only crawl to the given depth
        FRAME_ATTR_NO_THREAD_ABORT = 0x40,  // This is a helperMethodFrame that should not trigger thread aborts on entry
    };


    // A helper method frame with a GSCookie in the front.
    // The layout:
    // struct {
    //     GSCookie cookie;
    //     HelperMethodFrame frame;
    // }HelperMethodFrameWithCookie;

    // Layout of HelperMethodFrame:
    // uint64_t vtable_ptr;
    // uint64_t m_NextFrame;
    // uint64_t m_pMD;              // ptr to method desc
    // uint32_t m_Attribs;          // will be padded to 64-bit
    // uint64_t m_pThread;
    // uint64_t m_FCallEntry;       // currently we always feed 0
    // LazyMachState m_MachState;
    struct HelperMethodFrameWithCookie
    {
    public:
        HelperMethodFrameWithCookie(uint64_t func_addr)
        {
            FrameStart[0] = vtable;
            FrameStart[1] = 0;
            FrameStart[2] = 0;
            //FrameStart[3] = FrameAttribs::FRAME_ATTR_NO_THREAD_ABORT;
            FrameStart[3] = 0;
            FrameStart[4] = 0;
            FrameStart[5] = func_addr;
        }
        size_t cookie = 0;
        uint64_t FrameStart[6];
        uint64_t MachState[24];
    };
    typedef void(*CLRMethodPointer)(uint64_t*);

    inline void SetPreemptive(uint64_t* gcPreemp)
    {
        CLRMethodPointer gcPreempCtor = (CLRMethodPointer)GCPreempCtor;
        gcPreempCtor(gcPreemp);
    }
    inline void ClearPreemptive(uint64_t* gcCoop)
    {
        CLRMethodPointer gcCoopCtor = (CLRMethodPointer)GCCoopCtor;
        gcCoopCtor(gcCoop);
    }
    inline void CaptureState(HelperMethodFrameWithCookie& frame)
    {
        CLRMethodPointer clr_LazyMachStateCaptureState = (CLRMethodPointer)LazyMachStateCaptureState;
        clr_LazyMachStateCaptureState(frame.MachState);
    }
    inline void RestoreState(HelperMethodFrameWithCookie& frame)
    {
        CLRMethodPointer clr_HelperMethodFrameRestoreState = (CLRMethodPointer)FrameRestore;
        clr_HelperMethodFrameRestoreState(frame.MachState);
    }
    inline void ConstructFrame(HelperMethodFrameWithCookie& frame)
    {
        CLRMethodPointer clr_HelperMethodFrame_Push = (CLRMethodPointer)HelperFramePush;
        clr_HelperMethodFrame_Push(frame.FrameStart);
    }
    inline void DeconstructFrame(HelperMethodFrameWithCookie& frame)
    {
        CLRMethodPointer clr_HelperMethodFrame_Pop = (CLRMethodPointer)HelperFramePop;
        clr_HelperMethodFrame_Pop(frame.FrameStart);
    }

    inline void Spinwait(int32_t duration)
    {
        for (int delay = 0; delay < duration; ++delay) 
        {
#if defined (TRINITY_PLATFORM_WINDOWS)
            __noop();
#else
            asm("");
#endif
        }


        //YieldProcessor();
    }

#if defined (TRINITY_PLATFORM_WINDOWS) && !defined (CORECLR)
#define TRINITY_FORCE_PREEMTIVE
//#define TRINITY_DISABLE_PREEMPTIVE
//#define TRINITY_OPTIONAL_PREEMPTIVE
#else
#define TRINITY_DISABLE_PREEMPTIVE
#endif

#if defined(TRINITY_FORCE_PREEMTIVE)
#define TRINITY_INTEROP_ENTER_UNMANAGED() \
	Runtime::HelperMethodFrameWithCookie frame(0); \
	uint64_t GCX[3] = { 0, 0, 0 };        \
	Runtime::CaptureState(frame);         \
	Runtime::ConstructFrame(frame);       \
	Runtime::SetPreemptive(GCX);          \

#define TRINITY_INTEROP_LEAVE_UNMANAGED() \
	Runtime::ClearPreemptive(GCX);        \
	Runtime::RestoreState(frame);         \
	Runtime::DeconstructFrame(frame);     \

//Toggle the flag to switch between frame construction and no frame construction
#elif defined(TRINITY_OPTIONAL_PREEMPTIVE)
    extern bool __transition_enabled;

#define TRINITY_INTEROP_ENTER_UNMANAGED() \
	Runtime::HelperMethodFrameWithCookie frame(0); \
	uint64_t GCX[3] = { 0, 0, 0 };        \
	if(Runtime::__transition_enabled)     \
        	{                                 \
	Runtime::CaptureState(frame);         \
	Runtime::ConstructFrame(frame);       \
	Runtime::SetPreemptive(GCX);          \
        	}                                      

#define TRINITY_INTEROP_LEAVE_UNMANAGED() \
	if(Runtime::__transition_enabled)     \
        	{                                 \
	Runtime::ClearPreemptive(GCX);        \
	Runtime::RestoreState(frame);         \
	Runtime::DeconstructFrame(frame);     \
        	}                                      

#elif defined(TRINITY_DISABLE_PREEMPTIVE)

#define TRINITY_INTEROP_ENTER_UNMANAGED() \

#define TRINITY_INTEROP_LEAVE_UNMANAGED() \

#endif

    }
