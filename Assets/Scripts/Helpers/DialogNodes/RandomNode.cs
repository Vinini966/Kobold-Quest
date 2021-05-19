using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNode : DialogNode
{
    List<string> nextNodes = new List<string>();

    int chance = -1; //if chance is -1, all outputs are equal;

    public RandomNode(int possibilities, JSONObject branches):
        base(null, NodeType.RANDOM)
    {
        for(int i = 0; i < possibilities; i++)
        {
            nextNodes.Add(branches.list[i].str);
        }
    }

    public RandomNode(JSONObject branches, int chance = -1):
        base(null, NodeType.RANDOM)
    {
        this.chance = chance;
        foreach(JSONObject n in branches.list)
        {
            nextNodes.Add(n.str);
        }
    }

    public string GetNextNode()
    {
        int n = 0;
        if(chance == -1)
        {
            n = Random.Range(0, nextNodes.Count);
        }
        else
        {
            if (Random.Range(0, 101) < chance)
                n = 0;
            else
                n = 1;
        }


        return nextNodes[n];
    }
}
