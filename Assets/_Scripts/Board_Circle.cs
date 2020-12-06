using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This and all of the Board maker scripts have been used in a lot of differing environments so the internal directions in the scripts may not work in the actual game (so south here may be East there)

public class Board_Circle : MonoBehaviour
{


    public static BoardTile[,] MakeBoard(Vector2Int size, float donutHoleSize)
    {

        Vector2 dimensionVec = size;
        if (dimensionVec == Vector2.zero) { dimensionVec = Vector2.one * 4; }
        float boardXSize = dimensionVec.x;
        float boardYSize = dimensionVec.y;
        BoardTile[,] board = new BoardTile[(int)boardXSize, (int)boardYSize];
        BoardMaker.BoardArea currentArea = BoardMaker.BoardArea.Middle; //this is used to make checking much easier
        float rotation = 0;
        int[,] circleArray = DrawShape(dimensionVec, donutHoleSize);
        //Tell cam Where middle is
        //load up a basic board size x board size chess board

        for (int x = 0; x < boardXSize; x++)
        {

            for (int y = 0; y < boardYSize; y++)
            {

                if (circleArray[x, y] == 1)
                {
                    bool study = false;
                    //if (x == 1 && y == 2) { print("Case study!"); study = true; }
                    //Get type
                    int tileValue = 0;
                    if (y + 1 < dimensionVec.y) { tileValue += circleArray[x, y + 1];  }
                    //if (study) print("y + 1 = " + tileValue);
                    if (y - 1 >= 0) { tileValue += circleArray[x, y - 1]; }
                   // if (study) print("y - 1 = " + tileValue);
                    if (x + 1 < dimensionVec.x) { tileValue += circleArray[x + 1, y]; }
                    //if (study) print("x + 1 = " + tileValue);
                    if (x - 1 >= 0) { tileValue += circleArray[x - 1, y]; }
                    //if (study) print("x - 1 = " + tileValue);
                    //if (study)print("Study results = " + tileValue);
                    switch (tileValue)
                    {
                        
                        case 2: currentArea = BoardMaker.BoardArea.Corner; break;
                        case 3: currentArea = BoardMaker.BoardArea.Edge; break;
                        default: currentArea = BoardMaker.BoardArea.Middle; break;

                    }

                    // get rotation
                    rotation = CalcRotation(circleArray, new Vector2Int(x, y), currentArea);  //CalcRotation(new Vector2Int(x, y), dimensionVec, currentArea);
                    board[x, y] = new BoardTile(currentArea, rotation);
                }
                else
                {
                    board[x, y] = new BoardTile(BoardMaker.BoardArea.Empty, 0);
                }
            }
        }
        return board;
    }

    static float CalcRotation(int[,] array, Vector2Int gridIndex, BoardMaker.BoardArea type)
    {
        //middle
        if (type == BoardMaker.BoardArea.Middle) { return 0; }

        int north, south, east, west;
        north = south = west = east = 0;

        if (gridIndex.y + 1 < array.GetLength(1)) { north = array[gridIndex.x, gridIndex.y + 1]; }
        if (gridIndex.y - 1 >= 0) { south = array[gridIndex.x, gridIndex.y - 1]; }
        if (gridIndex.x + 1 < array.GetLength(0)) { east = array[gridIndex.x + 1, gridIndex.y]; }
        if (gridIndex.x - 1 >= 0) { west = array[gridIndex.x - 1, gridIndex.y]; }

        if (type == BoardMaker.BoardArea.Corner)
        {
            

            if(north + west == 0) { return 0; }
            if(south + west == 0) { return 90; }
            if(north + east == 0) { return 270; }
            if(south + east == 0) { return 180; }
        }
        else
        {
            if (north == 0) { return 270; }
            if (east == 0) { return 180; }
            if (west == 0) { return 0; }
            if (south == 0) { return 90; }
        }

        //shouldn't reach here
        return 0;

    }
        



    //Difficult Circle calculations

    


