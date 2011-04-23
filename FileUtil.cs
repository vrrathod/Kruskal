using System;
using System.Collections.Generic;
using System.IO;

namespace MST
{
    /// <summary>
    /// Generic class to provide utility functions
    /// </summary>
    class Util
    {
        /// <summary>
        /// Reads the file and creates graph nodes.
        /// </summary>
        /// <param name="szFileName">Name of the file.</param>
        /// <param name="oGraph">The graph object</param>
        public static void readFileForNodes(String szFileName, ref Graph oGraph)
        {
            // file should have Rows as 
            // "<node id> <X-co-ordinate> <Y-co-ordinate>"
            StreamReader oFile = new StreamReader(szFileName);

            if (oFile != null)
            {
                String szLine;

                while (!oFile.EndOfStream)
                {
                    szLine = oFile.ReadLine();
                    if (szLine.Trim().Length != 0)
                    {
                        String[] listN = szLine.Split(new char[] { ' ', '\t' });
                        GraphNode oNode = new GraphNode(UInt32.Parse(listN[0]),
                                                        Int32.Parse(listN[1]),
                                                        Int32.Parse(listN[2]));
                        oGraph.addNode(oNode);
                    }
                }

            }
        }
        /// <summary>
        /// Reads the file and creates graph edges.
        /// </summary>
        /// <param name="szFileName">Name of the file.</param>
        /// <param name="oGraph">The graph object.</param>
        public static void readFileForEdges(String szFileName, ref Graph oGraph)
        {
            // file should have Rows as 
            // "<from-node> <to-node> <traffic in kbps>"
            StreamReader oFile = new StreamReader(szFileName);

            if (oFile != null)
            {
                String szLine;

                UInt32 uStartIdx = (UInt32) oGraph.m_listEdges.Count;

                while (!oFile.EndOfStream)
                {
                    szLine = oFile.ReadLine();
                    if (szLine.Trim().Length != 0)
                    {
                        String[] listN = szLine.Split(new char[] { ' ', '\t' });
                        oGraph.addEdge(++uStartIdx, 
                                       UInt32.Parse(listN[0]),
                                       UInt32.Parse(listN[1]),
                                       UInt32.Parse(listN[2]));
                    }                  
                }

            }
        }

        /// <summary>
        /// Calculates the euclidian distance.
        /// </summary>
        /// <param name="n1">The node 1.</param>
        /// <param name="n2">The node 2.</param>
        /// <returns>Euclidian distance.</returns>
        public static Double calcEuclidianDistance(GraphNode n1, GraphNode n2)
        {
            Int32 nXDiff = n1.m_nX - n2.m_nX;
            Int32 nYDiff = n1.m_nY - n2.m_nY;
            return Math.Sqrt(nXDiff * nXDiff + nYDiff * nYDiff);
        }

        // contains IDs for all the edges in the path.
        public static List<UInt32> listPath = new List<UInt32>();


        /// <summary>
        /// Determines whether 'from' and 'to' has path or connected.
        /// It uses Depth first search method, by examining edges
        /// This is overloaded method.
        /// first method provides interfacing, while second does actual work.
        /// </summary>
        /// <param name="from">From GraphNode</param>
        /// <param name="to">To GraphNode</param>
        /// <returns>
        /// 	<c>true</c> if [has path] ; otherwise, <c>false</c>.
        /// </returns>
        public static bool hasPathDFS(GraphNode from, GraphNode to)
        {
            List<UInt32> listEdgeIDs = new List<UInt32>();
            listPath.Clear();
            return hasPathDFS(from, to, from.m_nID, to.m_nID, false, listEdgeIDs);
        }

        // this method actually implements 
        /// <summary>
        /// Determines whether there is any path connecting from & to.
        /// </summary>
        /// <param name="from">From Node.</param>
        /// <param name="to">To Node.</param>
        /// <param name="nFromID">The original from ID. 
        ///  This is a recursive method, hence we need it.
        ///  or May be it can be stored as a class member. </param>
        /// <param name="nToID">The original to ID.
        ///  This is a recursive method, hence we need it.
        ///  or May be it can be stored as a class member. </param>
        /// <param name="bValidate">check to identify any loops</param>
        /// <param name="listEdgeIDs">check to identify any loops.
        /// This is a way we will avoid any checked Edge ID.
        /// </param>
        /// <returns>
        /// 	<c>true</c> if from and to are connected otherwise, <c>false</c>.
        /// </returns>
        protected static bool hasPathDFS(GraphNode from, GraphNode to, 
                                         UInt32 nFromID, UInt32 nToID, 
                                         bool bValidate, 
                                         List<UInt32> listPathEdges)
        {
            bool bRet = false;

            if (from.m_nID == nToID)
            {
                // we have reached destination.
                bRet = true;
                return bRet;
            }

            if (bValidate && (from.m_nID == nFromID))
            {
                // it should not loop;
                return bRet;
            }

            foreach (GraphEdge e in from.m_listEdge)
            {
                // if we already have examined the edge, we can skip it.
                if (listPathEdges.Contains(e.m_nID))
                    continue;
                else
                    listPathEdges.Add(e.m_nID); // this remembers examined edges.

                listPath.Add(e.m_nID);

                // we have to move to the next node, if we have come from 
                // a 'from' node we should examine a 'to' node & if from
                // a 'to' node we should examine a 'from' node for next edge.
                if (from.m_nID == e.m_nodeFrom.m_nID)
                    bRet = Util.hasPathDFS(e.m_nodeTo, to, 
                                           nFromID, nToID, 
                                           true, listPathEdges);
                else
                    bRet = Util.hasPathDFS(e.m_nodeFrom, to, 
                                           nFromID, nToID, 
                                           true, listPathEdges);

                if (bRet == true)
                    break;
                else
                {
                    // bRet == false, only if recursive check fails.
                    // => seems like this link does not lead to the 'to' node.
                    // hence removing edge id.
                    listPath.Remove(e.m_nID);
                }
            }

            return bRet;
        }

        /// <summary>
        /// Generates all the possible edges for given nodes and returns them.
        /// </summary>
        /// <param name="listNodes">The list nodes.</param>
        /// <param name="listEdges">[out]The list edges.</param>
        /// <returns></returns>
        public static bool generateEdgeList(SortedNodeList listNodes,
                                            out SortedEdgeList listEdges)
        {
            bool bRet = false;
            SortedEdgeList l_listEdges = new SortedEdgeList();
            UInt32 nEdgeID = 0;

            for(Int32 nFrom = 0; nFrom < listNodes.Count; ++nFrom)
            {
                for (Int32 nTo = nFrom+1; nTo < listNodes.Count; ++nTo)
                {
                    // make sure it is sorted by distance.
                    GraphEdge e = new GraphEdge(++nEdgeID,
                                                listNodes[nFrom],
                                                listNodes[nTo],
                                                0);
                    l_listEdges.AddSorted(e);
                    bRet = true;
                }
            }
            listEdges = l_listEdges;
            return bRet;
        }
    }
}
