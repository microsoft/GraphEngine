using System.Collections.Generic;

namespace GraphEngine.DataImporter
{
    internal interface IGenerator<T>
    {
        void Scan(T content);
        IEnumerable<T> PreprocessInput(IEnumerable<string> input);
        void SetType(string type);
    }
}
