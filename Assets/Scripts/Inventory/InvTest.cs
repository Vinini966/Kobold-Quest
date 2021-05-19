using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvTest : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField]
    public Item[] items;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Waiting...");
        inventory.Hide();
        foreach(Item i in items)
        {
            inventory.AddItem(i, Random.Range(0, 10));
        }
        //inventory.Show();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
