using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LC_Abduction1 : MonoBehaviour
{
    [Header("Settings")]
    public string unitTag = "Enemy";
    public float matchDuration = 60f; // seconds

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public TMP_Text remainingText;
    public TMP_Text timerText;
    public Button restartButton;

    private float timeLeft;
    private bool matchOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartScene);

        timeLeft = matchDuration;
    }

    void Update()
    {
        if (matchOver) return;

        // Count units
        int count = GameObject.FindGameObjectsWithTag(unitTag).Length;
        remainingText.text = "Civilians Left: " + count;

        // Timer countdown
        timeLeft -= Time.deltaTime;
        timerText.text = "Time Left: " + Mathf.CeilToInt(timeLeft);

        // Lose condition (all units gone)
        if (count == 0)
        {
            EndMatch("DEFEAT");
        }
        // Win condition (time ran out, units still exist)
        else if (timeLeft <= 0f)
        {
            EndMatch("SUCCESS!!");
        }
    }

    void EndMatch(string message)
    {
        matchOver = true;

        gameOverPanel.SetActive(true);
        gameOverText.text = message;

        Time.timeScale = 0f;
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
