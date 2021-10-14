using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core.Syntax
{
    public class Vector : Node, IList<object?>
    {
        private readonly List<object?> _items;

        public Vector()
        {
            _items = new List<object?>();
        }

        public Vector(IEnumerable<object?> nodes)
        {
            _items = nodes.ToList();
        }

        public IEnumerator<object?> GetEnumerator() => _items.GetEnumerator();

        public override string ToString() => $"[{string.Join(' ', _items)}]";

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public void Add(object? item) => _items.Add(item);

        public void Clear() => _items.Clear();

        public bool Contains(object? item) => _items.Contains(item);

        public void CopyTo(object?[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(object? item) => _items.Remove(item);

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public int IndexOf(object? item) => _items.IndexOf(item);

        public void Insert(int index, object? item) => _items.Insert(index, item);

        public void RemoveAt(int index) => _items.RemoveAt(index);

        public object? this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
    }
}
