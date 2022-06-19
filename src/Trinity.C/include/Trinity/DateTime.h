// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "String.h"
#include <os/os.h>
#include <locale>
#include <ctime>

namespace Trinity
{
    class DateTime
    {
        tm m_time;
    public:

        DateTime(time_t time)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            localtime_s(&(this->m_time), &time);
#else
            tm* ptime = localtime(&time);
            m_time = *ptime;
#endif
        }

        static DateTime Now()
        {
            return DateTime(::time(NULL));
        }
        String Pad(int32_t value)
        {
            String text = String::Format("{0}", value);
            if (text.Length() == 1)
                return "0" + text;
            if (text.Length() == 0 || text.Length() > 2)
                return "00";
            return text;
        }
        String ToString()
        {
            //Default .NET DateTime.ToString format is:
            //11/24/2014 10:14:10 AM
            return String::Format("{0}/{1}/{2} {3}:{4}:{5} {6}", Pad(Month()), Pad(Day()), Year(), Pad(Hour12()), Pad(Minute()), Pad(Second()), IsAM() ? "AM" : "PM");
        }
        String ToStringForFilename()
        {
            return String::Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", Pad(Month()), Pad(Day()), Year(), Pad(Hour12()), Pad(Minute()), Pad(Second()), IsAM() ? "AM" : "PM");
        }
        inline int32_t Year()
        {
            return m_time.tm_year + 1900;
        }
        inline int32_t Month()
        {
            return m_time.tm_mon + 1;
        }
        inline int32_t Day()
        {
            return m_time.tm_mday;
        }
        inline int32_t Hour()
        {
            return m_time.tm_hour;
        }
        inline int32_t Hour12()
        {
            int32_t ret = m_time.tm_hour % 12;
            if (ret == 0)
                ret = 12;
            return ret;
        }
        inline int32_t Minute()
        {
            return m_time.tm_min;
        }
        inline int32_t Second()
        {
            return m_time.tm_sec;
        }
        inline bool IsAM()
        {
            return (m_time.tm_hour < 12);
        }
        // returns a UNIX-style timestamp.
        inline int64_t Timestamp()
        {
            return mktime(&m_time);
        }
    };
}
