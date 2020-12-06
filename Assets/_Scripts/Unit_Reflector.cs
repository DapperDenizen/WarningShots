using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Reflector : Unit_Base
{
    Vector2Int[] reflectionDirections = { };

    // Start is called before the first frame update
    void Start()
    {
        type = UnitType.Reflector;
        switch (myTrans.eulerAngles.z)
        {
            case 0: reflectionDirections = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(-1, 0) }; break;
            case 90: reflectionDirections = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, -1) }; break;
            case 180: reflectionDirections = new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(1, 0) };  break;
            case 270: reflectionDirections = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 1) }; break;
        }

    }

    public Vector2Int ExplodeOrBounce(Vector2Int incoming)
    {
        Vector2Int adjustedInc = indexPos - incoming;
        if (adjustedInc == reflectionDirections[0])
        {
            return reflectionDirections[1] * -1;
        }
        else if (adjustedInc == reflectionDirections[1])
        {
            return reflectionDirections[0]*-1;
        }
        return Vector2Int.zero; //this means it Explodes
    }

}
