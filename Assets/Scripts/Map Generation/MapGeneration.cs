//Code by Jonathan Babtiste

using System.Collections.Generic;
using UnityEngine;
//using AI_Midterm_XNA;
//using UnityEngine.TileMaps; //Commented out due to not being sure whether I should use tilemaps or not

public class MapGeneration : MonoBehaviour
{
    public enum TileType
    {
        Blank, Ground, RightWall, LeftWall, BottomWall, TopWall, BottomLeftCorner, BottomRightCorner, TopLeftCorner, TopRightCorner, HorizontalWallStrip, VerticalWallStrip, RightCap, LeftCap, BottomCap, TopCap, MiddleWall, Outer
    };

    public int randomSeed; //The seed used to create map
    public string nextScene; //what scene is loaded on the exit
    public bool isThisTutorial;

    [Header("Floor Generation")]
    public IntRange floorWidth = new IntRange(100, 100); //Floor width and height range (please do not use low numbers lmao)
    public IntRange floorHeight = new IntRange(100, 100);
    public IntRange roomAmount = new IntRange(6, 12);

    [Header("Room Generation")]
    public IntRange roomWidth = new IntRange(3, 10); //Room mwidth and height range
    public IntRange roomHeight = new IntRange(3, 10);

    [Header("Corridor Generation")]
    public IntRange corridorLength = new IntRange(6, 10); //How long the corridors can be
    public IntRange corridorWidth = new IntRange(1, 3); //How wide the corridor is

    [Header("Tile Selection")]
    public Sprite[] groundTiles;
    public Sprite[] rightWallTiles;
    public Sprite[] leftWallTiles;
    public Sprite[] bottomWallTiles;
    public Sprite[] topWallTiles;
    public Sprite[] bottomLeftCornerWallTiles;
    public Sprite[] topLeftCornerWallTiles;
    public Sprite[] bottomRightCornerWallTiles;
    public Sprite[] topRightCornerWallTiles;
    public Sprite[] horizontalStripWallTiles;
    public Sprite[] verticalStripWallTiles;
    public Sprite[] middleWallTiles;
    public Sprite[] rightCapWallTiles;
    public Sprite[] leftCapWallTiles;
    public Sprite[] bottomCapWallTiles;
    public Sprite[] topCapWallTiles;
    public Sprite[] outerTiles;

    [Header("EnemyObjects")]
    public GameObject[] enemyObjects;
    public IntRange enemyRoomAmountChance = new IntRange(1, 2); //This represents how many enemies can spawn in a room.
    public int enemySpawnChance = 10; //Chance of an enemy to spawn in a room (The higher the number the less the chance)

    [Header("ItemObjects")] //This reflects how the enemy objects work, it's pretty much the same thing repeated for items this time. 
    public GameObject BaseItem;
    public Item[] itemObjects;
    public IntRange itemRoomAmountChance = new IntRange(1, 3);
    public int itemSpawnChance = 5;
    public Item keyItem;

    [Header("Shop")] //Shop related stuff here
    public GameObject ShopLocation;
    public GameObject ShopWarp;
    public int shopSpawnChance = 2;


    [Header("Objects")]
    public GameObject player;
    public Inventory inventory;
    public GameObject exitObject;
    public Sprite radarTile;

    private TileType[][] tiles;
    private Vector3[,] tileLocs;
    private Room[] rooms;
    private Corridor[] corridors;
    private GameObject levelData;
    private GameObject radarData;

    public int columns;
    public int rows;

    public float tileScale;
    public float randomWeight;

    public Material material;

    public delegate void OnMapGenerated();
    public OnMapGenerated mapGenerated;


