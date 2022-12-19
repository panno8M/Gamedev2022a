using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.GameSystem.PathNetwork
{
  public class PathNet : MonoBehaviour
  {
    List<PathNode> paths;
    [SerializeField] bool SolveOnAwake = true;
    [SerializeField] PathNode _source;
    [SerializeField] PathNode _destination;
    void Start()
    {
      paths = new List<PathNode>(GetComponentsInChildren<PathNode>());
      if (SolveOnAwake)
      {
        Solve(_source);

        var dst = _destination.searchNodes[_source];
        SearchNode dst0 = dst.shotest;
        while (dst0 != null)
        {
          Debug.Log($"{dst0.target.name} -> {dst.target.name} : {dst.cost - dst0.cost}");
          dst = dst0;
          dst0 = dst0.shotest;
        }
      }
    }

    void Solve(PathNode source)
    {
      PathPriorityQueue queue = new PathPriorityQueue(paths.Count);
      SearchNode ssource = source.GenSearchNode(source);

      ssource.cost = 0;
      queue.Push(ssource);

      while (queue.Count != 0)
      {
        SearchNode node = queue.Pop();
        node.closed = true;
        foreach (Route route in node.routes)
        {
          if (!route.dst) { continue; }
          SearchNode to = route.dst.GenSearchNode(source);
          if (to.closed) { continue; }
          float cost = node.cost + node.deltaCost(to);

          if (cost >= to.cost) { continue; }
          to.cost = cost;
          to.shotest = node;
          queue.Update(to);
        }
      }
    }
  }
}