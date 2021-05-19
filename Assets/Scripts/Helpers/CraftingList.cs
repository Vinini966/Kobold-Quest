//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemAmout
{
    [SerializeField]
    public Item item;

    [SerializeField]
    public int amount;
}

[System.Serializable]
public struct Recipe
{
    [SerializeField]
    public int level;
    public int time;

    [SerializeField]
    public ItemAmout[] inputs;

    [SerializeField]
    public ItemAmout output;
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewRecipeList", menuName = "Inventory/Crafting List", order = 2)]
public class CraftingList : ScriptableObject
{

    [SerializeField]
    public Recipe[] recipes;
}

