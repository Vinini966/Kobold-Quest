//Code by Vincent Kyne

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PathNode
{
    #region Declarations
    public PathNode ParentNode;
    public PathNode EndNode;
    private Vector2 gridLocation;
    public float TotalCost;
    public float DirectCost;
    #endregion

    #region Properties
    public Vector2 GridLocation
    {
        get { return gridLocation; }
        set
        {
            gridLocation = new Vector2(
            (float)Mathf.Clamp(value.x, 0f, (float)PathFinder.map.columns),
            (float)Mathf.Clamp(value.y, 0f, (float)PathFinder.map.rows));
        }
    }
    public int GridX
    {
        get { return (int)gridLocation.x; }
    }
    public int GridY
    {
        get { return (int)gridLocation.y; }
    }
    #endregion

    #region Constructor
    public PathNode(
    PathNode parentNode,
    PathNode endNode,
    Vector2 gridLocation,
    float cost)
    {
        ParentNode = parentNode;
        GridLocation = gridLocation;
        EndNode = endNode;
        DirectCost = cost;
        if (!(endNode == null))
        {
            TotalCost = DirectCost + LinearCost();
        }
    }
    #endregion

    #region Helper Methods
    public float LinearCost()
    {
        return (
        Vector2.Distance(
        EndNode.GridLocation,
        this.GridLocation));
    }
    #endregion

    #region Public Methods
    public bool IsEqualToNode(PathNode node)
    {
        return (GridLocation == node.GridLocation);
    }
    #endregion
}

