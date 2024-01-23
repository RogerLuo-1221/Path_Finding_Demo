using System;

namespace Utils
{
    public class Heap<T> where T : HeapItem<T>
    {
        private readonly T[] _items;
        
        public int Count { get; private set; }
        
        public Heap(int size)
        {
            _items = new T[size];
        }
        
        public bool Contains(T item)
        {
            return Equals(_items[item.HeapIndex], item);
        }
	
        public void Add(T item)
        {
            item.HeapIndex = Count;
            _items[Count] = item;
            SortUp(item);
            
            Count++;
        }
        
        public void Update(T item)
        {
            SortUp(item);
        }

        public T Pop()
        {
            var firstItem = _items[0];
            
            Count--;
            _items[0] = _items[Count];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            
            return firstItem;
        }
        
        private void SortUp(T item)
        {
            var parentIndex = (item.HeapIndex - 1) / 2;
		
            while (true)
            {
                var parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap (item,parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }
        
        private void SortDown(T item)
        {
            while (true)
            {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;

                if (childIndexLeft < Count)
                {
                    var swapIndex = childIndexLeft;

                    if (childIndexRight < Count)
                    {
                        if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) < 0)
                    {
                        Swap (item,_items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void Swap(T itemA, T itemB)
        {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            
            (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
        }
    }
    
    public abstract class HeapItem<T> : IComparable<T>
    {
        public abstract int HeapIndex { get; set; }
        
        public abstract int CompareTo(T other);
    }
}
