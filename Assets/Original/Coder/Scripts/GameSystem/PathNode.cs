using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Assembly.GameSystem.PathNetwork
{
  public class SearchNode
  {
    public PathNode target;
    public List<Route> routes;
    public SearchNode shotest;
    public bool closed;
    public float cost = Mathf.Infinity;
    public int queueIdx = -1;

    public SearchNode Cleared(List<Route> newRoutes)
    {
      routes = newRoutes;
      shotest = null;
      closed = false;
      cost = Mathf.Infinity;
      queueIdx = -1;
      return this;
    }

    public static bool operator <(SearchNode a, SearchNode b)
    {
      return a.cost < b.cost;
    }
    public static bool operator >(SearchNode a, SearchNode b)
    {
      return a.cost > b.cost;
    }
    public float deltaCost(SearchNode other)
    {
      return target.sqrDistance(other.target);
    }
  }

  public class PathNode : MonoBehaviour
  {
    public Dictionary<PathNode, SearchNode> searchNodes = new Dictionary<PathNode, SearchNode>();
    public List<Route> routes;
    public float sqrDistance(PathNode other)
    {
      return (transform.position - other.transform.position).sqrMagnitude;
    }
    public SearchNode GenSearchNode(PathNode start)
    {
      SearchNode result;
      if (searchNodes.TryGetValue(start, out result))
      {
        // if (result.closed) { return result.Cleared(routes); }
        return result;
      }
      return searchNodes[start] = new SearchNode
      {
        target = this,
        routes = this.routes,
      };
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
      Gizmos.DrawIcon(transform.position, "RedFlag", true);

      foreach (Route route in routes)
      {
        if (!route.dst) { continue; }
        GizmosEx.DrawArrow(transform.position, route.dst.transform.position);
      }
    }
#endif

  }
  [Serializable]
  public class Route
  {
    public PathNode dst;
  }
}