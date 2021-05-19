//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class DialogImporter
{
    public TextAsset dialogFile;

    //In case Dialog Desinger updates and changes JSON tags
    public static string nodeType = "node_type",
                  next = "next",
                  nodeName = "node_name",
                  speaker = "character",
                  text = "text",
                  choices = "choices",
                  poss = "possibilities",
                  branch = "branches",
                  chance = "chance_1",
                  lang = "ENG"; //replace with main setting for language.

    JSONObject dialog;

    public Dictionary<string, DialogNode> dialogTree;

    // Start is called before the first frame update
    public void ReadTree(TextAsset textAsset)
    {
        dialogTree = new Dictionary<string, DialogNode>();
        dialogFile = textAsset;

        dialog = new JSONObject(dialogFile.text);
        dialog = dialog[0]["nodes"];

        //parses the json file based on node types
        foreach(JSONObject n in dialog.list)
        {
            switch (n[nodeType].str)
            {

                case "start":
                    DialogNode a = new DialogNode(n[next].str, DialogNode.NodeType.START);
                    dialogTree.Add(n[nodeName].str, a);
                    break;
                case "show_message":

                    if (n[text][lang].str.Contains("\\n"))
                        n[text][lang].str = n[text][lang].str.Replace("\\n", "\n");

                    if (n.keys.Contains("choices"))
                    {
                        ChoiceNode c = new ChoiceNode(n[speaker][0].str, n[text][lang].str, n[choices]);
                        dialogTree.Add(n[nodeName].str, c);
                    }
                    else
                    {
                        TextBoxNode tb = new TextBoxNode(n[next].str, n[speaker][0].str, n[text][lang].str);
                        dialogTree.Add(n[nodeName].str, tb);
                    }
                    break;
                case "chance_branch":
                case "random_branch":
                    if (n.keys.Contains("chance_1"))
                    {
                        RandomNode cb = new RandomNode(n[branch], (int)n[chance].i);
                        dialogTree.Add(n[nodeName].str, cb);
                    }
                    else
                    {
                        RandomNode rb = new RandomNode((int)n[poss].i, n[branch]);
                        dialogTree.Add(n[nodeName].str, rb);
                    }
                    break;
                case "execute":
                    string function = n[text].str;
                    string[] inputs = function.Split(',');

                    CodeNode cn = new CodeNode(n[next].str, Int32.Parse(inputs[0]), inputs[1], inputs[2]);
                    dialogTree.Add(n[nodeName].str, cn);

                    break;
                default:
                    DialogNode d = new DialogNode(n[next].str, DialogNode.NodeType.NULL);
                    dialogTree.Add(n[nodeName].str, d);
                    break;
            }
        }

    }

}
