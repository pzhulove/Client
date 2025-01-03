using System;
using System.Collections.Generic;


namespace PathFinder
{


    public class MaxHeap<T> 
        where T : class,IComparable<T>
    {

        private T[] _array = null;
        private int _count = 0;

        public MaxHeap(int capacity)
        {
            _array = new T[capacity];
        }

        public MaxHeap(T[] array)
        {
            _array = new T[array.Length];
            foreach (T item in array)
            {
                Add(item);
            }
        }

        public void Add(T item)
        {
            if (_array.Length <= _count)
            { 
                Array.Resize(ref _array, _count * 2);
            }

            _array[_count] = item;

            Up(_count++);
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(T item)
        {
            if (_array != null)
            {
                for(int index = 0;index < _count;++index)
                {
                    if (item== _array[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public T Pop()
        {
            if (_count == 0)
            {
                return default(T);
            }

            if (_count == 1)
            {
                _count = 0;
                return _array[0];
            }
    
            T pop = _array[0];


            Swap(0, --_count);
            Down(0);

            return pop;
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            for (int index = 0; index < _count; index++)
            {
                if (item== _array[index])
                {
                    Swap(index, --_count);
                    int newindex = this.Up(index);
                    if (newindex == index)
                    {
                        this.Down(index);
                    }
                    return true;
                }
            }
            return false;
        }

        public void RebuildElement(T item)
        {


            for(int index = 0;index < _count;++index){
                T other = _array[index];
                if(item== other)
                {
                    int newindex = this.Up(index);
                    if (newindex == index)
                    {
                        this.Down(index);
                        return;
                    }
                }
            }

        }

        private int Down(int index)
        {
            int left = 0;
            int right = 0;
            int side = 0;
            while(index * 2 + 1 < _count){
                left = index * 2 + 1;
                right = index * 2 + 2;

                side = left;
                if (right < _count)
                {
                    if (_array[right].CompareTo(_array[left]) > 0)
                    {
                        side = right;
                    }
                }

                if (_array[side].CompareTo(_array[index]) > 0)
                {
                    Swap(index, side);
                    index = side;

                }else{
                    break;
                }

            }
            return index;
        }

        private int Up(int index)
        {
            int parent = 0;
            while (index > 0)
            {
                parent = (index - 1) / 2;
                if (_array[index].CompareTo(_array[parent]) > 0)
                {
                    Swap(index, parent);
                    index = parent;
                    parent = (parent - 1) / 2;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

    
        private void Swap(int indexA, int indexB)
        {
            T  temp = _array[indexA];
            _array[indexA] = _array[indexB];
            _array[indexB] = temp;
        }


        private bool IsMax(int index)
        {
            if (index >= _count)
            {
                return true;
            }

            int left = index * 2 + 1;
            int right = index * 2 + 2;

            if (left < _count)
            {
                if (_array[left].CompareTo(_array[index]) > 0)
                {
                    return false;
                }
            }

            if (right < _count)
            {
                if (_array[right].CompareTo(_array[index]) > 0)
                {
                    return false;
                }
            }

            return IsMax(left) && IsMax(right);
        }

        // for test
        public bool IsValid()
        {
            return IsMax(0);
        }
    }
}