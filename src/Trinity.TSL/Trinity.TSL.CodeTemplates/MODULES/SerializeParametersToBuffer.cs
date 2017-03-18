using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.TSL;

namespace t_Namespace
{
    class SerializeParametersToBuffer : __meta
    {
        /**
         * The content are specified by a series of parameters (the input of a method);
         * This module generates code that evaluate the parameters and serialize them
         * into a buffer. For a cell, the code serializes to byte[] tmpcell; For a message
         * accessor, it serializes to an unmanaged buffer tmpcellptr;
         * Arguments:
         *  0. target       "cell" for cell target; "message" for message accessor.
         */
        public unsafe void serialize()
        {
            int preservedHeaderLength = 0;
            int BufferLength;

            MODULE_BEGIN();
            TARGET("NStructBase");
            META_VAR("bool", "forcell", "(context->m_arguments[0] == \"cell\")");

            byte* targetPtr;
            IF("%forcell");
            targetPtr = null;
            ELSE();
            targetPtr = (byte*)preservedHeaderLength;
            END();

            MODULE_CALL("PushPointerFromParameters", "node", "\"push\"");

            IF("%forcell");
            byte[] tmpcell = new byte[(int)(targetPtr)];
            fixed (byte* tmpcellptr = tmpcell)
            {
                targetPtr = tmpcellptr;
                MODULE_CALL("PushPointerFromParameters", "node", "\"assign\"");
            }
            ELSE();
            {
                BufferLength     = (int)targetPtr;
                byte* tmpcellptr = (byte*)Memory.malloc((ulong)targetPtr);
                Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
                targetPtr = tmpcellptr;
                tmpcellptr += preservedHeaderLength;
                targetPtr  += preservedHeaderLength;
                MODULE_CALL("PushPointerFromParameters", "node", "\"assign\"");
            }
            END();

            MODULE_END();
        }
    }
}
