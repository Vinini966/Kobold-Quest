//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    [System.Serializable]
    public enum tag {Trap, Food, Bait, Material, Weapon, Armor, Trash}

    [SerializeField]
    public string itemName;

    [SerializeField]
    public Sprite itemPicture;

    [SerializeField]
    [Multiline]
    public string description;

    [SerializeField]
    public tag Tag;

    [SerializeField]
    public float useTime;

    [SerializeField]
    public int sellPrice;

    [SerializeField]
    public int buyPrice;

    [SerializeField]
    public ScriptItem scriptItem;
}
