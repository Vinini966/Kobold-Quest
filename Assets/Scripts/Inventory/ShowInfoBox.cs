//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowInfoBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Item item;
    public GameObject infoBoxPrefab;
    public Inventory baseInventory;
    string infoBoxText;
    GameObject infoBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        Destroy(infoBox);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBox = Instantiate(infoBoxPrefab, baseInventory.transform);
        infoBox.transform.GetChild(0).GetComponent<Text>().text = infoBoxText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(infoBox);
    }

    public void SetupInfobox(Item baseItem, Inventory inventory)
    {
        item = baseItem;
        baseInventory = inventory;
        if(item.Tag != Item.tag.Trap)
            infoBoxText = item.itemName + "\n" + item.description + "\n" + item.sellPrice + "GP";
        else
            infoBoxText = item.itemName + "\nSetup Time: " + item.useTime + " sec\n" + item.description + "\n" + item.sellPrice + "GP";
    }
}
