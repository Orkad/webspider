using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib
{
    public class UniqueQueue<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private readonly HashSet<T> _set;

        private readonly bool _memory;

        public UniqueQueue(bool memory)
        {
            _memory = memory;
            _set = new HashSet<T>();
            _queue = new Queue<T>();
        }

        public int QueueCount => _queue.Count;
        public int MemoryCount => _set.Count;

        public T Peek()
        {
            return _queue.Peek();
        }

        public T Dequeue()
        {
            if (!_memory)
                _set.Remove(_queue.Peek());
            return _queue.Dequeue();
        }

        public void Enqueue(T item)
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
