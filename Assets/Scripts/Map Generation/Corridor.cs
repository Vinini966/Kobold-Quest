using UnityEngine;

//The direction this corridor is moving in
public enum Direction
{
    North, East, South, West,
}


public class Corridor
{
    public int startXPos; //x position for the start of the corridor
    public int startYPos; //y position for the start of the corridor
    public int corridorLength; //Value for how long the corridor will be
    public int corridorWidth; //Value for how wide the corridor will be
    public Direction direction; //Which direction is the corridor going from the room it comes from

    //This will get the end X position of the corridor based on its start position and direction
    public int EndPositionX
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return startXPos;
            }
            if (direction == Direction.East)
            {
                return startXPos + corridorLength - 1;
            }
            return startXPos - corridorLength + 1; //This line is checking West
        }
    }

    //This will get the end Y position of the corridor based on its start position and direction
    public int EndPositionY
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
            {
                return startYPos;
            }
            if (direction == Direction.North)
            {
                return startYPos + corridorLength - 1;
            }
            return startYPos - corridorLength + 1; //This line is checking South
        }
    }

    //Method for Corridor creation
    public void SetupCorridor(Room room, IntRange length, IntRange width, IntRange roomWidth, IntRange roomHeight, int floorWidth, int floorHeight, bool firstCorridor)
    {
        //First, set a random direction
        direction = (Direction)Random.Range(0, 4);

        /*//Math time, what the line below is doing is getting the direction opposite to the room is connecting to. (commented out cause it's cooler without it)
        //We want to know this because with this information we can prevent corridors doubling back on themselves.
        //So if the corridor entering the room passed in was coming from the North we want the opposite direction to be South.
        //In North's case the math would look like:    0 + 2 = 2.    2 % 4 = 2.    Direction(2) = South.   We have the opposite.
        Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);

        //If it's not the first corridor and the direction is the same as the oppositeDirection value
        //Basically this is catching our double-back situation
        if (!firstCorridor && direction == oppositeDirection)
        {
            int directionInt = (int)direction;
            directionInt++; //rotate direction 90 degrees
            directionInt = directionInt % 4; //and mod it so we cant get a value of 5
            direction = (Direction)directionInt; 
        }*/

        //set the corridor's length
        corridorLength = length.Random;

        //set the corridor's width
        corridorWidth = width.Random;

        //create a variable for the max length
        int maxLength = length.maxNum;
        int maxWidth = width.maxNum;

        //Now we do stuff based on which direction we got
        switch (direction)
        {
            case Direction.North:
                //startXPos can be random just needs to be within range of the room
                startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth - 1);
                startXPos = Mathf.Clamp(startXPos, maxWidth, floorWidth - maxWidth); //clamp the width xPos of the corridor based on maxWidth of corridor
                startYPos = room.yPos + room.roomHeight; //yPos needs ot be on top though

                //max length the corridor can be is the floor height, but also with taking the room height into account
                maxLength = (floorHeight - 3) - startYPos - roomHeight.minNum;
                break;
            //Same logic applies to each case, just with different values of course
            case Direction.East:
                startXPos = room.xPos + room.roomWidth;
                startYPos = Random.Range(room.yPos, room.yPos + room.roomHeight - 1);
                startYPos = Mathf.Clamp(startYPos, maxWidth, floorHeight - maxWidth);
                maxLength = (floorWidth - 3) - startXPos - roomWidth.minNum;
                break;

            case Direction.South:
                startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth);
                startXPos = Mathf.Clamp(startXPos, maxWidth, floorWidth - maxWidth);
                startYPos = room.yPos;
                maxLength = 3 + startYPos - roomHeight.minNum ;
                break;

            case Direction.West:
                startXPos = room.xPos;
                startYPos = Random.Range(room.yPos, room.yPos + room.roomHeight);
                startYPos = Mathf.Clamp(startYPos, maxWidth, floorHeight - maxWidth);
                maxLength = 3 + startXPos - roomWidth.minNum;
                break;
        }

        //Clamping the length just to make sure it can't go outside the parameters of the floor
        corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);

        //Clamp the width of the corridor to make sure it can't go outside the parameters of the floor and room
        //Corridor width should never be wider than the room itself

    }
}
