//Code by Vincent Kyne

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class PathFinder
{
    #region Declarations
    public static MapGeneration map;
    private enum NodeStatus { Open, Closed };
    private static Dictionary<Vector2, NodeStatus> nodeStatus = new Dictionary<Vector2, NodeStatus>();
    private const int CostStraight = 1;
    private const int CostDiagonal = 2;
    private static List<PathNode> openList = new List<PathNode>();
    private static Dictionary<Vector2, float> nodeCosts = new Dictionary<Vector2, float>();
    #endregion

    public static void Init(MapGeneration mapGeneration)
    {
        map = mapGeneration;
    }

    #region Helper Methods
    static private void addNodeToOpenList(PathNode node)
    {
        int index = 0;
        float cost = node.TotalCost;
        while ((openList.Count() > index) &&
        (cost < openList[index].TotalCost))
        {
            index++;
        }
        openList.Insert(index, node);
        nodeCosts[node.GridLocation] = node.TotalCost;
        nodeStatus[node.GridLocation] = NodeStatus.Open;
    }
    static private List<PathNode> findAdjacentNodes( PathNode currentNode, PathNode endNode, bool flying = false)
    {
        List<PathNode> adjacentNodes = new List<PathNode>();
        int X = currentNode.GridX;
        int Y = currentNode.GridY;
        bool upLeft = true;
        bool upRight = true;
        bool downLeft = true;
        bool downRight = true;
        if ((X > 0) && (map.getTileTypeFromTilePosition(X - 1, Y) == MapGeneration.TileType.Ground || flying))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X - 1, Y),
            CostStraight + currentNode.DirectCost));
        }
        else
        {
            upLeft = false;
            downLeft = false;
        }
        if ((map.getTileTypeFromTilePosition(X + 1, Y) == MapGeneration.TileType.Ground || flying))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X + 1, Y),
            CostStraight + currentNode.DirectCost));
        }
        else
        {
            upRight = false;
            downRight = false;
        }
        if ((Y > 0) && ((map.getTileTypeFromTilePosition(X, Y - 1) == MapGeneration.TileType.Ground || flying)))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X, Y - 1),
            CostStraight + currentNode.DirectCost));
        }
        else
        {
            upLeft = false;
            upRight = false;
        }
        if ((map.getTileTypeFromTilePosition(X, Y + 1) == MapGeneration.TileType.Ground || flying))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X, Y + 1),
            CostStraight + currentNode.DirectCost));
        }
        else
        {
            downLeft = false;
            downRight = false;
        }
        if ((upLeft) && ((map.getTileTypeFromTilePosition(X - 1, Y - 1) == MapGeneration.TileType.Ground || flying)))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X - 1, Y - 1),
            CostDiagonal + currentNode.DirectCost));
        }
        if ((upRight) && ((map.getTileTypeFromTilePosition(X + 1, Y - 1) == MapGeneration.TileType.Ground || flying)))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X + 1, Y - 1),
            CostDiagonal + currentNode.DirectCost));
        }
        if ((downLeft) && ((map.getTileTypeFromTilePosition(X - 1, Y + 1) == MapGeneration.TileType.Ground || flying)))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X - 1, Y + 1),
            CostDiagonal + currentNode.DirectCost));
        }
        if ((downRight) && ((map.getTileTypeFromTilePosition(X + 1, Y + 1) == MapGeneration.TileType.Ground || flying)))
        {
            adjacentNodes.Add(new PathNode(
            currentNode,
            endNode,
            new Vector2(X + 1, Y + 1),
            CostDiagonal + currentNode.DirectCost));
        }
        return adjacentNodes;
    }
    #endregion

    #region Public Methods
    static public List<Vector2> FindPath(
    Vector2 start,
    Vector2 end,
    bool flying = false)
    {


        Vector2 startTile = map.getTileFromPosition(start.x, start.y);
        Vector2 endTile = map.getTileFromPosition(end.x, end.y);

        if ((map.getTileTypeFromPosition(end.x, end.y) != MapGeneration.TileType.Ground || 
             map.getTileTypeFromPosition(start.x, start.y) != MapGeneration.TileType.Ground) && 
             !flying)
        {
            return null;
        }
        openList.Clear();
        nodeCosts.Clear();
        nodeStatus.Clear();
        PathNode startNode;
        PathNode endNode;
        endNode = new PathNode(null, null, endTile, 0);
        startNode = new PathNode(null, endNode, startTile, 0);
        addNodeToOpenList(startNode);
        while (openList.Count > 0)
        {
            PathNode currentNode = openList[openList.Count - 1];
            if (currentNode.IsEqualToNode(endNode))
            {
                List<Vector2> bestPath = new List<Vector2>();
                while (currentNode != null)
                {
                    bestPath.Insert(0, currentNode.GridLocation);
                    currentNode = currentNode.ParentNode;
                }
                return bestPath;
            }
            openList.Remove(currentNode);
            nodeCosts.Remove(currentNode.GridLocation);
            foreach (PathNode possibleNode in findAdjacentNodes(currentNode, endNode, flying))
            {
                if (nodeStatus.ContainsKey(possibleNode.GridLocation))
                {
                    if (nodeStatus[possibleNode.GridLocation] ==
                    NodeStatus.Closed)
                    {
                        continue;
                    }
                    if (
                    nodeStatus[possibleNode.GridLocation] ==
                    NodeStatus.Open)
                    {
                        if (possibleNode.TotalCost >=
                        nodeCosts[possibleNode.GridLocation])
                        {
                            continue;
                        }
                    }
                }
                addNodeToOpenList(possibleNode);
            }
            nodeStatus[currentNode.GridLocation] = NodeStatus.Closed;
        }
        return null;
    }
    #endregion
}
