using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MouseDragController dragController;

    [SerializeField] private GameObject inGame;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Image imgSound;
    [SerializeField] private Sprite spriteSoundOn;
    [SerializeField] private Sprite spriteSoundOff;
    private bool isSoundOn = true;

    [SerializeField] private float limitTimer;
    [SerializeField] private Image imgTimeGuage;
    [SerializeField] private TMP_Text text_Timer;

    [SerializeField] private TMP_Text text_Score;

    public bool isGameStart { get; private set; }

    private int score;
    public int Score
    {
        get { return score; }
        private set
        {
            score = value;
            text_Score.text = $"점수: {score}";
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
    }

    public void GameStart()
    {
        isGameStart = true;

        mainMenu.SetActive(false);
        inGame.SetActive(true);
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
        isGameStart = false;
        dragController.Init();
        text_Score.text = $"게임 종료";
        StopAllCoroutines();
        audioSource.Stop();
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
            text_Timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

            yield return null;
        }

        if(timer <= 0)
        {
            GameOver();
            text_Timer.text = $"0:00";
        }
    }
}
