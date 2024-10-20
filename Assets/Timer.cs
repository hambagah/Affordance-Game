using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

    public TMP_Text timerText;
    bool finished = false;
    string seconds;
    private float t;
    public float stageTime = 90f;
    public GameOver GameOverScreen;

    void Start () {
        t = 0f;
        Time.timeScale = 1;
        GameOverScreen.Off();
    }
   
    // Update is called once per frame
    void Update () {
        t += Time.deltaTime;
        if (!finished)
        {
            seconds = (t % 60).ToString("f0");

            timerText.text = seconds;
        }
        if (t >= stageTime)
        {
            GameOver();
            finished = true;
        }
    }

    public void GameOver() {
        GameOverScreen.Setup(1);
    }
}