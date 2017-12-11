using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Trinity.DynamicCluster.Persistency;

namespace Trinity.Azure.Storage
{
    internal unsafe class PersistedCellEnumerator : IEnumerator<PersistedCell>
    {
        private byte[] content;
        private int length;
        private long target_lowKey;
        private long target_highKey;
        private SmartPointer sp;
        private byte* bp;
        private byte* ep;
        private GCHandle handle;

        public PersistedCellEnumerator(byte[] content, long target_lowKey, long target_highKey)
        {
            this.content=content;
            this.length=content.Length;
            this.target_lowKey=target_lowKey;
            this.target_highKey=target_highKey;
            this.handle = GCHandle.Alloc(content, GCHandleType.Pinned);
            this.bp = (byte*)handle.AddrOfPinnedObject().ToPointer();
            sp = SmartPointer.New(bp);
            ep = bp + length;
        }

        public PersistedCell Current => new PersistedCell
        {
            Buffer = content,
            CellId = *sp.lp,
            CellType = *(ushort*)(sp.bp + sizeof(long)),
            Length = *(int*)(sp.bp + sizeof(long)+sizeof(ushort)),
            Offset = (int)(sp.bp + sizeof(long) + sizeof(ushort) + sizeof(int) - bp)
        };

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            handle.Free();
        }

        public bool MoveNext()
        {
            if (sp.bp >= ep) return false;
            sp.lp++;
            sp.sp++;
            sp.bp += *sp.ip + sizeof(int);
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}