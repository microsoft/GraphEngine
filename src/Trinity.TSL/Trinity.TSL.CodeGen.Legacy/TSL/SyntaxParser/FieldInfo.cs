using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal class FieldInfo
    {
        internal Field field;
        internal string src;
        internal FieldInfo(Field _field, string field_src)
        {
            this.field = _field;
            this.src = field_src;
        }
    }
}
