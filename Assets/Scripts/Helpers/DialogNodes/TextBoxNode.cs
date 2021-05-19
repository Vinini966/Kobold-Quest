//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxNode : DialogNode
{
    public string character;
    public string text;

    public TextBoxNode(string nextNode, string character, string text, NodeType type = NodeType.TEXT) : 
        base(nextNode, type)
    {
        this.character = character;
        this.text = text;
    }
}