    //Calculates the distance from the center point to XY
    static float distanceCalc(float x, float y, float ratio)
    {
        return Mathf.Sqrt((Mathf.Pow(y * ratio, 2)) + Mathf.Pow(x, 2));
    }
    //Checks if this square is a valid circle position, if true it is
    static bool IsValidTile(float x, float y, float ratio, float radius, float donutHole)
    {
        float checker = distanceCalc(x, y, ratio);
        return (checker <= radius && checker > donutHole);
    }
    //Calculates the rotation of Edge and Corner pieces 
    static float CalcRotation(Vector2Int gridIndex, Vector2 gridMax, BoardMaker.BoardArea type)
    {
        if (type.Equals(BoardMaker.BoardArea.Middle)) { return 0; }

        if (type.Equals(BoardMaker.BoardArea.Corner))
        {
            //Corner
            if (gridIndex.y < gridMax.y / 2 - 1 && gridIndex.x > gridMax.x / 2 - 1)
            {
                //bottom right quadrant
                return 180;//0;
            }
            else if (gridIndex.y > gridMax.y / 2 - 1 && gridIndex.x > gridMax.x / 2 - 1)
            {
                //bottom left quadrant
                return 270f;
            }
            else if (gridIndex.y > gridMax.y / 2 - 1 && gridIndex.x < gridMax.x / 2 - 1)
            {
                //top left quadrant
                return 0;// 180f;
            }
            else
            {
                //top right quadrant
                return 90f;
            }
        }
        else
        {
            //Edge
            //North
            int northDist = (int)gridMax.x - gridIndex.x;
            //East
            int eastDist = gridIndex.y;
            //South
            int southDist = gridIndex.x;
            //West
            int westDist = (int)gridMax.y - gridIndex.y;
            //Smallest <Couldnt figure out a sexier way to do this, im a lil tired tho so i dont feel too bad>
            int smallDist = Mathf.Min(new int[] { northDist, eastDist, southDist, westDist });

            if (smallDist == northDist) { return 180; }
            if (smallDist == eastDist) { return 90f; }
            if (smallDist == southDist) { return 0f; }
            if (smallDist == westDist) { return 270f; }
            //Shouldnt reach here...
            return 0;

        }
    }
    //Draw circle
    public static int[,] DrawShape(Vector2 size, float holeSize)
    {
        
        int[,] toReturn = new int[(int)size.x, (int)size.y];

        //Circle variables
        float radiusX = size.x / 2;
        float radiusY = size.y / 2;
        float boardXSize;
        float boardYSize;
        //Circle variables
        float ratio = radiusX / radiusY;
        float radius = radiusX > radiusY ? radiusX : radiusY;
        if ((radiusX * 2) % 2 == 0)
        {
            boardXSize = Mathf.Ceil(radiusX - 0.5f) * 2 + 1;
        }
        else
        {
            boardXSize = Mathf.Ceil(radiusX) * 2;
        }
        if ((radiusY * 2) % 2 == 0)
        {
            boardYSize = Mathf.Ceil(radiusY - 0.5f) * 2 + 1;
        }
        else
        {
            boardYSize = Mathf.Ceil(radiusY) * 2;
        }
        //Circle variables
        int actualX = 0;
        int actualY = 0;
        for (float y = -boardYSize / 2 + 1; y < boardYSize / 2; y++)
        {
            for (float x = -boardXSize / 2 + 1; x < boardXSize / 2; x++)
            {
                if (IsValidTile(x, y, ratio, radius, holeSize))
                {

                    toReturn[actualX, actualY] = 1;
                }
                actualX++;
            }
            actualX = 0;
            actualY++;
        }

        /*
        for (int iy = 0; iy < boardYSize-1; iy++)
        {
            string temp = "";
            for (int ix = 0; ix < boardXSize-1; ix++)
            {
                temp += toReturn[ix, iy] + " |";
            }
            print(temp);
        }
        //*/


        return toReturn;
    }
}
