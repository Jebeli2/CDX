namespace CDX.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class ObjectSet<T> : IEnumerable<T> where T : class
    {
        private int size;
        private T?[] keyTable;
        private float loadFactor;
        private int threshold;
        protected int shift;
        protected int mask;


        public ObjectSet(int initialCapacity = 51, float loadFactor = 0.8f)
        {
            this.loadFactor = loadFactor;
            int tableSize = TableSize(initialCapacity, loadFactor);
            threshold = (int)(tableSize * loadFactor);
            mask = tableSize - 1;
            shift = BitOperations.LeadingZeroCount((uint)mask);
            keyTable = new T[tableSize];
        }

        protected int Place(T item)
        {
            return (int)((ulong)item!.GetHashCode() * 0x9E3779B97F4A7C15L >> shift);
        }

        private int LocateKey(T key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            T?[] keyTable = this.keyTable;
            for (int i = Place(key); ; i = i + 1 & mask)
            {
                T? other = keyTable[i];
                if (other == null) return -(i + 1);
                if (other.Equals(key)) return i;
            }
        }

        public bool Add(T key)
        {
            int i = LocateKey(key);
            if (i >= 0) return false;
            i = -(i + 1);
            keyTable[i] = key;
            if (++size >= threshold) Resize(keyTable.Length << 1);
            return true;
        }

        public bool AddRange(T[] array)
        {
            return AddRange(array, 0, array.Length);
        }
        public bool AddRange(T[] array, int offset, int length)
        {
            EnsureCapacity(length);
            int oldSize = size;
            for (int i = offset, n = i + length; i < n; i++)
            {
                Add(array[i]);
            }
            return oldSize != size;
        }

        public bool Remove(T key)
        {
            int i = LocateKey(key);
            if (i < 0) return false;
            T?[] keyTable = this.keyTable;
            int mask = this.mask;
            int next = i + 1 & mask;
            T? kkey;
            while ((kkey = keyTable[next]) != null)
            {
                int placement = Place(kkey);
                if ((next - placement & mask) > (i - placement & mask))
                {
                    keyTable[i] = kkey;
                    i = next;
                }
                next = next + 1 & mask;
            }
            keyTable[i] = null;
            size--;
            return true;
        }

        public int Count => size;
        public bool IsEmpty => size == 0;

        public void Shrink(int maximumCapacity)
        {
            if (maximumCapacity < 0) throw new ArgumentOutOfRangeException(nameof(maximumCapacity), maximumCapacity, "must be >= 0");
            int tableSize = TableSize(maximumCapacity, loadFactor);
            if (keyTable.Length > tableSize) Resize(tableSize);
        }

        public void Clear(int maximumCapacity)
        {
            int tableSize = TableSize(maximumCapacity, loadFactor);
            if (keyTable.Length <= tableSize)
            {
                Clear();
                return;
            }
            size = 0;
            Resize(tableSize);
        }

        public void Clear()
        {
            if (size == 0) return;
            size = 0;
            Array.Fill(keyTable, null);
        }

        public bool Contains(T key)
        {
            return LocateKey(key) >= 0;
        }

        public T? this[T key]
        {
            get
            {
                int i = LocateKey(key);
                return i < 0 ? null : keyTable[i];
            }
        }

        public void EnsureCapacity(int additionalCapacity)
        {
            int tableSize = TableSize(size + additionalCapacity, loadFactor);
            if (keyTable.Length < tableSize) Resize(tableSize);
        }


        private void Resize(int newSize)
        {
            int oldCapacity = keyTable.Length;
            threshold = (int)(newSize * loadFactor);
            mask = newSize - 1;
            shift = BitOperations.LeadingZeroCount((uint)mask);
            T?[] oldKeyTable = keyTable;
            keyTable = new T[newSize];
            if (size > 0)
            {
                for (int i = 0; i < oldCapacity; i++)
                {
                    T? key = oldKeyTable[i];
                    if (key != null) AddResize(key);
                }
            }
        }

        private void AddResize(T key)
        {
            T?[] keyTable = this.keyTable;
            for (int i = Place(key); ; i = (i + 1) & mask)
            {
                if (keyTable[i] == null)
                {
                    keyTable[i] = key;
                    return;
                }
            }
        }
        private static int TableSize(int capacity, float loadFactor)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "must be >= 0");
            int tableSize = (int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(2, (int)Math.Ceiling(capacity / loadFactor)));
            if (tableSize > 1 << 30) throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "is too large");
            return tableSize;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ObjectSetEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ObjectSetEnumerator(this);
        }

        private class ObjectSetEnumerator : IEnumerator<T>
        {
            private readonly ObjectSet<T> _set;
            private int nextIndex;
            private int currentIndex;
            private bool valid = true;
            private bool hasNext;
            private T? current;
            public ObjectSetEnumerator(ObjectSet<T> _set)
            {
                this._set = _set;
                Reset();
            }
            public T Current => current!;

            object IEnumerator.Current => current!;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (!hasNext) return false;
                if (!valid) return false;
                T? key = _set.keyTable[nextIndex];
                currentIndex = nextIndex;
                current = key;
                FindNextIndex();
                return true;
            }

            public void Reset()
            {
                currentIndex = -1;
                nextIndex = -1;
                FindNextIndex();
            }

            private void FindNextIndex()
            {
                T?[] keyTable = _set.keyTable;
                for (int n = _set.keyTable.Length; ++nextIndex < n;)
                {
                    if (keyTable[nextIndex] != null)
                    {
                        hasNext = true;
                        return;
                    }
                }
                hasNext = false;
            }
        }
    }
}
