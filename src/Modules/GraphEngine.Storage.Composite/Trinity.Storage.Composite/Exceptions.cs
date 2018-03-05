using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.Storage.Composite
{
    public class TSLCodeGenException : Exception
    {
        public TSLCodeGenException() : base() { }
        public TSLCodeGenException(string info) : base(info){ }
    }

    public class TSLBuildException : Exception
    {
        public TSLBuildException() : base() { }
        public TSLBuildException(string info) : base(info) { }
    }

    public class AsmLoadException : Exception
    {
        public AsmLoadException() : base() { }
        public AsmLoadException(string info) : base(info) { }

    }

    public class NotInitializedException : Exception
    {
        public NotInitializedException(): base(){}
        public NotInitializedException(string info) : base(info) { }

    }
}
