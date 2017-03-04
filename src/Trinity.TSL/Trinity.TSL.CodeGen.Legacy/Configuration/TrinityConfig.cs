using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace Trinity
{
    /// <summary>
    /// Specifies the running mode supported by Trinity.
    /// </summary>
    internal enum RunningMode : int
    {
        /// <summary>
        /// Undefined running mode.
        /// </summary>
        Undefined,

        /// <summary>
        /// Current process runs in embedded mode.
        /// </summary>
        Embedded,

        /// <summary>
        /// Current process runs as a Trinity server.
        /// </summary>
        Server,

        /// <summary>
        /// Current process runs as a Trinity proxy.
        /// </summary>
        Proxy,

        /// <summary>
        /// Current process runs as a Trinity client.
        /// </summary>
        Client,
    }
}