#include "catch.hpp"
#include "Trinity/IO/Console.h"

#ifdef CATCH_CONFIG_NOSTDOUT

#include <ostream>
#include <streambuf>
#include "Trinity/String.h"
class ostream_adapter_t : public std::streambuf, public std::ostream
{
private:
    const size_t szbuf = 64;
    char* pbuf;

    void _reset_buf()
    {
        setp(pbuf, pbuf + szbuf - 1);
    }
public:
    ostream_adapter_t() : std::ostream(this)
    {
        pbuf = new char[szbuf + 1];
        pbuf[szbuf] = 0;
        _reset_buf();
    }

    std::streambuf::int_type overflow(std::streambuf::int_type c) override
    {
        pbuf[szbuf - 1] = c;
        Trinity::IO::Console::Write("{0}", Trinity::String(pbuf));
        _reset_buf();

        return 0;
    }

    int sync() override
    {
        *pptr() = 0;
        Trinity::IO::Console::WriteLine("{0}", Trinity::String(pbuf));
        _reset_buf();
        return 0;
    }
};

ostream_adapter_t ostream_trinity_console_adapter;

std::ostream& Catch::cout() { return ostream_trinity_console_adapter; }
std::ostream& Catch::cerr() { return ostream_trinity_console_adapter; }
std::ostream& Catch::clog() { return ostream_trinity_console_adapter; }

#endif