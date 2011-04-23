using System;
using System.Collections.Generic;

namespace MST
{
    class Program
    {
        // this will store the edges of MST
        static SortedEdgeList listMSTEdges;
        // this is original graph structure which will run KRUSKAL algo
        static Graph oGraph;
        // after running KRUSKAL, we will store the updated path & hop info.
        static List<PathListEntry> listPaths;
        // average delay for the designed network.
        static Double m_dAvgHop;
        // max utilization
        static Double m_dMaxUtil;
        // avg delay
        static Double m_dAvgDelay;

        // main method
        static void Main(string[] args)
        {
            // we will create a graph.
            oGraph = new Graph();
            // we will read following files to get node locations & traffic
            String szNodeFile = @"..\..\nodelocations.txt";
            String szEdgeFile = @"..\..\traffictable.txt";

            // read node list & add them into graph.
            Util.readFileForNodes(szNodeFile, ref oGraph);
            // read edge list & add them into graph.
            Util.readFileForEdges(szEdgeFile, ref oGraph);
            
            // init MST edges data structure
            listMSTEdges = new SortedEdgeList();
            // init Path list
            listPaths = new List<PathListEntry>();
            // lets run kruskal
            oGraph.runKruskal(out listMSTEdges);

            
            Console.Clear();
            
            // TASK 1: print loc1, loc2, distance, with a total dist
            taskPrintMST();
            Console.WriteLine("----------------------------------------------------");
            // TASK 2: Utilization\
            taskCalcNPrintUtilization();
            Console.WriteLine("----------------------------------------------------");
            // TASK 3: Average hops
            taskAvgHop();
            Console.WriteLine("----------------------------------------------------");
            // TASK 4: average delay
            taskAvgDelay();
        }

        /// <summary>
        /// Find minimum spanning tree & print it.
        /// finding is done @ oGraph.runKruskal()
        /// now this function prints the MST.
        /// </summary>
        public static void taskPrintMST()
        {
            Double dTotal = 0;
            Console.WriteLine("Loc1 |  Loc2 |  Distance");
            foreach (GraphEdge e in listMSTEdges)
            {
                Console.WriteLine("  {0} |   {1}  |    {2} Km",
                                  e.m_nodeFrom.m_nID.ToString("0#"),
                                  e.m_nodeTo.m_nID.ToString("0#"),
                                  e.m_dWeight.ToString("f"));
                dTotal += e.m_dWeight;
            }
            Console.WriteLine("Total Weight = {0} Km", dTotal.ToString("f"));
        }

        // 
        /// <summary>
        /// Find load on each of the MST edges.
        /// 1. Find paths for original traffic routes.
        /// 2. Update the MST edges to include original traffic.
        /// 3. Calculate Utilization (arrival rate / departure rate).
        /// USED DFS model to find the connectivity.
        /// </summary>
        public static void taskCalcNPrintUtilization()
        {
            // 1. for each of the original route,
            foreach (GraphEdge e in oGraph.m_listEdges)
            {
                // 1. find & store path between each pair of nodes.
                if (Util.hasPathDFS(e.m_nodeFrom, e.m_nodeTo))
                {
                    PathListEntry oPath = new PathListEntry();
                    oPath.m_nFromNode = e.m_nodeFrom.m_nID;
                    oPath.m_nToNode = e.m_nodeTo.m_nID;
                    oPath.m_nTraffic = e.m_nTraffic;
                    oPath.m_listPath.AddRange( Util.listPath);
                    listPaths.Add(oPath);

                    // if count = 1 => it is directly connected in MST
                    // so following 'if' checks if it is not directly connected
                    if (Util.listPath.Count > 1)
                    {
                        // 2. for each edge in the path, add the traffic
                        foreach (GraphEdge oeMST in listMSTEdges)
                        {
                            if (Util.listPath.Contains(oeMST.m_nID))
                            {
                                oeMST.m_nTraffic += e.m_nTraffic;
                            }
                        }
                    }
                }
            }

            // Each link is updated @ traffic loads (Kbps) => arrival rate.
            // We have T1(1.544Mbps) links => departure rate.
            // utilization is arrival / departure rate.
            Double dTotalUtil = 0;
            Double dUtil = 0;
            Console.WriteLine("From |  To |  Traffic  | Utilization ");
            foreach (GraphEdge e in listMSTEdges)
            {
                dUtil = e.m_nTraffic; // is in Kbps
                dUtil /= (1.544 * 1024); // to convert to Kbps from Mbps
                Console.WriteLine("  {0} |  {1} |  {2} Kbps | {3}",
                                  e.m_nodeFrom.m_nID.ToString("0#"),
                                  e.m_nodeTo.m_nID.ToString("0#"),
                                  e.m_nTraffic.ToString("0##.##"),
                                  dUtil.ToString("p"));
                dTotalUtil += dUtil;
                if (dUtil > m_dMaxUtil)
                    m_dMaxUtil = dUtil;
            }
            Console.WriteLine(" Max Util = {0}", m_dMaxUtil.ToString("p"));
            Double dAvg = dTotalUtil;
            dAvg /= listMSTEdges.Count;
            Console.WriteLine(" Avg Util = {0}", dAvg.ToString("p"));
        }
        // third task, average hops.
        public static void taskAvgHop()
        {
            Double dTotalTraffic = 0;
            Double dHopTraffic = 0;
            Console.WriteLine("From | To | Traffic | Hops");
            foreach (PathListEntry e in listPaths)
            {
                Console.WriteLine(e.printforTask3());
                // sum up all the hops
                dHopTraffic += e.m_listPath.Count * e.m_nTraffic;
                // sum up all the traffic
                dTotalTraffic += e.m_nTraffic;
            }

            m_dAvgHop = dHopTraffic / dTotalTraffic;
            Console.WriteLine(" Avg Hops = {0}", m_dAvgHop.ToString("f"));
        }

        /// <summary>
        /// prints the avg delay.
        /// </summary>
        public static void taskAvgDelay()
        {
            // by definition
            // avgdelay = (Tbar * avgHop) / (1 - maxUtil);
            UInt32 nPktSz = 512; // bytes
            Double dTbar = nPktSz * 8;
            dTbar /= 1.544 * 1024 * 1024; // MB = 1024*1024 bytes

            m_dAvgDelay = dTbar * m_dAvgHop;
            m_dAvgDelay /= (1 - m_dMaxUtil);
            m_dAvgDelay *= 1000;

            Console.WriteLine(" Avg Delay = {0} mili sec (considering Mbps = 1024*1024 bps)", 
                              m_dAvgDelay.ToString("f"));

            dTbar = nPktSz * 8;
            dTbar /= 1.544 * 1000 * 1000; // MB = 1024*1024 bytes

            m_dAvgDelay = dTbar * m_dAvgHop;
            m_dAvgDelay /= (1 - m_dMaxUtil);
            m_dAvgDelay *= 1000;
            Console.WriteLine(" Avg Delay = {0} mili sec (considering Mbps = 1000*1000 bps)",
                              m_dAvgDelay.ToString("f"));
        }
    }
}
