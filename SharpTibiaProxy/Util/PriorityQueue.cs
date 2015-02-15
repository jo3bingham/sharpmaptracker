using System;
using System.Collections.Generic;

namespace SharpTibiaProxy.Util
{

    [Serializable]
    public class PriorityQueue<T>
    {
        protected readonly List<T> _list = new List<T>();
        protected readonly Comparison<T> _comparer;

        public PriorityQueue()
            : this(Comparer<T>.Default.Compare)
        {
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            this._comparer = comparer.Compare;
        }

        public PriorityQueue(Comparison<T> comparer)
        {
            this._comparer = comparer;
        }


        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool Empty
        {
            get { return _list.Count == 0; }
        }


        public int Push(T element)
        {
            int p = _list.Count;
            _list.Add(element);
            do
            {
                if (p == 0)
                    break;
                int p2 = (p - 1) / 2;
                if (Compare(p, p2) < 0)
                {
                    SwitchElements(p, p2);
                    p = p2;
                }
                else
                    break;
            } while (true);
            return p;
        }

        public void PushAll(IEnumerable<T> elements)
        {
            foreach (var item in elements)
                Push(item);
        }

        public T Pop()
        {
            if (Empty)
                return default(T);

            int p = 0;
            T result = _list[0];
            _list[0] = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
            do
            {
                int pn = p;
                int p1 = 2 * p + 1;
                int p2 = 2 * p + 2;
                if (_list.Count > p1 && Compare(p, p1) > 0)
                    p = p1;
                if (_list.Count > p2 && Compare(p, p2) > 0)
                    p = p2;

                if (p == pn)
                    break;

                SwitchElements(p, pn);
            } while (true);

            return result;
        }

        public T Peek()
        {
            if (Empty)
                return default(T);

            return _list[0];
        }



        public void Clear()
        {
            _list.Clear();
        }

        public void Update(T element)
        {
            Update(_list.IndexOf(element));
        }

        public bool Contains(T element)
        {
            return _list.Contains(element);
        }


        int Compare(int i, int j)
        {
            return _comparer(_list[i], _list[j]);
        }

        void SwitchElements(int i, int j)
        {
            T h = _list[i];
            _list[i] = _list[j];
            _list[j] = h;
        }

        void Update(int i)
        {
            int p = i, pn;
            int p1, p2;
            do
            {
                if (p == 0)
                    break;
                p2 = (p - 1) / 2;
                if (Compare(p, p2) < 0)
                {
                    SwitchElements(p, p2);
                    p = p2;
                }
                else
                    break;
            } while (true);
            if (p < i)
                return;
            do
            {
                pn = p;
                p1 = 2 * p + 1;
                p2 = 2 * p + 2;
                if (_list.Count > p1 && Compare(p, p1) > 0)
                    p = p1;
                if (_list.Count > p2 && Compare(p, p2) > 0)
                    p = p2;

                if (p == pn)
                    break;
                SwitchElements(p, pn);
            } while (true);
        }
    }
}

