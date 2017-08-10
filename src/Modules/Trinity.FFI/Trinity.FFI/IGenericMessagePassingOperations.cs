using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.FFI
{
    public interface IGenericMessagePassingOperations
    {
        TrinityErrorCode RegisterSynchronousHandler(int protocolId, SynchronousFFIHandler handler);
        TrinityErrorCode RegisterSynchronousHandler(int protocolId, AsynchronousFFIHandler handler);
    }
}
