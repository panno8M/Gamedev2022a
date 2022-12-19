using System;
using System.Collections;
using System.Collections.Generic;

namespace Assembly.GameSystem.PathNetwork
{
  class PathPriorityQueue
  {
    private readonly SearchNode[] _array;
    private readonly IComparer _comparer;
    public int Count { get; private set; } = 0;

    public PathPriorityQueue(int capacity, IComparer comparer = null)
    {
      _array = new SearchNode[capacity];
      _comparer = comparer;
    }

    public void Update(SearchNode item)
    {
      if (Push(item)) { return; }
      if (GoUp(item)) { return; }
      if (GoDown(item)) { return; }

    }
    bool GoUp(SearchNode item)
    {
      int n = item.queueIdx;
      int i = n;
      while (n != 0)
      {
        int parent = (n - 1) / 2;

        if (_array[n] > _array[parent])
        {
          Swap(n, parent);
          n = parent;
        }
        else { break; }
      }
      return i != n;
    }
    bool GoDown(SearchNode item)
    {
      int i = item.queueIdx;
      int parent = item.queueIdx;
      while (true)
      {
        int child = 2 * parent + 1;
        if (child > Count - 1) { break; }

        if (child < Count - 1 && (_array[child] < _array[child + 1])) child += 1;

        if (_array[parent] < _array[child])
        {
          Swap(parent, child);
          parent = child;
        }
        else { break; }
      }
      return i != parent;
    }

    public bool Push(SearchNode item)
    {
      if (item.queueIdx != -1) { return false; }

      item.queueIdx = this.Count;
      _array[this.Count++] = item;

      GoUp(item);

      return true;
    }

    public SearchNode Pop()
    {
      Swap(0, --this.Count);

      GoDown(_array[0]);

      _array[Count].queueIdx = -1;
      return _array[Count];
    }

    public IEnumerable<SearchNode> PeekAll()
    {
      for (int i = 1; i <= Count; i++)
      { yield return _array[Count - i]; }
    }

    public IEnumerable<SearchNode> PopAll()
    {
      for (int i = 0; i < Count; i++)
      { yield return Pop(); }
    }

    private void Swap(int a, int b)
    {
      SearchNode tmp = _array[a];
      _array[a] = _array[b];
      _array[b] = tmp;
      _array[a].queueIdx = b;
      _array[b].queueIdx = a;
    }
  }
}