    // Start is called before the first frame update
    void Start()
    {
        if (randomSeed > 0)
        {
            Random.InitState(randomSeed);
        }

        if (!isThisTutorial)
        {
            PlayerStats stats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
            roomAmount.maxNum = stats.maxRooms;
            enemyRoomAmountChance.maxNum = stats.maxEnemies;
            if (stats.currentLevel >= 5)
            {
                nextScene = "Win";
            }
        }

        columns = floorWidth.Random;
        rows = floorHeight.Random;

        tileScale = groundTiles[0].bounds.size.x;

        levelData = new GameObject("levelData");
        radarData = new GameObject("radarData");

        tileLocs = new Vector3[columns, rows];

        SetupTilesArray();

        GenerateMap();

        SetTileValuesForRooms();
        SetTileValuesForCorridors();
        SetWallTileValues();

        CreateTiles();

        CreateRadarMap();


        PathFinder.Init(this);

        if(mapGenerated != null)
        {
            mapGenerated();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupTilesArray()
    {
        tiles = new TileType[columns][]; //Set array with the random width

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[rows]; //Now set the random height
        }
    }

    void GenerateMap()
    {
        //Create the array of rooms
        rooms = new Room[roomAmount.Random];
        int exitRoom = Random.Range(1, rooms.Length - 1); //This chooses which room will become an exit room
        int keyRoom = Random.Range(1, rooms.Length - 1); //Chooses the room that will hold the key that must generate
        int shopRoom = Random.Range(1, rooms.Length - 1); //Chooses the room with the shop warp tile.
        bool spawnExit = false;
        bool spawnKey = false;
        bool spawnShop = false;

        //Create the array of corridors based on room's length
        corridors = new Corridor[rooms.Length - 1];

        //Create first room and corridor
        rooms[0] = new Room();
        corridors[0] = new Corridor();


        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows, player, inventory, this);

        //Generate the first corridor
        corridors[0].SetupCorridor(rooms[0], corridorLength, corridorWidth, roomWidth, roomHeight, columns, rows, true);

        //Starting loop for rooms (starts at 1 cause the first room is already made
        for (int i = 1; i < rooms.Length; i++)
        {
            //Create room
            rooms[i] = new Room();

            //If current room matches exitRoom/keyRoom/shopRoom value, make this room be the one to generate the corresponding thing
            if (i == exitRoom)
                spawnExit = true;
            if (i == keyRoom)
                spawnKey = true;
            if (i == shopRoom)
                spawnShop = true;

            //Setup the room that was created (now with corridor data!)
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1], this, spawnExit, spawnKey, spawnShop);
            spawnExit = false; //Once setupRoom is done make sure spawnExit/spawnKey/spawnShop is false before coming back in
            spawnKey = false;
            spawnShop = false;

