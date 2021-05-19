//Code by Vincent Kyne

using Febucci.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public TextAsset dialogfile;
    public Image charImage;

    DialogImporter dialogtree;

    public AudioSource audioPlay;

    public CharacterProfile profile;
    Dictionary<string, CharacterProfile.CharData> profileDict;

    string ignoreChars = ".,?;!";

    public bool readyNext = true,
         active = false,
         firstpass = true;

    public string currentNode = "START";

    public Text speaker;
    public TextAnimatorPlayer text;
    public VerticalLayoutGroup buttonHolder;

    public GameObject ButtonPrefab;
    
    [SerializeField]
    public List<GameObject> Interatables;

    public delegate void OnComplete();
    public OnComplete onComplete;

    public delegate void OnStart();
    public OnStart onStart;

    bool textDone = false;


    // Start is called before the first frame update
    void Start()
    {
        dialogtree = new DialogImporter();
        if (dialogfile != null)
            dialogtree.ReadTree(dialogfile);

        Interatables.Insert(0, gameObject);

        profileDict = profile.ToDictionary();

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (!readyNext)
            {
                switch (dialogtree.dialogTree[currentNode].nodeType)
                {
                    case DialogNode.NodeType.TEXT:
                        if (firstpass)
                        {
                            speaker.text = ((TextBoxNode)dialogtree.dialogTree[currentNode]).character;
                            if (profileDict.ContainsKey(speaker.text))
                                if (charImage != null)
                                    charImage.sprite = profileDict[speaker.text].profilePicture;
                                else
                                    Debug.LogWarning("No Portrait Box. Not Showing Portrait.");
                            else
                                Debug.LogError("Missing Profile for " + speaker.text);
                            firstpass = false;
                            text.ShowText(((TextBoxNode)dialogtree.dialogTree[currentNode]).text);
                        }
                        if(textDone)
                        {
                            if (Input.GetKeyUp(KeyCode.Return))
                            {
                                currentNode = dialogtree.dialogTree[currentNode].nextNode;
                                readyNext = true;
                            }
                        }
                        else
                        {
                            if (Input.GetKeyUp(KeyCode.Return))
                            {
                                text.SkipTypewriter();
                            }
                        }
                        
                        
                        break;

                    case DialogNode.NodeType.CHOICE:
                        if (firstpass)
                        {
                            for(int i = 0; i < ((ChoiceNode)dialogtree.dialogTree[currentNode]).choices.Count; i++)
                            {
                                GameObject a = Instantiate(ButtonPrefab);
                                a.transform.SetParent(buttonHolder.transform);
                                a.GetComponentsInChildren<Text>()[0].text = ((ChoiceNode)dialogtree.dialogTree[currentNode]).choices[i].text;
                                int x = i;
                                a.GetComponent<Button>().onClick.AddListener(() => Choice(x));
                            }
                            speaker.text = ((TextBoxNode)dialogtree.dialogTree[currentNode]).character;
                            if (profileDict.ContainsKey(speaker.text))
                                charImage.sprite = profileDict[speaker.text].profilePicture;
                            else
                                Debug.LogError("Missing Profile for " + speaker.text);
                            firstpass = false;
                            text.ShowText(((TextBoxNode)dialogtree.dialogTree[currentNode]).text);
                        }
                        if (textDone)
                        {

                        }
                        else
                        {
                            if (Input.GetKeyUp(KeyCode.Return))
                            {
                                text.SkipTypewriter();
                            }
                        }
                        break;

                    case DialogNode.NodeType.RANDOM:
                        currentNode = ((RandomNode)dialogtree.dialogTree[currentNode]).GetNextNode();
                        readyNext = true;
                        break;

                    case DialogNode.NodeType.CODE:
                        currentNode = ((CodeNode)dialogtree.dialogTree[currentNode]).ExacuteNode(Interatables);
                        readyNext = true;
                        break;

                    case DialogNode.NodeType.NULL:
                        currentNode = dialogtree.dialogTree[currentNode].nextNode;
                        readyNext = true;
                        break;
                    default:
                        currentNode = dialogtree.dialogTree[currentNode].nextNode;
                        break;
                }
            }
            else
            {
                //currentNode = dialogtree.dialogTree[currentNode].nextNode;
                readyNext = false;
                firstpass = true;
                textDone = false;
                //text.text = "";
                if (currentNode == null)
                {
                    speaker.text = "";
                    active = false;
                    gameObject.SetActive(false);
                    currentNode = "START";
                    if(onComplete != null)
                        onComplete();
                }
                    
            }
        }
    }

    public void Choice(int choice)
    {
        currentNode = ((ChoiceNode)dialogtree.dialogTree[currentNode]).GetNextBasedOnChoice(choice);
        readyNext = true;
        for(int i = buttonHolder.GetComponentsInChildren<Button>().Length - 1; i >= 0; i--)
        {
            Destroy(buttonHolder.GetComponentsInChildren<Button>()[i].gameObject);
        }
    }

    public void StartDialog()
    {
        active = true;
        gameObject.SetActive(true);
        if (onStart != null)
            onStart();
    }

    public void StartDialog(TextAsset dialog, List<GameObject> interactables = null)
    {
        gameObject.SetActive(true);
        dialogfile = dialog;
        Interatables = new List<GameObject>(interactables);
        if(Interatables == null)
        {
            Interatables = new List<GameObject>();
        }
        Interatables.Insert(0, gameObject);
        dialogtree = new DialogImporter();
        if (dialogfile != null)
            dialogtree.ReadTree(dialogfile);
        active = true;
        if (onStart != null)
            onStart();
    }

    public void test()
    {
        Debug.Log("Test");
    }

    public void TextShown()
    {
        textDone = true;
    }

    public void playLetter(char letter)
    {
        if (ignoreChars.Contains(letter.ToString()))
            return;

        int sound = (int)letter % 5;
        if (profileDict.ContainsKey(speaker.text))
        {
            switch (sound)
            {
                case 0:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Ah);
                break;
                case 1:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Eh);
                    break;
                case 2:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Oh);
                    break;
                case 3:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Uh);
                    break;
                case 4:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Oo);
                    break;
                default:
                    GetComponent<AudioSource>().PlayOneShot(profileDict[speaker.text].Ah);
                    break;
            }
        }
        else
        {
            Debug.LogError("Missing Profile for " + speaker.text);
        }
        
    }
}
