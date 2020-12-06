using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public static class BoardMaker
{
    public enum BoardType { Rectangle, Circle };
    public enum BoardArea { Middle, Edge, Corner, Empty };
    public static float tileSize =1;

    public static Unit_Base[,] MakeBoard( Data_Board boardData)
    {
        Transform holder = new GameObject("Board").GetComponent<Transform>();
        Unit_Base[,] boardGameplay = new Unit_Base[boardData.boardSize.x, boardData.boardSize.y];
        BoardTile[,] boardVisual;

        switch (boardData.boardType)
        {
            
            case BoardType.Circle: boardVisual = Board_Circle.MakeBoard(boardData.boardSize,boardData.secondaryFloat); break;
            default: boardVisual = Board_Default.MakeBoard(boardData.boardSize); break;
        }

        float xS = -(boardVisual.GetLength(1) / 2f);
        float yS = -(boardVisual.GetLength(0) / 2f);
        Vector2 startPos = new Vector2(yS * tileSize, xS * tileSize);

        for (int x = 0; x < boardVisual.GetLength(0); x++)
        {
            for (int y = 0; y < boardVisual.GetLength(1); y++)
            {
                Vector2 location = new Vector2(startPos.x + (tileSize * x), startPos.y + (tileSize * y));
                SpriteRenderer temp = GameObject.Instantiate(Data.instance.tileObj, location, Quaternion.Euler(new Vector3(0,0,boardVisual[x,y].rotation)),holder).GetComponent<SpriteRenderer>();
                temp.color = (x + y) % 2 == 0 ? Data.instance.artData.tileBlack : Data.instance.artData.tileWhite;
                if (temp.GetComponentInChildren<TextMeshPro>() != null)
                {
                    temp.GetComponentInChildren<TextMeshPro>().text = x + "," + y;
                }
                switch (boardVisual[x, y].areaType)
                {
                    case BoardArea.Corner: temp.sprite = Data.instance.artData.corner;  break;
                    case BoardArea.Edge: temp.sprite = Data.instance.artData.edge; break;
                    case BoardArea.Middle: temp.sprite = Data.instance.artData.middle; break;
                    case BoardArea.Empty: temp.color = Data.instance.artData.tileBlock; boardGameplay[x, y] =  temp.gameObject.AddComponent<Unit_Block>(); break;
                }
            }
        }



        return boardGameplay;
    }
}

public struct BoardTile
{
    public BoardMaker.BoardArea areaType;
    public float rotation;

    public BoardTile(BoardMaker.BoardArea aType, float rot)
    {
        areaType = aType;
        rotation = rot;
    }
}
