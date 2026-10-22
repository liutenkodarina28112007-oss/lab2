using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab
{
    /// <summary>
    /// Завдання 3-4. Узагальнений бінарний (двійковий пошуковий) тип.
    /// where T : class - елементами дерева не можуть бути структури.
    /// Порівняння елементів: через IComparer&lt;T&gt; (якщо передано) або IComparable&lt;T&gt;.
    /// Обхід (foreach) - центрований (inorder): лівий - корінь - правий.
    /// </summary>
    public class BinaryTree<T> : IEnumerable<T> where T : class
    {
        private class Node
        {
            public T Value;
            public Node Left;
            public Node Right;
            public Node(T value) { Value = value; }
        }

        private Node _root;
        private readonly IComparer<T> _comparer;

        public int Count { get; private set; }

        /// <summary>Порівняння за замовчуванням: T має реалізовувати IComparable&lt;T&gt;.</summary>
        public BinaryTree() : this(null) { }

        /// <summary>Порівняння через зовнішній IComparer&lt;T&gt;.</summary>
        public BinaryTree(IComparer<T> comparer)
        {
            if (comparer != null)
            {
                _comparer = comparer;
            }
            else
            {
                if (!typeof(IComparable<T>).IsAssignableFrom(typeof(T)) &&
                    !typeof(IComparable).IsAssignableFrom(typeof(T)))
                {
                    throw new InvalidOperationException(
                        $"Тип {typeof(T).Name} не реалізує IComparable<T>; передайте IComparer<T>.");
                }
                _comparer = Comparer<T>.Default;
            }
        }

        /// <summary>Додавання елемента. Дублікати (за компаратором) ігноруються.</summary>
        public bool Add(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (_root == null)
            {
                _root = new Node(value);
                Count++;
                return true;
            }

            Node current = _root;
            while (true)
            {
                int cmp = _comparer.Compare(value, current.Value);
                if (cmp == 0) return false;
                if (cmp < 0)
                {
                    if (current.Left == null) { current.Left = new Node(value); Count++; return true; }
                    current = current.Left;
                }
                else
                {
                    if (current.Right == null) { current.Right = new Node(value); Count++; return true; }
                    current = current.Right;
                }
            }
        }

        /// <summary>Пошук елемента.</summary>
        public bool Contains(T value)
        {
            if (value == null) return false;
            Node current = _root;
            while (current != null)
            {
                int cmp = _comparer.Compare(value, current.Value);
                if (cmp == 0) return true;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return false;
        }

        /// <summary>Видалення елемента.</summary>
        public bool Remove(T value)
        {
            if (value == null) return false;
            bool removed = false;
            _root = Remove(_root, value, ref removed);
            if (removed) Count--;
            return removed;
        }

        private Node Remove(Node node, T value, ref bool removed)
        {
            if (node == null) return null;
            int cmp = _comparer.Compare(value, node.Value);
            if (cmp < 0) node.Left = Remove(node.Left, value, ref removed);
            else if (cmp > 0) node.Right = Remove(node.Right, value, ref removed);
            else
            {
                removed = true;
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;
                // Два нащадки: замінюємо мінімальним елементом правого піддерева.
                Node min = node.Right;
                while (min.Left != null) min = min.Left;
                node.Value = min.Value;
                bool dummy = false;
                node.Right = Remove(node.Right, min.Value, ref dummy);
            }
            return node;
        }

        public void Clear() { _root = null; Count = 0; }

        // ---------- Власний ітератор (IEnumerable<T> + IEnumerator<T>) ----------

        public IEnumerator<T> GetEnumerator() => new InOrderEnumerator(_root);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Власний ітератор центрованого обходу (inorder) без рекурсії - на стеку.
        /// </summary>
        private class InOrderEnumerator : IEnumerator<T>
        {
            private readonly Node _root;
            private Stack<Node> _stack;
            private Node _currentNode;

            public InOrderEnumerator(Node root)
            {
                _root = root;
                Reset();
            }

            public T Current
            {
                get
                {
                    if (_currentNode == null)
                        throw new InvalidOperationException("Перебір не розпочато або вже завершено.");
                    return _currentNode.Value;
                }
            }

            object IEnumerator.Current => Current;

            private void PushLeft(Node node)
            {
                while (node != null)
                {
                    _stack.Push(node);
                    node = node.Left;
                }
            }

            public bool MoveNext()
            {
                if (_stack.Count == 0)
                {
                    _currentNode = null;
                    return false;
                }
                _currentNode = _stack.Pop();
                PushLeft(_currentNode.Right);
                return true;
            }

            public void Reset()
            {
                _stack = new Stack<Node>();
                _currentNode = null;
                PushLeft(_root);
            }

            public void Dispose() { _stack = null; }
        }
    }
}
