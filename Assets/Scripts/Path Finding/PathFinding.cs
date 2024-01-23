using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Path_Finding
{
    public class PathFinding : MonoBehaviour
    {
        public Transform player, target;
        public Grid grid;
        public Heap<Node> openSet;
        public HashSet<Node> closeSet;

        public virtual void Search()
        {
            
        }
    }
}
