using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Extension
{
    /// <summary>
    /// If a communication module is annotated with this attribute,
    /// it will be automatically registered when a communication instance
    /// is started.
    /// </summary>
    public class AutoRegisteredCommunicationModuleAttribute : Attribute { }
}
