using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneNavigator : MonoBehaviour
{
    public GameObject[] cutsceneImages; // Array of images to display
    private int currentIndex = 0;       // Index of the current image

    void Start()
    {
        // Ensure only the first image is active
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            cutsceneImages[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        // On left mouse click, move to the next image
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            ShowNextImage();
        }
    }

    void ShowNextImage()
    {
        if (currentIndex < cutsceneImages.Length)
        {
            // Hide the current image
            cutsceneImages[currentIndex].SetActive(false);

            // Move to the next image
            currentIndex++;

            // Show the next image if it exists
            if (currentIndex < cutsceneImages.Length)
            {
                cutsceneImages[currentIndex].SetActive(true);
            }
            else
            {
                EndCutscene(); // Cutscene finished
            }
        }
    }

    void EndCutscene()
    {
        Debug.Log("Cutscene finished! Loading Level1...");
        SceneManager.LoadScene("Menu"); // Replace "Level1" with the exact name of your scene
    }
}

