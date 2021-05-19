//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStarter : MonoBehaviour
{
    public TextAsset dialog;
    public List<GameObject> interactables = new List<GameObject>();

    public DialogSystem dialogSystem;
    public PlayerController player;

    public bool onStart;
    public bool onTrigger;
    public bool oneShot;
    bool active = false;

    private void OnEnable()
    {
        dialogSystem.onComplete += TextComplete;
        FindObjectOfType<MapGeneration>().mapGenerated += MapGenerated;
    }

    private void OnDisable()
    {
        dialogSystem.onComplete -= TextComplete;
        FindObjectOfType<MapGeneration>().mapGenerated -= MapGenerated;
    }

    // Start is called before the first frame update
    void Start()
    {
        //FindObjectOfType<MapGeneration>().mapGenerated += MapGenerated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onTrigger && other.tag == "Player")
        {
            player.cutScene = true;
            active = true;
            dialogSystem.StartDialog(dialog, interactables);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onTrigger && collision.tag == "Player")
        {
            player.cutScene = true;
            active = true;
            dialogSystem.StartDialog(dialog, interactables);
        }
    }

    public void TextComplete()
    {
        player.cutScene = false;
        if (oneShot && active)
        {
            gameObject.SetActive(false);
        }
    }

    public void MapGenerated()
    {
        player = FindObjectsOfType<PlayerController>()[0];
        if (onStart)
        {
            player.cutScene = true;
            active = true;
            dialogSystem.StartDialog(dialog, interactables);
        }
    }
}
