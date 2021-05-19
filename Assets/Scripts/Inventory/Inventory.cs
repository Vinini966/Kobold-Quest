//Code by Vincent Kyne

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    PlayerStats player;

    [SerializeField]
    public Item currency;
    [SerializeField]
    public CraftingList craftingList;
    public GameObject craftingButtonPrefab;

    public GameObject ItemUI;
    public GameObject Content;
    public GameObject CurrTrap;
    public GameObject trapPrefab;
    public bool open = false;
    public bool crafting = false;

    public static Timer countdownTimer;
    public static bool isCrafting = false;

    List<GameObject> itemTiles = new List<GameObject>();
    
    public int trapOn = 0;
    public int totalTraps = 0;

    public delegate void MenuClosing();
    public MenuClosing menuClosing;

    public delegate void MenuOpening();
    public MenuOpening menuOpening;


    // Start is called before the first frame update
    void Awake()
    {
        player = PlayerStats.getInstacne();
        if (player == null)
            Debug.LogError("Player Stats not active in this scene.");
        totalTraps = player.traps.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(player.traps.Count != 0)
        {
            if(!open)
                CurrTrap.SetActive(true);
            CurrTrap.GetComponent<Image>().sprite = player.traps[trapOn].itemPicture;
            CurrTrap.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "x" + player.inventory[player.traps[trapOn]];
        }
        else
        {
            CurrTrap.SetActive(false);
        }
        
    }

    public void Hide()
    {
        if (menuClosing != null)
            menuClosing();

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        CurrTrap.SetActive(true);
        open = false;
    }

    public void Show()
    {
        if (menuOpening != null)
            menuOpening();

        UpdateUI();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        CurrTrap.SetActive(false);
        
        open = true;
        crafting = false;
    }

    public void ShowCrafting()
    {
        if (menuOpening != null)
            menuOpening();

        UpdateUI(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        CurrTrap.SetActive(false);

        open = true;
        crafting = true;
    }

    public void UpdateUI(bool craftingOpen = false)
    {
        for(int i = itemTiles.Count - 1; i >= 0; i--)
        {
            Destroy(itemTiles[i]);
        }
        if (craftingOpen)
        {
            foreach(Recipe n in craftingList.recipes)
            {
                GameObject b = Instantiate(craftingButtonPrefab, Content.transform);
                b.GetComponent<Image>().sprite = n.output.item.itemPicture;
                b.GetComponent<Crafting>().recipe = n;
                b.GetComponent<Crafting>().baseInventory = this;
                b.GetComponent<Crafting>().timerBar = b.transform.GetChild(0).GetComponent<Slider>();
                b.GetComponent<Button>().onClick.AddListener(() => b.GetComponent<Crafting>().CraftRecipe());
                itemTiles.Add(b);
            }
        }
        else
        {
            foreach (KeyValuePair<Item, int> kvp in player.inventory)
            {
                if (kvp.Value > 0)
                {
                    GameObject a = Instantiate(ItemUI, Content.transform);
                    a.GetComponent<Image>().sprite = kvp.Key.itemPicture;
                    a.GetComponent<ShowInfoBox>().SetupInfobox(kvp.Key, this);
                    a.transform.GetChild(0).GetComponent<Text>().text = "x" + kvp.Value;
                    itemTiles.Add(a);

                }

            }
        }
        
    }

    public void AddItem(Item add, int amount)
    {
        if(player.inventory.ContainsKey(add))
        {
            player.inventory[add] += amount;
        }
        else
        {
            player.inventory.Add(add, amount);
            if(add.Tag == Item.tag.Trap)
            {
                player.traps.Add(add);
                totalTraps = player.traps.Count;
            }
        }

    }

    public bool SellItem(Item item, int amount)
    {
        if (player.inventory.ContainsKey(item))
        {
            if(player.inventory[item] >= amount)
            {
                player.inventory[item] -= amount;
                AddItem(currency, item.sellPrice);
                return true;
            }
            return false;
        }
        else
        {
            //error
            return false;
        }
    }

    public void TrashItem(Item item, int amount)
    {
        if (player.inventory.ContainsKey(item))
        {
            if (player.inventory[item] >= amount)
            {
                player.inventory[item] -= amount;
            }
            else
            {
                player.inventory[item] = 0;
            }
            if(player.inventory[item] == 0)
            {
                player.inventory.Remove(item);
                if (player.traps.Contains(item))
                {
                    player.traps.Remove(item);
                    totalTraps = player.traps.Count;
                    if(trapOn >= player.traps.Count)
                    {
                        trapOn = player.traps.Count - 1;
                    }
                }
            }
        }
        else
        {
            //error
        }
    }

    public void ValueChanged(Dropdown change)
    {
        Debug.Log("This Works");


        
    }

}
