using System;

// TODO: use the template based List<>

namespace MST
{
    /// <summary>
    /// data-struct for storing edges in the graph.
    /// </summary>
    class GraphEdge
    {
        // ID of the edge=> better searchability
        public UInt32 m_nID;
        // Connects Node1 or From Node
        //public UInt32 m_nFromNodeID;
        public GraphNode m_nodeFrom;
        // Connects Node2 or To Node 
        //public UInt32 m_nToNodeID;
        public GraphNode m_nodeTo;
        // weight of the node
        // in our case its the distance
        public Double m_dWeight;
        // traffic in Kbps
        public UInt32 m_nTraffic;

        // constructor
        public GraphEdge(UInt32 id, GraphNode from, GraphNode to, UInt32 traffic)
        {
            m_nID = id;
            m_nodeFrom = from;
            m_nodeTo = to;
            m_nTraffic = traffic;
            m_dWeight = Util.calcEuclidianDistance(from, to);
        }
        public override string ToString()
        {
            String s =  "[ID:" + m_nID.ToString("0#") + "]";
            s += "[" + m_nodeFrom.m_nID.ToString("0#") + "," + m_nodeTo.m_nID.ToString("0#") + "] = ";
            s += "{ Weight=" + m_dWeight.ToString("f") + "}";
            s += " {Traffic = " + m_nTraffic + "}";
            return s;
        }
    }

    /// <summary>
    /// data-struct for storing nodes in the graph.
    /// </summary>
    class GraphNode
    {
        // ID of the graph node.
        public UInt32 m_nID;
        // X location of graph node.
        public Int32 m_nX;
        // Y location of graph node.
        public Int32 m_nY;
        // list of edges from current node.
        public SortedEdgeList m_listEdge;

        // constructor
        public GraphNode(UInt32 id, Int32 x, Int32 y)
        {
            m_nID = id;
            m_nX = x;
            m_nY = y;
            m_listEdge = new SortedEdgeList();
        }
        // 
        /// <summary>
        /// adds edges uniquely to the edge list & sorted
        /// </summary>
        /// <param name="e">edge object</param>
        /// <returns> true if added successfully.</returns>
        public bool addEdge(GraphEdge e)
        {
            bool bRet = true;
            foreach (GraphEdge oe in m_listEdge)
            {
                if (oe.m_nID == e.m_nID)
                {
                    bRet = false;
                }
            }
            if (bRet)
                m_listEdge.AddSorted(e);

            return bRet;
        }

        public override string ToString()
        {
            return "[ID:" + m_nID + "][" + m_nX + "," + m_nY + "]";
        }
    }

    /// <summary>
    /// actual graph data structure
    /// </summary>
    class Graph
    {
        // list of nodes that belong to graph
        public SortedNodeList m_listNodes;

        // list of edges that belong to graph
        public SortedEdgeList m_listEdges;

        public Graph()
        {
            m_listEdges = new SortedEdgeList();
            m_listNodes = new SortedNodeList();
        }

        // addNode.
        // adds a node, just that
        public void addNode(GraphNode n)
        {
            m_listNodes.AddSorted(n);
        }

        // addEdge.
        // given valid from & to nodes, it adds an edge.
        // returns true in case of valid nodes, false otherwise
        public bool addEdge(UInt32 nID, UInt32 nFromID, UInt32 nToID, UInt32 nTraffic)
        {
            bool bRet = false;

            GraphNode from = null, to = null;
            if (!(isValidNode(nFromID)) || !(isValidNode(nToID)))
            {
                return bRet;
            }

            from = getNodeById(nFromID);
            to = getNodeById(nToID);

            GraphEdge e = new GraphEdge(nID, from, to, nTraffic);

            // check if from node is valid
            // check if to node is valid
            if (e.m_nodeFrom != null && e.m_nodeTo != null) 
            {
                // calculate weight of the edge
                e.m_dWeight = Util.calcEuclidianDistance(e.m_nodeFrom, e.m_nodeTo);
                // add an edge.
                m_listEdges.AddSorted(e);
                //// also add the edges into from node.
                from.addEdge(e);
                //// also add the edges into to node
                to.addEdge(e);

                bRet = true;
            }
            return bRet;
        }

        // check if given node id is really associated with an existing node.
        public bool isValidNode(UInt32 nNodeID)
        {
            bool bRet = false;

            foreach (GraphNode n in m_listNodes)
            {
                if (nNodeID == n.m_nID)
                {
                    bRet = true;
                    break;
                }
            }

            return bRet;
        }

        /// <summary>
        /// returns the GraphNode object for valid ID.
        /// </summary>
        /// <param name="nID">object ID.</param>
        /// <returns>object for valid ID</returns>
        protected GraphNode getNodeById(UInt32 nID)
        {
            foreach (GraphNode n in m_listNodes)
            {
                if (n.m_nID == nID)
                {
                    return n;
                }
            }
            return null;
        }


        /// <summary>
        /// Runs the KRUSKAL algorithm. It takes parameters from the graph.
        /// it applies the KRUSKAL algorithm on the edges and finally stores
        /// the Minimal list of edges in the graph so that it becomes a tree.
        /// I have preferred to use Graph data structres over tree, It allows 
        /// me to store more data than tree.
        /// </summary>
        /// <param name="listEdges">list of KRUSKAL Edges.</param>
        /// <returns></returns>
        public bool runKruskal(out SortedEdgeList listEdges)
        {
            bool bRet = false;
            // 1. an empty list of edges, this shall be filled.
            SortedEdgeList l_listEdges = new SortedEdgeList();
            // we already have sorted list, our DataStruct handles it.
            // we want to update our edges link in graphnodes hence cleaning 
            // existing links.
            foreach (GraphNode n in m_listNodes)
            {
                n.m_listEdge.Clear();
            }
            // 2 lets generate all possible list of edges (n!)
            SortedEdgeList l_AllPossibleEdges = new SortedEdgeList();
            Util.generateEdgeList(m_listNodes, out l_AllPossibleEdges);

            // 3. foreach edge @ graph (existing links)
            foreach (GraphEdge e in l_AllPossibleEdges)
            {
                // check if it connects two nodes. do a DFS search.
                // essentially this shall choose smallest edges first.
                // each of node contains edges. so hasPathDFS() checks for any
                // feasible path between those edges.
                // if no path exists, than this link connects them
                // so I am adding it.
                if (Util.hasPathDFS(e.m_nodeFrom, e.m_nodeTo) == false)
                {
                    // add the edge in the list
                    l_listEdges.AddSorted(e);
                    // add edges in from and to 
                    e.m_nodeFrom.addEdge(e);
                    e.m_nodeTo.addEdge(e);
                    bRet = true;
                }
            }
            
            // 4. assign it to out parameter
            listEdges = l_listEdges;
            return bRet;
        }

        /// <summary>
        /// Prints the nodes.
        /// </summary>
        public void printNodes()
        {
            foreach (GraphNode n in m_listNodes)
            {
                Console.WriteLine(n);
            }
        }

        /// <summary>
        /// Prints the edges.
        /// </summary>
        public void printEdges()
        {
            foreach (GraphEdge e in m_listEdges)
            {
                Console.WriteLine(e);
            }
        }

    }
}
