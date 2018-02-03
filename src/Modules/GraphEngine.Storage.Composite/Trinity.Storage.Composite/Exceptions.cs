using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.Storage.CompositeExtension
{
    public class TSLCodeGenError : Exception
    {
        public TSLCodeGenError() : base() { }
        public TSLCodeGenError(string info) : base(info){ }
    }

    public class TSLBuildError : Exception
    {
        public TSLBuildError() : base() { }
        public TSLBuildError(string info) : base(info) { }
    }

    public class AsmLoadError : Exception
    {
        public AsmLoadError() : base() { }
        public AsmLoadError(string info) : base(info) { }

    }

    public class NotInitializedError : Exception
    {
        public NotInitializedError(): base(){}
        public NotInitializedError(string info) : base(info) { }

    }
}
