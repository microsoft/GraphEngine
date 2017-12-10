/*MUTE*/
using System;
using System.Collections;
using System.Collections.Generic;
using Trinity.Core.Lib;
using Trinity.TSL;
/*MUTE_END*/

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NFieldType")]
    [MAP_VAR("t_array_name", "data_type_get_accessor_name(node)")]
    [MAP_VAR("t_accessor_type", "data_type_get_accessor_name(node->arrayInfo.arrayElement)")]
    [MAP_VAR("t_data_type", "node->arrayInfo.arrayElement")]
    [MAP_LIST("t_dim", "node->arrayInfo.array_dimension_size")]
    [MAP_VAR("t_dim_idx", "Discard($$) + GetString(GET_ITERATOR_VALUE())")]
    [MAP_VAR("t_int", "", MemberOf = "t_dim")]
    [MAP_VAR("t_int_2", "node->arrayInfo.array_dimension_size->size()")]
    [MAP_VAR("t_int_3", "node->arrayInfo.arrayElement->type_size()")]
    [MAP_VAR("t_int_4", "([&](int idx){int offset = 1;for(int i=idx + 1;i<node->arrayInfo.array_dimension_size->size();++i){offset *= (*(node->arrayInfo.array_dimension_size))[i];} return offset;})(GET_ITERATOR_VALUE())")]
    [MAP_VAR("t_iterator", "Discard($$) + \"iterator_\" + GetString(GET_ITERATOR_VALUE())", MemberOf = "t_dim")]
    public unsafe class t_array_name : __meta, IEnumerable<t_accessor_type>
    {
        [FOREACH]
        private static readonly int SizeDimt_dim_idx = t_int;
        [END]
        /// <summary>
        /// Gets the rank (number of dimentsions) of the Array.
        /// </summary>
        public static readonly int Rank = t_int_2;
        internal byte* CellPtr;
        internal long? CellID;
        internal t_array_name(byte* _CellPtr)
        {
            this.CellPtr = _CellPtr;
        }
        /// <summary>
        /// Gets the total number of elements
        /// </summary>
        public int Length
        {
            get
            {
                return
                    /*FOREACH("*")*/
                    SizeDimt_dim_idx
                    /*END*/
                    ;
            }
        }

        /// <summary>
        /// Returns a byte array that contains the value of this instance.
        /// </summary>
        /// <returns>a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[Length * t_int_3];
            fixed (byte* ptr = ret)
            {
                Memory.Copy(CellPtr, ptr, Length * t_int_3);
            }
            return ret;
        }

        [IF("data_type_need_accessor(node->arrayInfo.arrayElement)")]
        t_accessor_type elementAccessor;
        [END]
        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe t_accessor_type this[
            /*FOREACH(",")*/
            int indexDimt_dim_idx
            /*END*/
            ]
        {
            get
            {
                //[i_1,i_2,i_3,...,i_n]
                //i_n + i_n-1 * sd_n + i_n-2 * sd_n * sd_n-1 + ... + i_1 * s_2 * s_3 * ... * s_n

                byte* offset = CellPtr + 
                    /*FOREACH("+")*/
                    indexDimt_dim_idx * t_int_4
                    /*END*/
                    ;

                IF("data_type_need_accessor(node->arrayInfo.arrayElement)");
                {
                    elementAccessor.CellPtr = offset;
                    return elementAccessor;
                }
                ELSE();
                {
                    return *(t_accessor_type*)offset;
                }
                END();
            }
            set
            {
                byte* offset = CellPtr + 
                    /*FOREACH("+")*/
                    indexDimt_dim_idx * t_int_4
                    /*END*/
                    ;
                IF("data_type_need_accessor(node->arrayInfo.arrayElement)");
                {
                    if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                    Memory.Copy(value.CellPtr, offset, t_int_3);
                }
                ELSE();
                {
                    *(t_accessor_type*)offset = value;
                }
                END();
            }
        }
        #region Foreach
        /// <summary>
        /// Performs the specified action on each element
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates element in array</param>
        public unsafe void ForEach(Action<t_accessor_type> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + Length * t_int_3;
            while (targetPtr < endPtr)
            {
                IF("data_type_need_accessor(node->arrayInfo.arrayElement)");
                {
                    elementAccessor.CellPtr = targetPtr;
                    action(elementAccessor);
                    targetPtr += t_int_3;
                }
                ELSE();
                {
                    action(*(t_accessor_type*)targetPtr);
                    targetPtr += t_int_3;
                }
                END();
            }
        }

        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            t_array_name target;
            internal _iterator(t_array_name target)
            {
                targetPtr   = target.CellPtr;
                endPtr      = target.CellPtr + target.Length * t_int_3;
                this.target = target;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal t_accessor_type current()
            {
                IF("data_type_need_accessor(node->arrayInfo.arrayElement)");
                {
                    target.elementAccessor.CellPtr = targetPtr;
                    return target.elementAccessor;
                }
                ELSE();
                {
                    return *(t_accessor_type*)targetPtr;
                }
                END();
            }
            internal void move_next()
            {

                targetPtr += t_int_3;
            }
        }
        public IEnumerator<t_accessor_type> GetEnumerator()
        {
            _iterator _it = new _iterator(this);
            while (_it.good())
            {
                yield return _it.current();
                _it.move_next();
            }
        }
        unsafe IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Sets a range of elements in the Array to zero, to false, or to null, depending on the element type.
        /// </summary>
        /// <param name="index">The starting index of the range of elements to clear.</param>
        /// <param name="length">The number of elements to clear.</param>
        public unsafe void Clear(int index, int length)
        {
            if (index < 0 || length < 0 ||index >= Length || index+length > Length) throw new IndexOutOfRangeException();
            Memory.memset(CellPtr + index* t_int_3, 0, (ulong)(length * t_int_3));
        }

        public unsafe static implicit operator t_data_type[ /*FOREACH(",")*/ /*USE_LIST("t_dim")*/ /*END*/ ]
            (t_array_name accessor)
        {
            t_data_type[ /*FOREACH(",")*/ /*USE_LIST("t_dim")*/ /*END*/ ] ret 
                = new t_data_type[ /*FOREACH(",")*/ t_int /*END*/ ];

            IF("!data_type_need_accessor($t_data_type)");
            fixed (t_data_type* p = ret)
            {
                Memory.Copy(accessor.CellPtr, p, META_OUTPUT("node->type_size()"));
            }
            ELSE();

            t_accessor_type _element = new t_accessor_type(accessor.CellPtr);

            FOREACH();
            USE_LIST("t_dim");
            for (int t_iterator = 0; t_iterator < t_int; ++t_iterator)
            /*END*/
            {
                ret[
                    /*FOREACH(",")*/
                    t_iterator
                    /*END*/
                    ] = _element;
                _element.CellPtr += META_OUTPUT("node->type_size()"); ;
            }

            END();

            return ret;
        }

        [MODULE_CALL("AccessorReverseImplicitOperator", "node")]

        public static bool operator == (t_array_name a, t_array_name b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.Length * t_int_3);
        }

        public static bool operator != (t_array_name a,t_array_name b)
        {
            return !(a == b);
        }
    }
}
