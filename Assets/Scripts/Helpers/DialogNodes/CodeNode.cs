//Code by Vincent Kyne

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeNode : DialogNode
{
    //index,class,function

    int gameObject;
    string callClass;
    string function;

    public CodeNode(string nextNode, int gameObject, string callClass, string function):
        base(nextNode, NodeType.CODE)
    {
        this.gameObject = gameObject;
        this.callClass = callClass;
        this.function = function;
    }

    public string ExacuteNode(List<GameObject> interactable)
    {
        //Calls a function using string inputs. For code node in dialog system
        Type t = null;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            t = assembly.GetType(callClass);
            if (t != null)
                break;
        }

        var i = interactable[gameObject].GetComponent(t);

        t.GetMethod(function).Invoke(i, null);

        return nextNode;
    }
}
