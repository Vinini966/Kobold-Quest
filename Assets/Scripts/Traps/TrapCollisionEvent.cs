using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollisionEvent : MonoBehaviour
{
    [SerializeField]
    public Item item;


    CollisionEvent @event;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        if (item != null)
        {
            sR.sprite = item.itemPicture;
            gameObject.name = item.name + "(" + System.Enum.GetName(typeof(Item.tag), item.Tag) + ")";

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateData()
    {
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        if (item != null)
        {
            sR.sprite = item.itemPicture;
            gameObject.name = item.name + "(" + System.Enum.GetName(typeof(Item.tag), item.Tag) + ")";

            if (item.scriptItem != null)
                @event = item.scriptItem.collisionEvent;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (item.scriptItem != null)
            item.scriptItem.collisionEvent.Invoke(collision, this);
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
    //        gameObject.name = "Base(Trap)";
    //    }
    //}
}
