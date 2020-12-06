using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleHandler : MonoBehaviour
{
    [SerializeField] GameObject playHide;

    public void PlayHide()
    {
        playHide.SetActive(!playHide.activeInHierarchy);
    }


    public void LoadLevel(int lvl)
    {
        Data.instance.currBoard = Data.instance.levelsData.levels[lvl];
        SceneManager.LoadScene(1);
    }

}
