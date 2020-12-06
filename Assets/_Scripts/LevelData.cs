using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LevelsData", menuName = "Data/LevelData", order = 2)]
public class LevelData : ScriptableObject
{
    public List<Data_Board> levels;
}
