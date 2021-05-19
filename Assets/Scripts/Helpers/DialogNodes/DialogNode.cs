//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNode 
{
    public enum NodeType {NULL, TEXT, CHOICE, START, RANDOM, CODE};

    public string nextNode;
    public NodeType nodeType;

    public DialogNode(string nextNode, NodeType type = NodeType.NULL)
    {
        this.nextNode = nextNode;
        this.nodeType = type;
    }
}
