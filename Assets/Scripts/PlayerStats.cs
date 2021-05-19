using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //There's probably other stats I missed but this is just a base
    public int health = 10;
    public int speed = 10;
    public int defense = 10;
    public int offense = 10;
    public int currency = 10;
    public int exp = 0;
    public int level = 0;

    //Level Stats
    public int currentLevel = 0; 
    public int maxRooms = 20;
    public int maxEnemies = 5;


    public static PlayerStats i;

    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    public List<Item> traps = new List<Item>();

    // Start is called before the first frame update
    void Awake()
    {
        if(i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame++
    void Update()
    {
        
    }

    public static PlayerStats getInstacne()
    {
        return i;
    }

    public void raiseDifficulty()
    {
        currentLevel += 1;
        maxRooms += 4;
        maxEnemies += 2;
    }
}
