using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "newLevel", menuName = "Data/Level", order = 1)]
public class Data_Board : ScriptableObject
{
   public BoardMaker.BoardType boardType;
   public Vector2Int boardSize;
   public float secondaryFloat;
   public List<ReflectorZones> refZones = new List<ReflectorZones>();
   public List<Vector2Int> playerPositions = new List<Vector2Int>();
}
[System.Serializable]
public struct ReflectorZones
{
    public Vector2Int pos;
    public float rot;
}