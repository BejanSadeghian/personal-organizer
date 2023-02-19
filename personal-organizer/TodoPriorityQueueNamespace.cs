using TodoItemNamespace;
using System.Collections;
using TodoItemNamespace;

namespace TodoPriorityQueueNamespace
{
    public class TodoPriorityQueue : IEnumerable<TodoItem>
    {
        private List<TodoItem> _items = new List<TodoItem>();

        public int Count => _items.Count;

        public void Enqueue(TodoItem item)
        {
            int index = _items.BinarySearch(item, new TodoComparer());
            if (index < 0)
            {
                index = ~index;
            }
            _items.Insert(index, item);
        }

        public TodoItem Dequeue()
        {
            TodoItem item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public TodoItem Peek(int position = 0)
        {
            return _items[position];
        }

        public void RemovePosition(int position)
        {
            _items.RemoveAt(position);
        }

        public void PrintValues(int startIndex=1)
        {
            int counter = startIndex;
            Console.WriteLine("\nCURRENT LIST:\n~~~~~~~");
            foreach (TodoItem item in _items)
            {
                Console.WriteLine($"{counter}. {item}");
                counter += 1;
            }
        }
        private class TodoComparer : IComparer<TodoItem>
        {
            public int Compare(TodoItem x, TodoItem y)
            {
                return x.Composite.CompareTo(y.Composite);
            }
        }

        public void Add(TodoItem item)
        {
            _items.Add(item);
            // Reorder the items based on Composite
            _items.Sort((x, y) => x.Composite.CompareTo(y.Composite));
        }

        public IEnumerator<TodoItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


}

