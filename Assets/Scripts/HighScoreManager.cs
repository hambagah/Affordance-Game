using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public float totalTime; // Time in seconds
    }

    public Transform leaderboardContent; // Assign the LeaderboardContent object
    public GameObject leaderboardEntryTemplate; // Assign the template
    public int maxEntries = 10; // Maximum number of leaderboard entries

    private List<ScoreEntry> highScores = new List<ScoreEntry>();

    private void Start()
    {
        // Load scores from PlayerPrefs
        LoadScores();
        UpdateLeaderboard();
    }

    public void AddNewScore(string playerName, float totalTime)
    {
        // Add new score
        highScores.Add(new ScoreEntry { playerName = playerName, totalTime = totalTime });

        // Sort by total time (ascending for best times)
        highScores.Sort((a, b) => a.totalTime.CompareTo(b.totalTime));

        // Trim to max entries
        if (highScores.Count > maxEntries)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        // Save and update leaderboard
        SaveScores();
        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Populate leaderboard
        for (int i = 0; i < highScores.Count; i++)
        {
            var entry = Instantiate(leaderboardEntryTemplate, leaderboardContent);
            entry.SetActive(true);

            Text entryText = entry.GetComponent<Text>();
            string formattedTime = FormatTime(highScores[i].totalTime);

            entryText.text = $"{i + 1}. {highScores[i].playerName} - Time: {formattedTime}";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void SaveScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetString($"HighScore_{i}_Name", highScores[i].playerName);
            PlayerPrefs.SetFloat($"HighScore_{i}_Time", highScores[i].totalTime);
        }

        PlayerPrefs.SetInt("HighScoreCount", highScores.Count);
    }

    private void LoadScores()
    {
        highScores.Clear();

        int count = PlayerPrefs.GetInt("HighScoreCount", 0);
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"HighScore_{i}_Name", "Player");
            float time = PlayerPrefs.GetFloat($"HighScore_{i}_Time", 0f);

            highScores.Add(new ScoreEntry { playerName = name, totalTime = time });
        }
    }
}
