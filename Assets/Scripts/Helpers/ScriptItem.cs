//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

[System.Serializable]
[CreateAssetMenu(fileName = "NewScriptItem", menuName = "Inventory/Script Item", order = 2)]
public class ScriptItem : ScriptableObject
{
    [SerializeField]
    public CollisionEvent collisionEvent;
    [SerializeField]
    public UltEvent onPickup;
}
