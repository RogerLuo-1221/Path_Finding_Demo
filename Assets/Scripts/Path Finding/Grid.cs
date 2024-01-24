using System.Collections.Generic;
using UnityEngine;

namespace Path_Finding
{
    public class Grid : MonoBehaviour
    {
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public Vector2 nodeSize;
        public int rowCount;
        public int colCount;

        private Node[,] _grid;
        
        private void Start()
        {
            rowCount = Mathf.RoundToInt(gridWorldSize.x / nodeSize.x);
            colCount = Mathf.RoundToInt(gridWorldSize.y / nodeSize.y);
        }

        public Node GetNode(int r, int c)
        {
            return NodeInBoundary(r, c) ? _grid[r, c] : null;
        }

        public void GenerateGrid()
        {
            _grid = new Node[rowCount, colCount];
            var gridBottomLeft = transform.position 
                                 + Vector3.left * gridWorldSize.x / 2 
                                 + Vector3.back * gridWorldSize.y / 2;

            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < colCount; c++)
                {
                    var nodePos = gridBottomLeft
                                  + Vector3.right * (nodeSize.x * (r + 0.5f))
                                  + Vector3.forward * (nodeSize.y * (c + 0.5f));
                    var flagWalkable = !(Physics.CheckSphere(nodePos, nodeSize.x / 2.1f, unwalkableMask));

                    _grid[r, c] = new Node(flagWalkable, nodePos, r, c);
                }
            }
        }

        public Node NodeFromWorldPos(Vector3 pos)
        {
            var percentR = Mathf.Clamp01((gridWorldSize.x / 2 + pos.x) / gridWorldSize.x);
            var percentC = Mathf.Clamp01((gridWorldSize.y / 2 + pos.z) / gridWorldSize.y);

            var posR = Mathf.RoundToInt((rowCount - 1) * percentR);
            var posC = Mathf.RoundToInt((colCount - 1) * percentC);

            return _grid[posR, posC];
        }

        public bool NodeWalkable(int r, int c) => NodeInBoundary(r, c) && _grid[r, c].walkable;

        private bool NodeInBoundary(int r, int c) => r >= 0 && r < rowCount && c >= 0 && c < colCount;

        public List<Node> pathRetrace;
        public HashSet<Node> openSet;
        public HashSet<Node> closeSet;

        public void RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            
            var curNode = endNode;

            while (curNode != startNode)
            {
                Debug.DrawLine(curNode.worldPos, curNode.parent.worldPos, Color.red);
                path.Add(curNode);
                curNode = curNode.parent;
            }

            path.Add(startNode);
            path.Reverse();

            pathRetrace = path;
        }
        
        private void OnDrawGizmos() 
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (_grid == null) return;

            foreach (var node in _grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;

                if (openSet != null)
                {
                    if (openSet.Contains(node)) Gizmos.color = Color.cyan;
                }

                if (closeSet != null)
                {
                    if (closeSet.Contains(node)) Gizmos.color = Color.yellow;
                }
                
                if (pathRetrace != null)
                {
                    if (pathRetrace.Contains(node)) Gizmos.color = Color.green;
                }
                
                Gizmos.DrawCube(node.worldPos, new Vector3(nodeSize.x, 0.02f, nodeSize.y) * 0.9f);
            }
        }
    }
}
