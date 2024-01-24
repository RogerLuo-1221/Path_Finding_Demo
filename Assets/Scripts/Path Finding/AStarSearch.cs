using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Path_Finding
{
    public class AStarSearch : MonoBehaviour
    {
        public Transform player, target;
        public Grid grid;

        private Node _startNode;
        private Node _targetNode;
        
        private void Update()
        {
            grid.GenerateGrid();
            FindPath(player.position, target.position);
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = grid.NodeFromWorldPos(startPos);
            var targetNode = grid.NodeFromWorldPos(targetPos);
            
            var openSet = new Heap<Node>(grid.rowCount * grid.colCount);
            var closeSet = new HashSet<Node>();
            
            var retraceOpenSet = new HashSet<Node>();
            var retraceCloseSet = new HashSet<Node>();
            
            openSet.Add(startNode);
            retraceOpenSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var curNode = openSet.Pop();
                
                closeSet.Add(curNode);

                retraceOpenSet.Remove(curNode);
                retraceCloseSet.Add(curNode);
                
                grid.openSet = retraceOpenSet;
                grid.closeSet = retraceCloseSet;

                if (curNode == targetNode)
                {
                    grid.RetracePath(startNode, targetNode);
                    
                    return;
                }

                foreach (var neighbor in GetNeighbors(curNode))
                {
                    if (closeSet.Contains(neighbor))
                        continue;

                    var costToNeighbor = curNode.gCost + curNode.DistanceTo(neighbor);

                    if (openSet.Contains(neighbor) && costToNeighbor >= neighbor.gCost) 
                        continue;
                    
                    neighbor.gCost = costToNeighbor;
                    neighbor.parent = curNode;
                        
                    if (!openSet.Contains(neighbor))
                    {
                        neighbor.hCost = neighbor.DistanceTo(targetNode);
                        
                        openSet.Add(neighbor);

                        retraceOpenSet.Add(neighbor);
                    }
                    else
                    {
                        openSet.Update(neighbor);
                    }
                }
            }
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            
            var r = node.gridR;
            var c = node.gridC;

            for (var rDirection = -1; rDirection <= 1; rDirection++)
            {
                for (var cDirection = -1; cDirection <= 1; cDirection++)
                {
                    if (rDirection != 0 && cDirection != 0)
                    {
                        if (grid.NodeWalkable(r + rDirection, c + cDirection) &&
                            (grid.NodeWalkable(r + rDirection, c) || grid.NodeWalkable(r, c + cDirection)))
                            neighbors.Add(grid.GetNode(r + rDirection, c + cDirection));
                    }
                    else if (rDirection != 0)
                    {
                        if (grid.NodeWalkable(r + rDirection, c))
                            neighbors.Add(grid.GetNode(r + rDirection, c));
                    }
                    else if (cDirection != 0)
                    {
                        if (grid.NodeWalkable(r, c + cDirection))
                            neighbors.Add(grid.GetNode(r, c + cDirection));
                    }
                }
            }

            return neighbors;
        }
    }
}
