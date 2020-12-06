using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour
{

    public static BattleHandler instance;
    //Board
    public Unit_Base[,] gameBoard;
    //Characters
    List<Unit_Character> chars = new List<Unit_Character>();
    List<Unit_Bullet> bullets = new List<Unit_Bullet>();
    [SerializeField]int charTurn = 0;
    [SerializeField] SpriteRenderer background;
    //
    [SerializeField]TextMeshProUGUI winTxt, msTxt;
    bool gameDone = false;
    // Start is called before the first frame update
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Data_Board boardDat = Data.instance.currBoard;
        gameBoard = BoardMaker.MakeBoard(boardDat);
        foreach (Vector2Int charPos in boardDat.playerPositions)
        {
            AddCharacter(charPos);
        }
        foreach (ReflectorZones reflectorZ in boardDat.refZones)
        {
            AddReflector(reflectorZ.pos, reflectorZ.rot);
        }
        background.color = Data.instance.artData.characterColour[charTurn];
    }

    void AddReflector(Vector2Int pos, float rotation)
    {
        gameBoard[pos.x, pos.y] = Instantiate(Data.instance.reflectorObj, BoardToWorld(new Vector2Int(pos.x, pos.y),-5f), Quaternion.Euler(0,0,rotation)).GetComponent<Unit_Reflector>();
        gameBoard[pos.x, pos.y].indexPos = pos;
    }

    void AddCharacter(Vector2Int pos)
    {
        chars.Add(Instantiate(Data.instance.mushroomObj, BoardToWorld(new Vector2Int(pos.x, pos.y),-5f), Quaternion.identity).GetComponent<Unit_Character>());
        gameBoard[pos.x, pos.y] = (Unit_Base)chars[chars.Count-1];
        chars[chars.Count - 1].playerNumb = chars.Count - 1;
        chars[chars.Count - 1].indexPos = pos;
    }

    // Update is called once per frame
    void Update()
    {
        chars[charTurn].Turn();
        if (!gameDone) { return; }

        if (Input.GetKey(KeyCode.Return))
        {
            //Load title
            SceneManager.LoadScene(0);
        }

    }

    //general mov
    public bool RequestPosUpdate(Unit_Base mover, Vector2Int move)
    {
        Vector2Int end = mover.indexPos + move;
        if (end.x > gameBoard.GetLength(0)-1 || end.y > gameBoard.GetLength(1)-1 || end.x < 0 || end.y < 0) { return false; }
        if (gameBoard[end.x, end.y] == null)
        {
            //We're good
            MoveObj(mover, end);
            return true;
        }      
        return false;
    }

    //bullet mov
    public bool RequestPosUpdate(Unit_Bullet mover, Vector2Int move)
    {
        Vector2Int end = mover.indexPos + move;
        if (end.x > gameBoard.GetLength(0) - 1 || end.y > gameBoard.GetLength(1) - 1 || end.x < 0 || end.y < 0) { mover.myRend.sprite = Data.instance.artData.XplodeWall; return false; }
        //If empty go ahead
        if (gameBoard[end.x, end.y] == null)
        {
            //We're good
            MoveObj(mover, end);
            return true;
        }

        //Check specifics
        if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Block)
        {
            mover.myRend.sprite = Data.instance.artData.XplodeWall;
            return false;

        }
        else if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Reflector)
        {
            Vector2Int result = gameBoard[end.x, end.y].GetComponent<Unit_Reflector>().ExplodeOrBounce(mover.indexPos);
            if (result == Vector2Int.zero)
            {
                //explode 
                mover.myRend.sprite = Data.instance.artData.XplodeWall;
                return false;
            }
            else
            {
                //Reflect
                mover.bouncin = true;
                mover.direction = result;
                mover.myRend.sprite = Data.instance.artData.bounce;
                mover.myTrans.rotation = Quaternion.Euler(0, 0, mover.myTrans.rotation.eulerAngles.z + 90);
                MoveObj(mover, end);
                return true;

            }
        }
        else
        if(gameBoard[end.x, end.y].type == Unit_Base.UnitType.Bullet)
        {
            //mid air collision
            //Set sprite
            mover.myRend.sprite = Data.instance.artData.XplodeCenter;
            //Delet collided bullet
            bullets.Remove(gameBoard[end.x, end.y].GetComponent<Unit_Bullet>());
            Destroy(gameBoard[end.x, end.y].gameObject);
            gameBoard[end.x, end.y] = null;
            //move bullet to collision zone
            MoveObj(mover, end);
            return false;
        }
        else
        {
            MoveObj(mover, end);
            mover.myRend.sprite = Data.instance.artData.XplodeCenter;
        }

        if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Character)
        {
            if (!gameBoard[end.x, end.y].GetComponent<Unit_Character>().dead)
            {

                KillChar(gameBoard[end.x, end.y].GetComponent<Unit_Character>());
                return true;
            }
        }


        return false;
    }

    //character shooting
    public bool ShootUpdate(Unit_Base mover, Vector2Int move)
    {
        Vector2Int end = mover.indexPos + move;
        float rot = 0;
        if (move.x == 0) { if (move.y == -1) { rot = 180; } } else { rot = -move.x * 90f; }
        if (end.x > gameBoard.GetLength(0) - 1 || end.y > gameBoard.GetLength(1) - 1 || end.x < 0 || end.y < 0) { return false; }
        
        if (gameBoard[end.x, end.y] == null)
        {
            //We're good
           bullets.Add(Instantiate(Data.instance.bulletObj, BoardToWorld(end,-6f), Quaternion.Euler(0, 0, rot)).GetComponent<Unit_Bullet>());
           bullets[bullets.Count - 1].direction = move;
           bullets[bullets.Count - 1].indexPos = end;
           gameBoard[end.x, end.y] = bullets[bullets.Count - 1];
           if (!CanMove(mover.indexPos)) { KillChar(mover.GetComponent<Unit_Character>()); }
            return true;
        }

        if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Reflector)
        {
            Vector2Int inty = gameBoard[end.x, end.y].GetComponent<Unit_Reflector>().ExplodeOrBounce(mover.indexPos);
            if (inty == Vector2Int.zero)
            {
                //Exploded so no
                return false;
            }
            else
            {
                bullets.Add(Instantiate(Data.instance.bulletObj, BoardToWorld(end, -6f), Quaternion.Euler(0, 0, rot)).GetComponent<Unit_Bullet>());
                bullets[bullets.Count - 1].direction = inty;
                bullets[bullets.Count - 1].indexPos = end;
                bullets[bullets.Count - 1].myRend.sprite = Data.instance.artData.bounce;
                bullets[bullets.Count - 1].bouncin = true;
                return true;
            }
        }

        if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Bullet)
        {
            gameBoard[end.x, end.y].GetComponent<Unit_Bullet>().xploded = true;
            gameBoard[end.x, end.y].myRend.sprite = Data.instance.artData.XplodeCenter;
            return true;
        }

        if (gameBoard[end.x, end.y].type == Unit_Base.UnitType.Character)
        {
            if (!gameBoard[end.x, end.y].GetComponent<Unit_Character>().dead) {
             KillChar(gameBoard[end.x, end.y].GetComponent<Unit_Character>());
            return true;
            }
        }

        return false; 
       
    }

    void MoveObj(Unit_Base mover, Vector2Int end)
    {
        //Update grid

        //Check where we're going
        if (gameBoard[end.x, end.y] == null)
        { 
            gameBoard[end.x, end.y] = mover;

        }//else if (gameBoard[end.x, end.y].type != Unit_Base.UnitType.Reflector) { Debug.LogError("ERROR ERROR @ " + end + " location not null but movement occuring"); }

        //check where we are
        if (gameBoard[mover.indexPos.x, mover.indexPos.y] == mover)
        {
            gameBoard[mover.indexPos.x, mover.indexPos.y] = null;
        }
            //Update Unit
            mover.Move(BoardToWorld(end,mover.myTrans.position.z), end);

    }

    public void NextTurn()
    {
        charTurn++;
        if (charTurn >= chars.Count)
        {
            List<int> temp = new List<int>();
 
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].Turn()) {temp.Add(i); }
            }
 
            for(int z = temp.Count-1; z >= 0; z--)
            {
                Destroy(bullets[temp[z]].gameObject);
                bullets.RemoveAt(temp[z]);
            }

            foreach (Unit_Character character in chars)
            {
               character.shot = gameDone;
            }
            charTurn = 0;
        }
        if (!CanMove(chars[charTurn].indexPos)) { KillChar(chars[charTurn]); }
        background.color = Data.instance.artData.characterColour[chars[charTurn].playerNumb];
        MSTextHandler(false);
    }

    bool CanMove( Vector2Int pos)
    {
        
        if (pos.x + 1 < gameBoard.GetLength(0))
        {
            if (gameBoard[pos.x + 1, pos.y] == null){ return true; }
        }

        if (pos.y + 1 < gameBoard.GetLength(1))
        {
            if (gameBoard[pos.x, pos.y+1] == null) { return true; }
        }

        if (pos.x - 1 >=0)
        {
            if (gameBoard[pos.x-1, pos.y] == null) { return true; }
        }

        if (pos.y - 1 >= 0)
        {
            if (gameBoard[pos.x, pos.y-1] == null) { return true; }
        }

        return false;
    }

    void KillChar(Unit_Character character)
    {
        chars.Remove(character.Die());
        if (charTurn > chars.Count-1) { charTurn = 0; }
        if (chars.Count == 1) { GameEnd(); }
    }

    void GameEnd()
    {
        gameDone = true;
        winTxt.gameObject.SetActive(true);
        msTxt.text = "Press Enter to leave";
        winTxt.text = "Winner is Player " + (chars[0].playerNumb+1);
    }

    public void MSTextHandler(bool move)
    {
        if (gameDone) { return; }
        msTxt.text = move ? "MOVE" : "SHOOT";
    }

    Vector3 BoardToWorld(Vector2Int index,float zPos)
    {
        float xS = -(gameBoard.GetLength(1) / 2f);
        float yS = -(gameBoard.GetLength(0) / 2f);
        Vector2 start = new Vector2(yS * BoardMaker.tileSize, xS * BoardMaker.tileSize);
        Vector3 tempy = new Vector3(start.x + ((BoardMaker.tileSize * index.x) ), start.y + ((BoardMaker.tileSize * index.y)), zPos);
        return tempy;
    }
}
