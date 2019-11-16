#include <msclr\marshal_cppstd.h>
#include "Trinity.TSL.WrappedTokenList.h"
#include <Trinity/Array.h>
#include <Trinity/String.h>

int get_utf16_offset(System::String^ buffer_line, size_t octet_count)
{
    int idx = 0;

    for each(u16char u16_char in buffer_line)
    {
        uint8_t char_len;
        int32_t cp;

        if (System::Char::IsHighSurrogate(u16_char))
        {
            cp = System::Char::ConvertToUtf32(buffer_line, idx);
            ++idx;
        }
        else if (!System::Char::IsLowSurrogate(u16_char))
        {
            cp = u16_char;
        }
        else
        {
            continue;
        }

        // See: http://en.wikipedia.org/wiki/UTF-8#Description
        char_len = 1 + ((cp & 0x80) >> 6) + ((cp & 0x800) >> 10) + ((cp & 0x10000) >> 15) + ((cp & 0x200000) >> 20) + ((cp & 0x4000000) >> 30);

        if (octet_count < char_len)
        {
            break;
        }
        else
        {
            octet_count -= char_len;
        }

        ++idx;
    }

    if (idx == buffer_line->Length && idx)
        --idx;

    return idx;
}

Trinity::String ManagedStringToUTF8String(System::String^ buffer)
{
	Trinity::Array<u16char> wcharArray(buffer->Length);
	for (size_t i = 0; i < wcharArray.Length(); ++i)
		wcharArray[i] = buffer[i];
	return Trinity::String::FromWcharArray(wcharArray);
}

System::String^ UTF8StringToManagedString(const Trinity::String& str)
{
	auto buffer = str.ToWcharArray();
	System::Text::StringBuilder^ ret = gcnew System::Text::StringBuilder();
	for (auto wc: buffer)
		if (wc != 0)
			ret->Append(wc);
		else
			break;
	return ret->ToString();
}

Trinity::TSL::WrappedTokenList::WrappedTokenList(
    System::String^ buffer,
    System::Collections::Generic::List<System::String^>^ bufferLines,
    System::Collections::Generic::List<int>^ lineOffsets)
{
    auto utf8_buffer = ManagedStringToUTF8String(buffer);
    TokenList unmanagedList(utf8_buffer.c_str());
    this->tokens = gcnew System::Collections::Generic::List<WrappedTokenInfo^>();
    for (auto &token : unmanagedList.tokens)
    {
        auto loc = token.tokenLocation;
        /* locations in loc are 1-based indices.
           Converting them to C style offsets now.*/
        --loc.first_line;
        --loc.first_column;
        --loc.last_line;
        --loc.last_column;

        auto first_line_buf = bufferLines[loc.first_line];
        auto last_line_buf = bufferLines[loc.last_line];

        int first_offset = lineOffsets[loc.first_line] + get_utf16_offset(first_line_buf, loc.first_column);
        int second_offset = lineOffsets[loc.last_line] + get_utf16_offset(last_line_buf, loc.last_column);
        this->tokens->Add(gcnew WrappedTokenInfo(token.tokenType, first_offset, second_offset));
    }
}
