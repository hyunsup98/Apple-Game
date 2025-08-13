using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ScoreData
{

}

public class GameManager : MonoBehaviour
{
    [SerializeField] private MouseDragController dragController;

    [SerializeField] private GameObject inGame;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource gameoverAudioSource;
    [SerializeField] private Image imgSound;
    [SerializeField] private Sprite spriteSoundOn;
    [SerializeField] private Sprite spriteSoundOff;
    private bool isSoundOn = true;

    [SerializeField] private float limitTimer;
    [SerializeField] private Image imgTimeGuage;
    [SerializeField] private TMP_Text text_Timer;

    [SerializeField] private TMP_Text text_Score;

    [SerializeField] private GameObject objCountDown;
    [SerializeField] private TMP_Text text_CountDown;
    [SerializeField] private float countdownTime;

    [SerializeField] private GameObject objResult;
    [SerializeField] private TMP_Text text_Result;

    private const string KEY_TOTALHIGHSCORE = "TotalHighScore";
    private const string KEY_HIGHSCORE = "HighScore";
    private const string KEY_LASTDATE = "LastDate";
    [SerializeField] private TMP_Text text_TotalHighScore;
    [SerializeField] private TMP_Text text_HighScore;

    public bool isGameStart { get; private set; }

    private int score;
    public int Score
    {
        get { return score; }
        private set
        {
            score = value;
            text_Score.text = $"점수: {score} 점";
        }
    }

    private void Awake()
    {
#if PLATFORM_ANDROID
        Application.targetFrameRate = 60;
#endif

        if (inGame.activeSelf == true) inGame.SetActive(false);
        if (mainMenu.activeSelf == false) mainMenu.SetActive(true);

        isGameStart = false;
        Score = 0;

        text_TotalHighScore.text = $"최고점수: {GetTotalHighScoreData().ToString()}점";
        CheckDateData();
    }

    public void GameStart()
    {
        mainMenu.SetActive(false);
        inGame.SetActive(true);

        text_HighScore.text = $"일일 최고점수: {GetScoreData().ToString()} 점";
        StartCoroutine(CountDown());
    }

    public IEnumerator CountDown()
    {
        objCountDown.SetActive(true);
        text_CountDown.gameObject.SetActive(true);
        text_CountDown.text = countdownTime.ToString();

        var timer = countdownTime;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            text_CountDown.text = Mathf.CeilToInt(timer).ToString();

            yield return null;
        }

        text_CountDown.text = $"Start!";

        yield return new WaitForSeconds(1f);

        objCountDown.SetActive(false);
        isGameStart = true;
        audioSource.Play();
        StartCoroutine(TimerCoroutine());
    }

    public void OnSoundOnOff()
    {
        if(isSoundOn)
        {
            isSoundOn = false;
            imgSound.sprite = spriteSoundOff;
            audioSource.mute = true;
        }
        else
        {
            isSoundOn = true;
            imgSound.sprite = spriteSoundOn;
            audioSource.mute = false;
        }
    }

    private void GameOver()
    {
        dragController.Init();
        SaveScoreData(Score);
        objResult.SetActive(true);
        text_Result.text = $"{Score} 점";
        audioSource.Stop();
        gameoverAudioSource.Play();
        StopAllCoroutines();
    }

    public void OnClickQuitResult(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OnClickMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int point)
    {
        Score += point;
    }

    private IEnumerator TimerCoroutine()
    {
        var timer = limitTimer;

        while(timer > 0)
        {
            timer -= Time.deltaTime;

            imgTimeGuage.fillAmount = timer / limitTimer;

            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            minutes = Mathf.Max(minutes, 0);
            seconds = Mathf.Max(seconds, 0);
            text_Timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

            yield return null;
        }

        if(timer <= 0)
        {
            GameOver();
            text_Timer.text = $"0:00";
        }
    }

    private void SaveScoreData(int score)
    {
        int currentHighScore = PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);

        if(score > currentHighScore)
        {
            PlayerPrefs.SetInt(KEY_HIGHSCORE, score);
            PlayerPrefs.Save();
        }

        int totalHighScore = PlayerPrefs.GetInt(KEY_TOTALHIGHSCORE, 0);

        if(score > totalHighScore)
        {
            PlayerPrefs.SetInt(KEY_TOTALHIGHSCORE, score);
            PlayerPrefs.Save();
        }
    }

    private int GetScoreData()
    {
        return PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);
    }

    private int GetTotalHighScoreData()
    {
        return PlayerPrefs.GetInt(KEY_TOTALHIGHSCORE, 0);
    }

    private void CheckDateData()
    {
        string lastDate = PlayerPrefs.GetString(KEY_LASTDATE, string.Empty);
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if(lastDate != today)
        {
            PlayerPrefs.SetInt(KEY_HIGHSCORE, 0);
            PlayerPrefs.SetString(KEY_LASTDATE, today);
            PlayerPrefs.Save();
        }
    }

}
