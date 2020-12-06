using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Character : Unit_Base
{
    public List<KeyCode> movVals = new List<KeyCode>();
    public bool shot = false;
    public bool dead = false;
    public int playerNumb = 0;
    bool setUp = false;
    private void Start()
    {
        movVals = Data.instance.playInputs[playerNumb].inputs;
        type = UnitType.Character;
        myTrans = transform;
        myRend.color = Data.instance.artData.characterColour[playerNumb];
        setUp = true; //this executes one tick slower than the battlehandler, as such the battlehandler will hit update before we set up. As this is a quick and dirty product ive done this instead of fixing the core issue
    }

    public void Turn()
    {
        if (!setUp) { return; }
        Vector2Int mov = new Vector2Int(0,0);
        if (Input.GetKeyDown(movVals[0])) { mov = new Vector2Int(0, 1);} //W ^
        else
        if (Input.GetKeyDown(movVals[1])) { mov = new Vector2Int(0, -1);} //S v
        else
        if (Input.GetKeyDown(movVals[2])) { mov = new Vector2Int(-1, 0);} //A <
        else
        if (Input.GetKeyDown(movVals[3])) { mov = new Vector2Int(1, 0);} //D >

        if (mov != Vector2Int.zero)
        {
            if (shot)
            {
                if (BattleHandler.instance.RequestPosUpdate(this, mov)) { BattleHandler.instance.NextTurn(); }
            }
            else
            {
                shot = BattleHandler.instance.ShootUpdate(this, mov);
                if (shot) { BattleHandler.instance.MSTextHandler(true); }
            }
        }

    }

    public Unit_Character Die()
    {
        dead = true;
        myRend.sprite = Data.instance.artData.dead;
        return this;
    }
}
