using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Bullet : Unit_Base
{
    public Vector2Int direction;
    public bool xploded = false;
    public bool bouncin = false;
    
    private void Start()
    {
        type = UnitType.Bullet;
    }

    public bool Turn()
    {
        if (bouncin) {myRend.sprite = Data.instance.artData.bullet;}
        if (xploded) {return true; }
        if (!BattleHandler.instance.RequestPosUpdate(this, direction))
        {
            //Explode
            //GetComponent<SpriteRenderer>().sprite = Data.instance.artData.XplodeWall;
            BattleHandler.instance.gameBoard[indexPos.x, indexPos.y] = null;
            xploded = true;
        }
        return false;
    }

}
