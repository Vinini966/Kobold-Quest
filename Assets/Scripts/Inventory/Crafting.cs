//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    bool craftable;
    public Inventory baseInventory;
    public GameObject infoBoxPrefab;
    public GameObject infoBox;
    public Slider timerBar;
    
    public string infoboxText;
    public string amoutLeft;
    public bool itemCrafting = false;

    // Start is called before the first frame update
    void Start()
    {
        infoboxText = recipe.output.item.name + 
                      " x" + recipe.output.amount + 
                      "\nCraft Time:\n" + recipe.time + " sec\nCost:\n";
        craftable = true;
        foreach(ItemAmout n in recipe.inputs)
        {
            infoboxText += n.item.name + " x" + n.amount;
            if (PlayerStats.getInstacne().inventory.ContainsKey(n.item))
            {
                amoutLeft = " (" + PlayerStats.getInstacne().inventory[n.item] + ")" + "\n";
                if (PlayerStats.getInstacne().inventory[n.item] < n.amount)
                {
                    craftable = false;
                }
            }
            else
            {
                amoutLeft = " (0)" + "\n";
                craftable = false;
            }
        }
        //countdownTimer = new Timer(recipe.time);
        gameObject.GetComponent<Button>().interactable = craftable;
    }

    private void OnEnable()
    {
        baseInventory.menuClosing += StopTimer;
    }

    private void OnDisable()
    {
        baseInventory.menuClosing -= StopTimer;
        Destroy(infoBox);
    }

    // Update is called once per frame
    void Update()
    {
        craftable = true;
        foreach (ItemAmout n in recipe.inputs)
        {
            if (PlayerStats.getInstacne().inventory.ContainsKey(n.item))
            {
                amoutLeft = " (" + PlayerStats.getInstacne().inventory[n.item] + ")" + "\n";
                if (PlayerStats.getInstacne().inventory[n.item] < n.amount)
                {
                    craftable = false;
                    break;
                }
            }
            else
            {
                amoutLeft = " (0)" + "\n";
                craftable = false;
                break;
            }
        }
        gameObject.GetComponent<Button>().interactable = craftable;

        if (Inventory.isCrafting && itemCrafting)
        {
            if (Inventory.countdownTimer.CheckTimer())
            {
                foreach (ItemAmout n in recipe.inputs)
                {
                    baseInventory.TrashItem(n.item, n.amount);
                    if(PlayerStats.getInstacne().inventory.ContainsKey(n.item))
                        amoutLeft = " (" + PlayerStats.getInstacne().inventory[n.item] + ")" + "\n";
                    else
                        amoutLeft = " (0)" + "\n";
                }
                if(infoBox != null && infoBox.activeInHierarchy)
                    infoBox.transform.GetChild(0).GetComponent<Text>().text = infoboxText + amoutLeft;
                baseInventory.AddItem(recipe.output.item, recipe.output.amount);
                timerBar.gameObject.SetActive(false);
                Inventory.isCrafting = false;
                itemCrafting = false;
            }
            else
            {
                Inventory.countdownTimer.TimerUpdate();
                timerBar.value = Inventory.countdownTimer.GetPercent();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBox = Instantiate(infoBoxPrefab, baseInventory.transform);
        infoBox.transform.GetChild(0).GetComponent<Text>().text = infoboxText + amoutLeft;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(infoBox);
    }

    public void CraftRecipe()
    {
        if (!Inventory.isCrafting)
        {
            if (craftable)
            {
                Inventory.countdownTimer = new Timer(recipe.time);
                timerBar.gameObject.SetActive(true);
                Inventory.countdownTimer.StartTimer();
                Inventory.isCrafting = true;
                itemCrafting = true;
            }
        }

        //if (craftable)
        //{
        //    foreach (ItemAmout n in recipe.inputs)
        //    {
        //        baseInventory.TrashItem(n.item, n.amount);
        //    }
        //    baseInventory.AddItem(recipe.output.item, recipe.output.amount);
        //}
        
    }

    void StopTimer()
    {
        if(Inventory.countdownTimer != null)
            Inventory.countdownTimer.StopTimer();
        Inventory.isCrafting = false;
                itemCrafting = false;
    }
}