            //as lng as we havet reached the end of the corridors array
            if (i < corridors.Length)
            {
                //create corridor
                corridors[i] = new Corridor();

                //Setup the corridor based on the room that was just created
                corridors[i].SetupCorridor(rooms[i], corridorLength, corridorWidth, roomWidth, roomHeight, columns, rows, false);
            }
        }
    }

    void SetTileValuesForRooms()
    {
        //Go through the rooms
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            //Go through room's width
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j; //int value for our current x position

                //Go through room's height
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k; //int value for our current y position

                    //Set tile value for that specific coordinate
                    tiles[xCoord][yCoord] = TileType.Ground;
                }
            }
        }
    }

    void SetTileValuesForCorridors()
    {
        int widthDirection = 0;
        int randNum = Random.Range(0, 2); //I'm jsut doing this so it is either negative or not negative
        if (randNum == 0)
        {
            widthDirection = 1;
        }
        if (randNum == 1)
        {
            widthDirection = -1;
        }

        //Go through the corridors
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            //Depending on the Direction, loop in a specific way to generate the corridor to its end point along with whatever width it has
            switch (currentCorridor.direction)
            {
                case Direction.North:
                    for (int j = 0; j < currentCorridor.corridorLength; j++)
                    {
                        int yCoord = currentCorridor.startYPos + j;

                        for (int k = 0; k < currentCorridor.corridorWidth; k++)
                        {
                            int xCoord = currentCorridor.startXPos + (k * widthDirection);
                            tiles[xCoord][yCoord] = TileType.Ground;
                        }
                    }
                    break;

                case Direction.East:
                    for (int j = 0; j < currentCorridor.corridorLength; j++)
                    {
                        int xCoord = currentCorridor.startXPos + j;

                        for (int k = 0; k < currentCorridor.corridorWidth; k++)
                        {
                            int yCoord = currentCorridor.startYPos + (k * widthDirection);
                            tiles[xCoord][yCoord] = TileType.Ground;
                        }
                    }
                    break;

                case Direction.South:
                    for (int j = 0; j < currentCorridor.corridorLength; j++)
                    {
                        int yCoord = currentCorridor.startYPos - j;

                        for (int k = 0; k < currentCorridor.corridorWidth; k++)
                        {
                            int xCoord = currentCorridor.startXPos + (k * widthDirection);
                            tiles[xCoord][yCoord] = TileType.Ground;
                        }
                    }
                    break;

                case Direction.West:
                    for (int j = 0; j < currentCorridor.corridorLength; j++)
                    {
                        int xCoord = currentCorridor.startXPos - j;

                        for (int k = 0; k < currentCorridor.corridorWidth; k++)
                        {
                            int yCoord = currentCorridor.startYPos + (k * widthDirection);
                            tiles[xCoord][yCoord] = TileType.Ground;
                        }
                    }
                    break;
            }


            //Go through the corridor's length (because right now it dont got a width or height)
            /*for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                //Depending on which direction grab the coord for how long the length is
                switch(currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }
                
                //and now set the tiletype
                tiles[xCoord][yCoord] = TileType.Ground;
            }*/
        }
    }

    //Now that we have all of the Ground tiles set, set wall tiles based on where the ground tiles are
    void SetWallTileValues()
    {
        List<Vector2> ChangeToWallTileList = new List<Vector2>();
        List<TileType> WhichWallTileList = new List<TileType>();

        //Go through all tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                //if this is a wall tile, save the tile position into a list and save what type of tile that is into another list
                if (IsThisAWallTile(i, j) != TileType.Ground && IsThisAWallTile(i, j) != TileType.Blank)
                {
                    ChangeToWallTileList.Add(new Vector2(i, j));
                    WhichWallTileList.Add(IsThisAWallTile(i, j));
                }
            }
        }

        //Go through the list of tiles saved and change them all into their specific wall tiles
        for (int k = 0; k < ChangeToWallTileList.Count; k++)
        {
            tiles[(int)ChangeToWallTileList[k].x][(int)ChangeToWallTileList[k].y] = WhichWallTileList[k];
        }
    }

    void CreateTiles()
    {
        //Go through all tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                //Create ground tile if it is one
                if (tiles[i][j] == TileType.Ground)
                {
                    InstantiateFromArray(groundTiles, i, j);
                }

                //Create wall tile if it is one
                if (tiles[i][j] == TileType.RightWall)
                {
                    InstantiateFromArray(rightWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.LeftWall)
                {
                    InstantiateFromArray(leftWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.TopWall)
                {
                    InstantiateFromArray(topWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.BottomWall)
                {
                    InstantiateFromArray(bottomWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.BottomLeftCorner)
                {
                    InstantiateFromArray(bottomLeftCornerWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.TopLeftCorner)
                {
                    InstantiateFromArray(topLeftCornerWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.BottomRightCorner)
                {
                    InstantiateFromArray(bottomRightCornerWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.TopRightCorner)
                {
                    InstantiateFromArray(topRightCornerWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.HorizontalWallStrip)
                {
                    InstantiateFromArray(horizontalStripWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.VerticalWallStrip)
                {
                    InstantiateFromArray(verticalStripWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.MiddleWall)
                {
                    InstantiateFromArray(middleWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.RightCap)
                {
                    InstantiateFromArray(rightCapWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.LeftCap)
                {
                    InstantiateFromArray(leftCapWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.TopCap)
                {
                    InstantiateFromArray(topCapWallTiles, i, j, true);
                }
                if (tiles[i][j] == TileType.BottomCap)
                {
                    InstantiateFromArray(bottomCapWallTiles, i, j, true);
                }

                //create outer tiles
                if (tiles[i][j] == TileType.Blank)
                {
                    InstantiateFromArray(outerTiles, i, j);
                }
            }
        }
    }

    void CreateRadarMap()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] == TileType.Ground) //For every ground tile make a radar tile there as well specifically for the radar.
                {
                    GameObject a = new GameObject("RadarTile(" + i + ", " + j + ")");
                    a.layer = 10; //make sure they are in the right layer
                    a.AddComponent<SpriteRenderer>();
                    a.GetComponent<SpriteRenderer>().sprite = radarTile;
                    a.AddComponent<RadarTileScript>();
                    a.GetComponent<RadarTileScript>().activeDistance = 8;
                    a.GetComponent<SpriteRenderer>().color = new Vector4(.6f, .6f, .6f, 0);
                    a.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    a.transform.position = new Vector3(i * tileScale, j * tileScale, 0f); //tiles should mimic the location of real tiles
                    a.transform.rotation = Quaternion.identity;
                    a.transform.parent = radarData.transform; //set tile's parent to radarData
                }
            }
        }
    }


    /* //The intention for this is to fill in all of the tiles not assigned to anything into outer tiles
    void CreateOuterTiles()
    {
        
    }*/

    //(If we decide to do tilemaps the gameobject array would instead be a tile array
    void InstantiateFromArray(Sprite[] prefabs, float xCoord, float yCoord, bool isWall = false)
    {
        //Create a random index for the array
        int randomIndex = WeightedRandom.Random(prefabs, randomWeight);

        //Set the position to instantiate tile
        Vector3 position = new Vector3(xCoord * tileScale, yCoord * tileScale, 0f);

        //Create the instance of the tile at the position
        GameObject a = new GameObject("Tile(" + xCoord + ", " + yCoord + ")");
        a.AddComponent<SpriteRenderer>().sprite = prefabs[randomIndex];
        a.GetComponent<SpriteRenderer>().material = material;
        //a.AddComponent<RenderDistance>();
        if (isWall)
        {
            a.AddComponent<Rigidbody2D>();
            a.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            a.AddComponent<CompositeCollider2D>();
            a.AddComponent<BoxCollider2D>();
            a.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0); //Make sure offset and size of collider reflect tile size
            a.GetComponent<BoxCollider2D>().size = new Vector2(tileScale, tileScale);
            a.GetComponent<BoxCollider2D>().usedByComposite = true;
        }
        a.transform.position = position;
        a.transform.rotation = Quaternion.identity;
        a.GetComponent<SpriteRenderer>().sortingOrder = (int)a.transform.position.y * -1; //set the layer to be equal to the y position, but as a negative value

        tileLocs[(int)xCoord, (int)yCoord] = a.transform.position;

        
        //GameObject tileInstance = Instantiate(a, levelData.transform);

        //Set the tile instance's parent to the level data
        a.transform.parent = levelData.transform;
    }

    public TileType IsThisAWallTile(int xCoord, int yCoord)
    {
        //If the coordinate being checked is on the edges of the level, do nothin
        if (xCoord == 0 || yCoord == 0 || xCoord == columns - 1 || yCoord == rows - 1)
        {
            return TileType.Blank;
        }

        //If the tile being checked is a ground tile then you aint changing it chief
        if (tiles[xCoord][yCoord] == TileType.Ground)
        {
            return TileType.Ground;
        }

        TileType topTile = tiles[xCoord][yCoord + 1];
        TileType bottomTile = tiles[xCoord][yCoord - 1];
        TileType leftTile = tiles[xCoord - 1][yCoord];
        TileType rightTile = tiles[xCoord + 1][yCoord];

        //These 15 if statements all check 4 the case that there is only one ground tile (or two corner ground tiles) in the surrounding tiles. In this case, this must be a wall tile
        if (rightTile == TileType.Ground && leftTile == TileType.Blank && bottomTile == TileType.Blank && topTile == TileType.Blank)
        {
            return TileType.LeftWall; //Wall tile (specifically the left wall)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Ground && bottomTile == TileType.Blank && topTile == TileType.Blank)
        {
            return TileType.RightWall; //Wall tile (specifically the right wall)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Blank && bottomTile == TileType.Ground && topTile == TileType.Blank)
        {
            return TileType.TopWall; //Wall tile (specifically the top wall)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Blank && bottomTile == TileType.Blank && topTile == TileType.Ground)
        {
            return TileType.BottomWall; //Wall tile (specifically the bottom wall)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Blank && bottomTile == TileType.Blank && topTile == TileType.Ground)
        {
            return TileType.BottomLeftCorner; //Wall tile (specifically the bottom left corner)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Ground && bottomTile == TileType.Blank && topTile == TileType.Ground)
        {
            return TileType.BottomRightCorner; //Wall tile (specifically the bottom right corner)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Ground && bottomTile == TileType.Ground && topTile == TileType.Blank)
        {
            return TileType.TopRightCorner; //Wall tile (specifically the top right corner)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Blank && bottomTile == TileType.Ground && topTile == TileType.Blank)
        {
            return TileType.TopLeftCorner; //Wall tile (specifically the top left corner)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Ground && bottomTile == TileType.Blank && topTile == TileType.Blank)
        {
            return TileType.VerticalWallStrip; //Wall tile (specfically the wall in between vertical paths)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Blank && bottomTile == TileType.Ground && topTile == TileType.Ground)
        {
            return TileType.HorizontalWallStrip; //Wall tile (specifically the wall in between horizontal paths)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Ground && bottomTile == TileType.Ground && topTile == TileType.Ground && tiles[xCoord][yCoord] == TileType.Blank)
        {
            return TileType.MiddleWall; //Wall tile (specifically the wall sourrounded by ground tiles)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Ground && bottomTile == TileType.Ground && topTile == TileType.Blank)
        {
            return TileType.BottomCap; //Wall tile (specifically with right, left & bottom as ground)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Ground && bottomTile == TileType.Blank && topTile == TileType.Ground)
        {
            return TileType.TopCap; //Wall tile (specifically with right, left & top as ground)
        }
        if (rightTile == TileType.Blank && leftTile == TileType.Ground && bottomTile == TileType.Ground && topTile == TileType.Ground)
        {
            return TileType.LeftCap; //Wall tile (specifically with left, top & bottom as ground)
        }
        if (rightTile == TileType.Ground && leftTile == TileType.Blank && bottomTile == TileType.Ground && topTile == TileType.Ground)
        {
            return TileType.RightCap; //Wall tile (specifically with right, top & bottom as ground)
        }

        return TileType.Blank; //If none of the above statements exist, do nothin
    }

    public Vector2 getTileFromPosition(float xPos, float yPos)
    {
        float xCoord = (xPos / tileScale);
        float yCoord = (yPos / tileScale);

        return new Vector2( Mathf.Round(xCoord), Mathf.Round(yCoord));
    }

    public Vector2 getTileFromPosition(Vector3 position)
    {
        float xCoord = (position.x / tileScale);
        float yCoord = (position.y / tileScale);

        return new Vector2(Mathf.Round(xCoord), Mathf.Round(yCoord));
    }

    public TileType getTileTypeFromPosition(float xPos, float yPos)
    {
        float xCoord = (xPos / tileScale);
        float yCoord = (yPos / tileScale);

        return tiles[(int)Mathf.Round(xCoord)][(int)Mathf.Round(yCoord)];
    }

    public TileType getTileTypeFromTilePosition(int xpos, int ypos)
    {
        return tiles[xpos][ypos];
    }

    public Vector3 getTilePosition(int x, int y)
    {
        return tileLocs[x, y];
    }
    
    public Vector3 getTilePosition(Vector2 input)
    {
        return tileLocs[(int)input.x, (int)input.y];
    }

    public Vector3 getClosestTile(Vector3 position)
    {
        return getTilePosition(getTileFromPosition(position));
    }
}
