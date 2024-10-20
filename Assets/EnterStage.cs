using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterStage : MonoBehaviour
{
    public int levelToChange;

    void OnTriggerEnter2D(Collider2D col)
    {
        SceneManager.LoadScene(levelToChange);
    }
}
