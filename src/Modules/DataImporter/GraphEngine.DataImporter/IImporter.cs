using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace GraphEngine.DataImporter
{
    public interface IImporter
    {
        bool IgnoreType();
    }
    public interface IImporter<T> : IImporter
    {
        /// <summary>
        /// Note that ImportEntity may import more than one entity. This would happen
        /// with a TreeImport which hierarchically import the childrens of the current entity.
        /// </summary>
        /// <param name="type">The type of the entity, should match a type name defined in TSL, otherwise an exception will be thrown.</param>
        /// <param name="content">The content of the entity.</param>
        /// <param name="parent_id">Optionally specifies the parent in a tree structure for the current entity.</param>
        /// <returns>
        /// The imported entity, returned for reference and inspection.
        /// </returns>
        /// <remarks>
        /// ImportEntity should guarantee that the entities extracted from the content line should be saved into storage before returning the root object.
        /// Saving the entity again with the returned ICell is unnecessary, and would cause data being overwritten.
        /// </remarks>
        ICell ImportEntity(string type, T content, long? parent_id = null);
        IEnumerable<T> PreprocessInput(IEnumerable<string> input);
    }
}
