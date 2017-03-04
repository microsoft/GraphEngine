using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    class OptionalFieldCalculator
    {
        public OptionalFieldCalculator(int fieldCount)
        {
            this.fieldCount = fieldCount;
            this.headerLength = (fieldCount + 0x07) >> 3;
        }
        public string GenerateMaskOnCode(int fieldSequence, string pointerName)
        {
            int offset = fieldSequence / 8;
            fieldSequence %= 8;
            string mask = string.Format(CultureInfo.InvariantCulture, "0x{0:X2}", (byte)((1 << fieldSequence)));
            return @"
            *(" + pointerName + "+" + offset + ") |= " + mask + @";
";
        }
        public string GenerateMaskOffCode(int fieldSequence, string pointerName)
        {
            int offset = fieldSequence / 8;
            fieldSequence %= 8;
            string mask = string.Format(CultureInfo.InvariantCulture, "0x{0:X2}", (byte)(~(1 << fieldSequence)));
            return @"
            *(" + pointerName + "+" + offset + ") &= " + mask + @";
";
        }
        public string GenerateReadBitExpression(int fieldSequence, string pointerName)
        {
            int offset = fieldSequence / 8;
            fieldSequence %= 8;
            string mask = string.Format(CultureInfo.InvariantCulture, "0x{0:X2}", (byte)((1 << fieldSequence)));
            return @"
            (0 != (*(" + pointerName + "+" + offset + ") & " + mask + @"))
";
        }
        public string GenerateClearAllBitsCode(string pointerName)
        {
            string ret = @"";
            for (int i = 0; i < headerLength; ++i)
            {
                ret += @"
            *(" + pointerName + @"+" + i + @") = 0x00;
";
            }
            return ret;
        }
        public int fieldCount;
        public int headerLength;
    }
}
