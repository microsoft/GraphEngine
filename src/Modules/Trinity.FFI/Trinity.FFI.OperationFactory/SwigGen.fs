module SwigGen

open GraphEngine.Jit.Verbs;

module Templates = 
    open System
    open Trinity.FFI.OperationFactory.PString;
   
        
    let to'templates (verb: Verb) =
        
        match verb with
        | LGet           -> 
            "
            static void (* _List_Get_{typesig})(void* dataSrc, int idx, {elem type} &elem) = {fn addr};
            static void List_Get_{typesig}(void* dataPtr, int idx, {elemType} &elem){{
                _List_Get_{typesig}(dataPtr, idx, elem);
            }}
            "

        | LSet           -> 
            "
            static void (* _Set_{private fn name})(void* dataSrc, int idx, {elem type} elem) = {fn addr};
            static void Set_{public fn name}(void* dataPtr, int idx, {elemType} elem){{
                {private fn name}(dataPtr, idx, elem);
            }}
            "

        | LInlineGet idx -> "
            void* {_}{list type name}{_}Get_at" + idx.ToString() + "(void* dataPtr, {element type} &elem) = {function pointer address};
            void {ListType}{_}Get_at" + idx.ToString() + "(void* dataPtr, {elemType} &elem){{
                {ListType}{_}Get_at"+ idx.ToString() + "(dataPtr, elem);
            }}
            "
        | LInlineSet idx -> "
            void* {_}{ListType}{_}Set_at" + idx.ToString() + "(void* dataPtr, {elemType} elem) = {funcPtrAddr};
            void {ListType}{_}Set_at" + idx.ToString() + "(void* dataPtr, {elemType} elem){{
                {_}{ListType}{_}Set_at" + idx.ToString() + "(dataPtr, elem);
            }}
           "
        | LContains      -> "
            void* {_}{ListType}{_}Contains(void* dataPtr, {elemType} elem) = {funcPtrAddr};
            void {ListType}{_}Contains(void* dataPtr, {elemType} elem){{
                {_}{ListType}{_}Contains(dataPtr, elem);
            }}
            "
        | LCount         -> "
            void* {_}{ListType}{_}Count(void* dataPtr) = {funcPtrAddr};
            void {ListType}{_}Count(void* dataPtr){{
                {_}{ListType}{_}Count(dataPtr);
            }}
            "
        | SGet fieldName -> "
            void* {_}{rootName}{_}Get{_}" + fieldName + "(void* dataPtr, {{fieldType}} &field) = {funcPtrAddr};
            void {rootName}{_}Get{_}"+ fieldName + "(void* dataPtr, {{fieldType}} &field){{
                {_}{rootName}{_}Get{_}" + fieldName + "(dataPtr, field);
            }}
            "
        
        | SSet fieldName -> "
            void* {_}{rootName}{_}Set{_}" + fieldName + "(void* dataPtr, {{fieldType}} field) = {funcPtrAddr};
            void {rootName}{_}Set{_}" + fieldName + "(void* dataPtr, {{fieldType}} field){{
                {_}{rootName}{_}Set{_}" + fieldName + "(dataPtr, field);
            }}
            "
        | BGet           -> "
            void * {_}{accessorType}{_}BasicGet(void* acc, {dataSrcType} &data) = {funcPtrAddr};
            void {accessorType}{_}BasicGet(void* acc, {dataSrcType} &data){{
                {_}{accessorType}{_}BasicGet(acc, data);
            }}
            "

        //| BSet          -> raise (NotImplementedException())

        | _             -> raise (NotImplementedException())
