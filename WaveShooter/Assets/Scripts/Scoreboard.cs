using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scoreboard : MonoBehaviour {

    public GameObject scoreObject;
    public GameObject highscoreObject;
    public GameObject longshotObject;

    public GameObject waveCountObject;
    public GameObject enemiesCountObject;

    public static Scoreboard instance;

    public bool ResetScoreboardOnLoad = false;

    TextMeshProUGUI score;
    TextMeshProUGUI highscore;
    TextMeshProUGUI longshot;
    TextMeshProUGUI waves;
    TextMeshProUGUI enemies;

    int currentScore = 0;

    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (ResetScoreboardOnLoad)
        {
            ResetScoreboard();
        }
    }

    private void Start()
    {
        score = scoreObject.GetComponent<TextMeshProUGUI>();
        waves = waveCountObject.GetComponent<TextMeshProUGUI>();
        enemies = enemiesCountObject.GetComponent<TextMeshProUGUI>();
        highscore = highscoreObject.GetComponent<TextMeshProUGUI>();
        longshot = longshotObject.GetComponent<TextMeshProUGUI>();

        if (score != null)
        {
            score.SetText(currentScore.ToString());
        }
        if (highscore != null)
        {
            highscore.SetText(PlayerPrefs.GetInt("Highscore", 0).ToString());
        }
        if (longshot != null)
        {
            longshot.SetText(PlayerPrefs.GetInt("Longshot", 0).ToString()+"m");
        }
    }

    public void UpdateScore(int points)
    {
        currentScore += points;
        score.SetText(Mathf.Abs(currentScore).ToString());
        if (currentScore > PlayerPrefs.GetInt("Highscore", 0))
        {
            PlayerPrefs.SetInt("Highscore", currentScore);
            highscore.SetText(Mathf.Abs(currentScore).ToString());
        }
    }

    public void UpdateLongshot(float distance)
    {
        int d = Mathf.RoundToInt(distance);
        if (d > PlayerPrefs.GetInt("Longshot", 0))
        {
            PlayerPrefs.SetInt("Longshot", d);
            longshot.SetText(PlayerPrefs.GetInt("Longshot", 0).ToString() + "m");
        }
    }

    public void ResetScoreboard()
    {
        PlayerPrefs.SetInt("Longshot", 0);
        PlayerPrefs.SetInt("Highscore", 0);
    }

    public void UpdateWaveInfo(int currentWave, int enemiesCount)
    {
        if (waves != null && enemies != null)
        {
            waves.SetText(currentWave.ToString());
            enemies.SetText(enemiesCount.ToString());
        }
    }

}
