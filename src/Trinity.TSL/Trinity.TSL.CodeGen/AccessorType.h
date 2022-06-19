#pragma once

namespace Trinity
{
    namespace Codegen
    {
        // Represent the role of an accessor.
        enum AccessorType
        {
            AT_STRUCT_FIELD,
            AT_CELL_FIELD,
            AT_LIST_ELEMENT,
            AT_ARRAY_ELEMENT
        };
    }
}
