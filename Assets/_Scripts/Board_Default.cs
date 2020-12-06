using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Default : MonoBehaviour
{
    public static BoardTile[,] MakeBoard(Vector2Int size)
    {
        
        Vector2 dimensionVec = size;
        if (dimensionVec == Vector2.zero) { dimensionVec = Vector2.one * 4; }
        float boardXSize = dimensionVec.x;
        float boardYSize = dimensionVec.y;
        BoardTile[,] board = new BoardTile[(int)boardXSize, (int)boardYSize];
        BoardMaker.BoardArea currentArea = BoardMaker.BoardArea.Middle; //this is used to make checking much easier
        float rotation = 0;
        //Tell cam Where middle is
        //load up a basic board size x board size chess board
        for (int x = 0; x < boardXSize; x++)
        {

            for (int y = 0; y < boardYSize; y++)
            {
                currentArea = BoardMaker.BoardArea.Middle;
                rotation = 0f;
                ///////Edge checks
                //check if Left edge
                if (x == 0 && y != 0 && y != boardYSize - 1)
                {
                    //0
                    currentArea = BoardMaker.BoardArea.Edge;
                    rotation = 0;
                }
                //check if right edge
                if (x == boardXSize - 1 && y != 0 && y != boardYSize - 1)
                {
                    //180
                    currentArea = BoardMaker.BoardArea.Edge;
                    rotation = 180f;


                }
                //check if Top edge
                if (y == 0 && x != 0 && x != boardXSize - 1)
                {

                    //-90
                    currentArea = BoardMaker.BoardArea.Edge;
                    rotation = 90f;

                }
                //check if bottom edge
                if (y == boardYSize - 1 && x != 0 && x != boardXSize - 1)
                {

                    //90
                    currentArea = BoardMaker.BoardArea.Edge;
                    rotation = -90f;

                }

                //Corners
                //bottom left
                if (x == 0 && y == 0)
                {
                    //0
                    currentArea = BoardMaker.BoardArea.Corner;
                    rotation = 90f;

                }
                //Bottom right
                if (x == boardXSize - 1 && y == 0)
                {
                    //90
                    currentArea = BoardMaker.BoardArea.Corner;
                    rotation = 180f;
                }
                //Top right
                if (x == boardXSize - 1 && y == boardYSize - 1)
                {
                    //180
                    currentArea = BoardMaker.BoardArea.Corner;
                    rotation = 270f;
                }
                //Top left
                if (x == 0 && y == boardYSize - 1)
                {
                    //270
                    currentArea = BoardMaker.BoardArea.Corner;
                    rotation =0f;
                }

                board[x, y] = new BoardTile(currentArea, rotation); 
            }
        }
        return board;
    }

    public static int[,] DrawShape(Vector2 size, float holeSize)
    {
        int[,] toReturn = new int[(int)size.x, (int)size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                toReturn[x, y] = 1;
            }
        }
        return toReturn;
    }
}
