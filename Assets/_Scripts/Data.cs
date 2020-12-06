using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data instance;
    public List<PlayInputs> playInputs;
    public ArtData artData;
    public LevelData levelsData;
    public GameObject tileObj, mushroomObj, bulletObj, reflectorObj;

    public Data_Board currBoard;

    void Awake()
    {

        if (instance != null)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
}

[System.Serializable]
public struct PlayInputs
{
    public List<KeyCode> inputs;
}
