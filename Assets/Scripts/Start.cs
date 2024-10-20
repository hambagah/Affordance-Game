using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void StartGame(string stageToPlay) 
    {
        SceneManager.LoadScene(stageToPlay, LoadSceneMode.Single);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
