using System;
using System.Collections.Generic;

namespace MST
{
    /// <summary>
    /// Extends the standard list<> class to provide sorting capabilities.
    /// Specifically deviced for GraphNodes
    /// </summary>
    class SortedNodeList : List<GraphNode>
    {
        /// <summary>
        /// Adds the GraphNode in the list by insertion sort.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddSorted(GraphNode node)
        {
            int nCount = 0;
            foreach (GraphNode n in this)
            {
                nCount++;
                if (n.m_nID < node.m_nID)
                    continue;
                else if (n.m_nID > node.m_nID)
                {
                    nCount--;
                    break;
                }
            }

            this.Insert(nCount, node);
        }
        /// <summary>
        /// check if current list contains any node with given ID 
        /// </summary>
        /// <param name="nID">ID of the node.</param>
        /// <returns>true if found, false otherwise</returns>
        public bool Exists(UInt32 nID)
        {
            bool bRet = false;
            foreach (GraphNode e in this)
            {
                if (e.m_nID == nID)
                {
                    bRet = true;
                    break;
                }
            }
            return bRet;
        }
    }

    /// <summary>
    /// Extends the standard list<> class to provide sorting capabilities.
    /// Specifically deviced for GraphEdges
    /// </summary>
    class SortedEdgeList : List<GraphEdge>
    {
        // sorted on weight
        /// <summary>
        /// Adds the GraphEdge in the list by insertion sort.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public void AddSorted(GraphEdge edge)
        {
            int nCount = 0;
            foreach (GraphEdge n in this)
            {
                nCount++;
                if (n.m_dWeight < edge.m_dWeight)
                    continue;
                else if (n.m_dWeight > edge.m_dWeight)
                {
                    nCount--;
                    break;
                }
            }

            this.Insert(nCount, edge);
        }

        /// <summary>
        /// check if current list contains any node with given ID 
        /// </summary>
        /// <param name="nID">ID of the edge.</param>
        /// <returns>true if found, false otherwise</returns>
        public bool Exists(UInt32 nID)
        {
            bool bRet = false;
            foreach (GraphEdge e in this)
            {
                if (e.m_nID == nID)
                {
                    bRet = true;
                    break;
                }
            }
            return bRet;
        }
    }

    /// <summary>
    /// Data structure for storing connectivity matrix and hop counts after
    /// computing the MST.
    /// </summary>
    class PathListEntry
    {
        public UInt32 m_nFromNode;
        public UInt32 m_nToNode;
        public UInt32 m_nTraffic;
        public List<UInt32> m_listPath;

        public PathListEntry()
        {
            m_nFromNode = 0;
            m_nToNode = 0;
            m_nTraffic = 0;
            m_listPath = new List<uint>();
        }

        public override string ToString()
        {
            String s = m_nFromNode + " <-> " + m_nToNode +":: Traffic{"+ m_nTraffic+"} [";
            foreach (UInt32 u in m_listPath)
            {
                s += u + ", ";
            }
            s += "]";
            return s;
        }
        public string printforTask3()
        {
            String s = "  "+m_nFromNode.ToString("0#") + " | "
                       + m_nToNode.ToString("0#")
                       + " |    " + m_nTraffic + " "
                       + "  |   " + m_listPath.Count;
            return s;
        }
    }


}
