using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Path_Finding
{
    public class JPS : MonoBehaviour
    {
        public Transform player, target;
        public Grid grid;

        private Node _startNode;
        private Node _targetNode;
        
        private void Update()
        {
            FindPath(player.position, target.position);
        }
        
        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            _startNode = grid.NodeFromWorldPos(startPos);
            _targetNode = grid.NodeFromWorldPos(targetPos);
            var openSet = new Heap<Node>(grid.rowCount * grid.colCount);
            var closeSet = new HashSet<Node>();

            var open = new HashSet<Node>();
            
            openSet.Add(_startNode);
            open.Add(_startNode);

            while (openSet.Count > 0)
            {
                grid.openSet = open;
                
                var curNode = openSet.Pop();
                closeSet.Add(curNode);
                
                if (curNode == _targetNode)
                {
                    grid.RetracePath(_startNode, _targetNode);
                    return;
                }
                
                foreach (var node in GetNeighbors(curNode))
                {
                    var jumpPoint = GetJumpPoint(node.gridR, node.gridC, curNode.DirectionTo(node));
                    
                    if (jumpPoint == null || closeSet.Contains(jumpPoint)) continue;
                    
                    var costToJumpPoint = curNode.gCost + curNode.DistanceTo(jumpPoint);

                    if (!openSet.Contains(jumpPoint) || costToJumpPoint < jumpPoint.gCost)
                    {
                        jumpPoint.gCost = costToJumpPoint;
                        jumpPoint.parent = curNode;

                        if (!openSet.Contains(jumpPoint))
                        {
                            jumpPoint.hCost = jumpPoint.DistanceTo(_targetNode);
                            
                            openSet.Add(jumpPoint);
                            open.Add(jumpPoint);
                        }
                        else
                        {
                            openSet.Update(jumpPoint);
                        }
                    }
                }
            }
            grid.closeSet = closeSet;
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            var r = node.gridR;
            var c = node.gridC;
            
            if (node.parent == null)
            {
                neighbors = grid.GetNeighbors(node);
            }
            else
            {
                var (rDirection, cDirection) = node.parent.DirectionTo(node);

                if (rDirection != 0 & cDirection != 0)
                {
                    if (grid.NodeWalkable(r + rDirection, c)) 
                        neighbors.Add(grid.GetNode(r + rDirection, c));
                    
                    if (grid.NodeWalkable(r, c + cDirection))
                        neighbors.Add(grid.GetNode(r, c + cDirection));
                    
                    if ((grid.NodeWalkable(r + rDirection, c) || grid.NodeWalkable(r, c + cDirection))
                        && grid.NodeWalkable(r + rDirection, c + cDirection))
                        neighbors.Add(grid.GetNode(r + rDirection, c + cDirection));
                    
                    if (grid.NodeWalkable(r + rDirection, c - cDirection) &&
                        !grid.NodeWalkable(r, c - cDirection) &&
                        grid.NodeWalkable(r + rDirection, c))
                        neighbors.Add(grid.GetNode(r + rDirection, c - cDirection));

                    if (grid.NodeWalkable(r - rDirection, c + cDirection) &&
                        !grid.NodeWalkable(r - rDirection, c) &&
                        grid.NodeWalkable(r, c + cDirection))
                        neighbors.Add(grid.GetNode(r - rDirection, c + cDirection));
                }
                else if (rDirection != 0)
                {
                    if (grid.NodeWalkable(r + rDirection, c))
                        neighbors.Add(grid.GetNode(r + rDirection, c));
                    
                    if (grid.NodeWalkable(r + rDirection, c + 1) && !grid.NodeWalkable(r, c + 1))
                    {
                        neighbors.Add(grid.GetNode(r + rDirection, c + 1));
                    }

                    if (grid.NodeWalkable(r + rDirection, c - 1) && !grid.NodeWalkable(r, c - 1))
                    {
                        neighbors.Add(grid.GetNode(r + rDirection, c - 1));
                    }
                }
                else if (cDirection != 0)
                {
                    if (grid.NodeWalkable(r, c + cDirection))
                        neighbors.Add(grid.GetNode(r, c + cDirection));

                    if (grid.NodeWalkable(r + 1, c + cDirection) && !grid.NodeWalkable(r + 1, c))
                    {
                        neighbors.Add(grid.GetNode(r + 1, c + cDirection));
                    }

                    if (grid.NodeWalkable(r - 1, c + cDirection) && !grid.NodeWalkable(r - 1, c))
                    {
                        neighbors.Add(grid.GetNode(r - 1, c + cDirection));
                    }
                }
            }

            return neighbors;
        }
        
        private Node GetJumpPoint(int r, int c, (int, int) direction)
        {
            if (!grid.NodeWalkable(r, c))
                return null;
            
            if (r == _targetNode.gridR && c == _targetNode.gridC)
                return _targetNode;
            
            var (rDirection, cDirection) = direction;

            if (rDirection != 0 && cDirection != 0)
            {
                if (!grid.NodeWalkable(r + rDirection, c) && !grid.NodeWalkable(r, c + cDirection)) return null;
                
                if (grid.NodeWalkable(r + rDirection, c - cDirection) && !grid.NodeWalkable(r, c - cDirection) ||
                    grid.NodeWalkable(r - rDirection, c + cDirection) && !grid.NodeWalkable(r - rDirection, c))
                {
                    return grid.GetNode(r, c);
                }

                if (GetJumpPoint(r + rDirection, c, (rDirection, 0)) != null)
                {
                    return grid.GetNode(r, c);
                }

                if (GetJumpPoint(r, c + cDirection, (0, cDirection)) != null)
                {
                    return grid.GetNode(r, c);
                }
            }
            else if (rDirection != 0)
            {
                if (!grid.NodeWalkable(r + rDirection, c)) return null;
                
                if (grid.NodeWalkable(r + rDirection, c + 1) && !grid.NodeWalkable(r, c + 1) ||
                    grid.NodeWalkable(r + rDirection, c - 1) && !grid.NodeWalkable(r, c - 1))
                {
                    return grid.GetNode(r, c);
                }
            }
            else if (cDirection != 0)
            {
                if (!grid.NodeWalkable(r, c + cDirection)) return null;
                
                if (grid.NodeWalkable(r + 1, c + cDirection) && !grid.NodeWalkable(r + 1, c) || 
                    grid.NodeWalkable(r - 1, c + cDirection) && !grid.NodeWalkable(r - 1, c))
                {
                    return grid.GetNode(r, c);
                }
            }
            
            return GetJumpPoint(r + rDirection, c + cDirection, direction);
        }
    }
}
