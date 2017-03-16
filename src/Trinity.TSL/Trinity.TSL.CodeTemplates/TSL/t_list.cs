using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using t_Namespace;

namespace Trinity.TSL
{
    public unsafe class t_list : __meta, IEnumerable<t_data_type>
    {
        public IEnumerator<t_data_type> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static implicit operator t_list_accessor(t_list x)
        {
            throw new NotFiniteNumberException();
        }

        internal void Add(t_accessor_type element)
        {
            throw new NotImplementedException();
        }
    }
}
