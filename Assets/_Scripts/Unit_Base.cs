using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Unit_Base : MonoBehaviour
{
    //Unit base is the core for all things that work on the board
    //Walls
    //Characters
    //Bullets
    //Reflectors
    public enum UnitType { Block, Character, Bullet, Reflector }
    public UnitType type;
    public Vector2Int indexPos;
    public Transform myTrans;
    public SpriteRenderer myRend;

    public virtual void Move(Vector3 newPos, Vector2Int newIndex)
    {
        myTrans.position = newPos;
        indexPos = newIndex;
    }
}
