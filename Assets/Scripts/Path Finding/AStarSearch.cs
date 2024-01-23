using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Path_Finding
{
    public class AStarSearch : MonoBehaviour
    {
        public Transform player, target;
        public Grid grid;

        private void Update()
        {
            FindPath(player.position, target.position);
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = grid.NodeFromWorldPos(startPos);
            var targetNode = grid.NodeFromWorldPos(targetPos);
            var openSet = new Heap<Node>(grid.rowCount * grid.colCount);
            var closeSet = new HashSet<Node>();

            var gridSet = new HashSet<Node>();
            
            openSet.Add(startNode);

            gridSet.Add(startNode);
            grid.openSet = gridSet;

            while (openSet.Count > 0)
            {
                var curNode = openSet.Pop();
                
                gridSet.Remove(curNode);
                grid.openSet = gridSet;
                
                closeSet.Add(curNode);
                grid.closeSet = closeSet;

                if (curNode == targetNode)
                {
                    grid.RetracePath(startNode, targetNode);
                    return;
                }

                foreach (var neighbor in grid.GetNeighbors(curNode))
                {
                    if (closeSet.Contains(neighbor)) continue;

                    var costToNeighbor = curNode.gCost + curNode.DistanceTo(neighbor);

                    if (!openSet.Contains(neighbor) || costToNeighbor < neighbor.gCost)
                    {
                        neighbor.gCost = costToNeighbor;
                        neighbor.hCost = neighbor.DistanceTo(targetNode);
                        neighbor.parent = curNode;
                        
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                            
                            gridSet.Add(neighbor);
                            grid.openSet = gridSet;
                        }
                    }
                }
            }
        }
    }
}
