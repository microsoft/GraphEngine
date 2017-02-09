using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.DataImporter
{
    public class ImporterException : Exception
    {
        public ImporterException(string fmt, params object[] args) :
            base(string.Format(fmt, args)) { }
    }
}
