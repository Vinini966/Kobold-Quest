using UnityEngine;

//So generating enemies and whatnot can be handled within the rooms. Do something like make an enum or array of monsters
//set their positions on where they are instantiated and yay!
//SHould work the same for items and whatnot...I think

public class Room : MonoBehaviour
{
    public int xPos; //x position of room
    public int yPos; //y position of room
    public int roomWidth; //width of the room
    public int roomHeight; //height of the room
    public Direction enteringCorridor; //The direction of a corridor entering the room;



    //Function for setting up the base of the room
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int width, int height, GameObject player, Inventory inventory, MapGeneration mapGeneration)
    {
        //Grab the tilescale
        float tileScale = mapGeneration.tileScale;

        //Set the room width and height
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        //Set the coordinates of the room (approximately in the center of the room)
        xPos = Mathf.RoundToInt(width / 2f - roomWidth / 2f);
        yPos = Mathf.RoundToInt(height / 2f - roomHeight / 2f);

        //Spawn the player at the center of room
        GameObject a = Instantiate(player, new Vector3((xPos + roomWidth / 2) * tileScale, (yPos + roomHeight / 2) * tileScale, 0), Quaternion.identity);
        a.GetComponent<PlayerController>().inventory = inventory;
    }

    //Overload function for rooms with corridors, the first function is used for the first room only.
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int floorWidth, int floorHeight, Corridor corridor, MapGeneration mapGeneration, bool spawnExit, bool spawnKey, bool spawnShop)
    {
        //Grab tilescale
        float tileScale = mapGeneration.tileScale;

        //Set the corridor's entering direction
        enteringCorridor = corridor.direction;

        //Set random values for width and height
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        //Now we're checking which direction the entering corridor was going and reacting accordingly
        switch (corridor.direction)
        {
            //So in the case of it going North...
            case Direction.North:
                //Clamp the height to make sure the room is not going beyond the floor (take the corridor's end position into account)
                roomHeight = Mathf.Clamp(roomHeight, 1, (floorHeight - 3) - corridor.EndPositionY);

                //The room's yPosition should be at end of the corridor because a north corridor is coming up to the bottom of this room
                yPos = corridor.EndPositionY;

                //In this case, the x position can be random, but it can't be in a position that's too far from the end x position of the corridor
                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);

                //Clamp the x position to make sure it isn't outside of the floor itself
                xPos = Mathf.Clamp(xPos, 3, (floorWidth - 3) - roomWidth);
                break;

            //This logic applies to all other cases, with other values of course
            case Direction.East:
                roomWidth = Mathf.Clamp(roomWidth, 1, (floorWidth - 3) - corridor.EndPositionX);
                xPos = corridor.EndPositionX;
                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
                yPos = Mathf.Clamp(yPos, 3, (floorHeight - 3) - roomHeight);
                break;

            case Direction.South:
                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY);
                yPos = corridor.EndPositionY - roomHeight + 1;
                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
                xPos = Mathf.Clamp(xPos, 3, (floorWidth - 3) - roomWidth);
                break;

            case Direction.West:
                roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);
                xPos = corridor.EndPositionX - roomWidth + 1;
                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
                yPos = Mathf.Clamp(yPos, 3, (floorHeight - 3) - roomHeight);
                break;
        }

        //Enemy room spawn chance random value
        int enemyRoomAmountChance = mapGeneration.enemyRoomAmountChance.Random;
        //Loop through the amount of enemies that can potentially spawn in this room
        for (int i = 0; i <= enemyRoomAmountChance; i++)
        {
            //Now randomize the chance of an enemy spawning
            int enemySpawnChance = Random.Range(0, mapGeneration.enemySpawnChance);

            //If the spawnChance is 0 then spawn a random lad in a random location
            if (enemySpawnChance == 0)
            {
                GameObject a = Instantiate(mapGeneration.enemyObjects[Random.Range(0, mapGeneration.enemyObjects.Length)], new Vector3(Random.Range(xPos, xPos + roomWidth) * tileScale, Random.Range(yPos, yPos + roomHeight) * tileScale, 0), Quaternion.identity);

            }
        }

        //item room spawn chance random value
        //The way item works will likely change to adjust to how our item system will work, consider this placeholder.
        //May also make a function for spawning entities instead of having the two separate and very similar enemy and item spawns
        int itemRoomAmountChance = mapGeneration.itemRoomAmountChance.Random;
        //loop through it and whatnot
        for (int i = 0; i <= itemRoomAmountChance; i++)
        {
            //Now randomize the chance of an item spawn
            int itemSpawnChance = Random.Range(0, mapGeneration.itemSpawnChance);

            //Same as enemy above as down here
            if (itemSpawnChance == 0)
            {
                GameObject a = Instantiate(mapGeneration.BaseItem, 
                                           new Vector3(Random.Range(xPos, xPos + roomWidth) * tileScale, 
                                                       Random.Range(yPos, yPos + roomHeight) * tileScale, 
                                                       0), 
                                           Quaternion.identity);
                a.GetComponent<WorldItem>().ItemFile = mapGeneration.itemObjects[Random.Range(0, mapGeneration.itemObjects.Length)];
            }

        }

        //Exit room & key placement conditions
        if (spawnExit)
        {
            GameObject a = Instantiate(mapGeneration.exitObject, new Vector3(Random.Range(xPos, xPos + roomWidth) * tileScale, Random.Range(yPos, yPos + roomHeight) * tileScale, 0), Quaternion.identity);
            a.GetComponent<ExitScript>().sceneToLoad = mapGeneration.nextScene;
            a.GetComponent<ExitScript>().isThisTutorial = mapGeneration.isThisTutorial;
        }

        if (spawnKey)
        {
            GameObject a = Instantiate(mapGeneration.BaseItem, new Vector3(Random.Range(xPos, xPos + roomWidth) * tileScale, Random.Range(yPos, yPos + roomHeight) * tileScale, 0), Quaternion.identity);
            a.GetComponent<WorldItem>().ItemFile = mapGeneration.keyItem;
            //a.transform.Find("RadarIcon").GetComponent<SpriteRenderer>().color = something i dunno
        }

        if (spawnShop)
        {
            /*
            GameObject a = Instantiate(mapGeneration.ShopWarp, new Vector3(Random.Range(xPos, xPos + roomWidth) * tileScale, Random.Range(yPos, yPos + roomHeight) * tileScale, 0), Quaternion.identity);
            GameObject b = Instantiate(mapGeneration.ShopLocation, new Vector3(-100, -100, 0), Quaternion.identity);
            a.GetComponent<WarpScript>().SetWarpPosition(b.transform.GetChild(1).transform.position); //set warp position to the warp in the shop's location
            b.transform.GetChild(1).GetComponent<WarpScript>().SetWarpPosition(a.transform.position); //make sure you can warp from the shop back to the dungeon.
            */
        }
    }
}
