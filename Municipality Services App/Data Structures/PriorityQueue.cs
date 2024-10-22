using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality_Services_App.Data_Structures
{
    /// <summary>
    /// fully chatGPT implemented PriorityQueue, i understand what its doing and it seems pretty standard based on other sources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        // A sorted dictionary where the key is the priority, and the value is a queue of items
        private SortedDictionary<int, Queue<T>> _queue = new SortedDictionary<int, Queue<T>>();

        // Adds an item with a given priority
        public void Enqueue(T item, int priority)
        {
            if (!_queue.ContainsKey(priority))
            {
                // If no queue exists for the given priority, create one
                _queue[priority] = new Queue<T>();
            }
            _queue[priority].Enqueue(item); // Add the item to the queue for that priority
        }

        // Removes and returns the item with the highest priority (lowest key)
        public T Dequeue()
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            // Get the queue with the lowest priority (smallest key)
            var firstKey = GetFirstKey();
            var item = _queue[firstKey].Dequeue();

            // If the queue for this priority is empty, remove the key from the dictionary
            if (_queue[firstKey].Count == 0)
            {
                _queue.Remove(firstKey);
            }

            return item;
        }

        // Returns the item with the highest priority without removing it
        public T Peek()
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            var firstKey = GetFirstKey();
            return _queue[firstKey].Peek();
        }

        // Helper method to get the first key (highest priority)
        private int GetFirstKey()
        {
            using (var enumerator = _queue.Keys.GetEnumerator())
            {
                enumerator.MoveNext();
                return enumerator.Current;
            }
        }

        // Checks if the priority queue is empty
        public bool IsEmpty()
        {
            return _queue.Count == 0;
        }
    }
}
