using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib
{
    public class Explorator<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private readonly HashSet<T> _set;

        public Explorator()
        {
            _set = new HashSet<T>();
            _queue = new Queue<T>();
        }

        public int QueueCount => _queue.Count;
        public int MemoryCount => _set.Count;

        public T Peek()
        {
            return _queue.Peek();
        }

        public T Explore()
        {
            T item = _queue.Dequeue();
            return item;
        }

        public void Add(T item)
        {
            if (!_set.Contains(item))
            {
                _queue.Enqueue(item);
                _set.Add(item);
            }
        }

        public bool Empty()
        {
            return _queue.Count == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
