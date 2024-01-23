using System;
using UnityEngine;
using Utils;

namespace Path_Finding
{
    public class Node : HeapItem<Node>
    {
        public bool walkable;
        public Vector3 worldPos;
        public int gridR;
        public int gridC;

        public float gCost;
        public float hCost;
        
        public Node parent;

        public Node(bool walkable, Vector3 worldPos, int gridR, int gridC)
        {
            this.walkable = walkable;   
            this.worldPos = worldPos;
            this.gridR = gridR;
            this.gridC = gridC;
        }

        private float fCost => gCost + hCost;

        public float DistanceTo(Node target)
        {
            var distanceX = Mathf.Abs(gridR - target.gridR);
            var distanceY = Mathf.Abs(gridC - target.gridC);

            return (distanceX > distanceY)
                ? 14.14f * distanceY + 10 * (distanceX - distanceY)
                : 14.14f * distanceX + 10 * (distanceY - distanceX);
        }
        
        public (int, int) DirectionTo(Node target)
        {
            var rDirection = Math.Clamp(target.gridR - gridR, -1, 1);
            var cDirection = Math.Clamp(target.gridC - gridC, -1, 1);
            
            return (rDirection, cDirection);
        }
    
        public override int HeapIndex { get; set; }

        public override int CompareTo(Node node)
        {
            var compare = fCost.CompareTo(node.fCost);

            if (compare == 0)
                compare = hCost.CompareTo(node.hCost);

            return -compare;
        }
    }
}
