//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Item ItemFile
    {
        get { return item; }
        set
        {
            SpriteRenderer sR = GetComponent<SpriteRenderer>();
            item = value;
            if (item != null)
            {
                sR.sprite = item.itemPicture;
                gameObject.name = item.name + "(" + System.Enum.GetName(typeof(Item.tag), item.Tag) + ")";
            }
            else
            {
                sR.sprite = null;
                gameObject.name = "Item(World)";
            }
        }
    }
    public Item item;
    SpriteRenderer spriteRenderer;
    public int amount = 1;
    void Start()
    {
        if (amount == 0)
            Destroy(this);
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        if (item != null)
        {
            sR.sprite = item.itemPicture;
            gameObject.name = item.name + "(" + System.Enum.GetName(typeof(Item.tag), item.Tag) + ")";
        }
        else
        {
            sR.sprite = null;
            gameObject.name = "Item(World)";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                collision.gameObject.GetComponent<PlayerController>().inventory.AddItem(ItemFile, amount);
                if(item.scriptItem != null)
                {
                    if(item.scriptItem.onPickup != null)
                    {
                        item.scriptItem.onPickup.Invoke();
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    //private void OnValidate()
    //{
    //    SpriteRenderer sR = GetComponent<SpriteRenderer>();
    //    if (item != null)
    //    {
    //        sR.sprite = item.itemPicture;
    //        gameObject.name = item.name + "(" + System.Enum.GetName(typeof(Item.tag), item.Tag) + ")";
    //    }
    //    else
    //    {
    //        sR.sprite = null;
    //        gameObject.name = "Item(World)";
    //    }
    //}
}
