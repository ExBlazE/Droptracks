using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Vector3 originalGravity;
    [SerializeField] float gravityMultiplier;
    
    [Space]

    [SerializeField] GameObject aimObject;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] GameObject ballPrefab;

    private int aimTrack;
    private float aimMoveTime = 0.05f;
    private bool aimIsMoving = false;

    private float spawnPosZ = -17.0f;
    private float[] spawnPosX = { 3, 0, -3 };

    [System.NonSerialized] public bool[] targetInTrack = { false, false, false };
    [System.NonSerialized] public bool[] ballInTrack = { false, false, false };

    [Space]

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;

    private Color color1 = new Color(1, 0.375f, 0);
    private Color color2 = new Color(0, 0.375f, 1);
    private float scoreBlinkTime = 0.5f;

    private int score;
    private float timeLeft = 10;
    private bool isGameActive = false;
    private bool isGamePaused = false;

    private bool isNewBestScore = false;
    private bool isNameGiven = false;

    [Space]

    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] TMP_InputField nameBox;

    [Space]

    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject volumeBox;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;

    [Space]

    [SerializeField] AudioSource musicSource;
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        originalGravity = Physics.gravity;
        Time.timeScale = 0;

        ShowBestScore();
    }

    public void StartGame()
    {
        Physics.gravity *= gravityMultiplier;
        Time.timeScale = 1;

        titleScreen.SetActive(false);
        volumeBox.SetActive(false);
        scoreText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);

        score = 0;
        AddScore(0);

        isGameActive = true;

        StartCoroutine(StartTimer());
        StartCoroutine(SpawnTargets());

        aimTrack = spawnPosX.Length / 2;
        aimObject.transform.position = new Vector3(spawnPosX[aimTrack], aimObject.transform.position.y, aimObject.transform.position.z);
    }

    void Update()
    {
        musicSource.volume = volumeSlider.value;

        if (!isGameActive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();

        if (isGamePaused)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !aimIsMoving)
        {
            aimTrack--;
            if (aimTrack < 0)
                aimTrack = spawnPosX.Length - 1;
            StartCoroutine(MoveAim());
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !aimIsMoving)
        {
            aimTrack++;
            if (aimTrack >= spawnPosX.Length)
                aimTrack = 0;
            StartCoroutine(MoveAim());
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SpawnBall();
    }

    IEnumerator SpawnTargets()
    {
        while (isGameActive)
        {
            for (int i = 0; i < spawnPosX.Length; i++)
            {
                if (!targetInTrack[i])
                {
                    Vector3 spawnPos = new Vector3(spawnPosX[i], 1, spawnPosZ);
                    GameObject newTarget = Instantiate(targetPrefab, spawnPos, targetPrefab.transform.rotation);
                    Target targetScript = newTarget.GetComponent<Target>();
                    targetScript.trackNumber = i;
                    targetInTrack[i] = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator MoveAim()
    {
        aimIsMoving = true;
        
        Vector3 startPos = aimObject.transform.position;
        Vector3 endPos = new Vector3(spawnPosX[aimTrack], aimObject.transform.position.y, aimObject.transform.position.z);

        float currentTime = 0;
        float fractionOfMove;

        while (currentTime < aimMoveTime)
        {
            currentTime += Time.deltaTime;
            fractionOfMove = currentTime / aimMoveTime;
            if (fractionOfMove > 1)
                fractionOfMove = 1;
            aimObject.transform.position = Vector3.Lerp(startPos, endPos, fractionOfMove);
            yield return null;
        }

        aimIsMoving = false;
    }

    IEnumerator StartTimer()
    {
        while (isGameActive)
        {
            timeLeft -= Time.deltaTime;
            timeText.SetText("Time: " + Mathf.Ceil(timeLeft));

            if (timeLeft <= 0)
                GameOver();

            yield return null;
        }
    }

    IEnumerator BlinkScoreCoroutine()
    {
        float currentTime = 0;

        while (currentTime < scoreBlinkTime)
        {
            currentTime += Time.deltaTime;
            float fractionOfBlink = currentTime / scoreBlinkTime;
            if (fractionOfBlink > 1)
                fractionOfBlink = 1;
            Color lerpedColor = Color.Lerp(color2, color1, fractionOfBlink);
            scoreText.color = lerpedColor;
            yield return null;
        }

        currentTime = 0;

        while (currentTime < scoreBlinkTime)
        {
            currentTime += Time.deltaTime;
            float fractionOfBlink = currentTime / scoreBlinkTime;
            if (fractionOfBlink > 1)
                fractionOfBlink = 1;
            Color lerpedColor = Color.Lerp(color1, color2, fractionOfBlink);
            scoreText.color = lerpedColor;
            yield return null;
        }
    }

    public void BlinkScore()
    {
        StopCoroutine(BlinkScoreCoroutine());
        StartCoroutine(BlinkScoreCoroutine());
    }

    public void AddScore(int num)
    {
        score += num;
        scoreText.SetText("Score Count: " + score);
    }

    void SpawnBall()
    {
        if (!ballInTrack[aimTrack])
        {
            Vector3 aimPos = aimObject.transform.position;
            if (aimIsMoving)
                aimPos = new Vector3(spawnPosX[aimTrack], aimObject.transform.position.y, aimObject.transform.position.z);
            GameObject newBall = Instantiate(ballPrefab, aimPos, ballPrefab.transform.rotation);
            BallBehaviour ballScript = newBall.GetComponent<BallBehaviour>();
            ballScript.trackNumber = aimTrack;
            ballInTrack[aimTrack] = true;
        }
    }

    public void PauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;

            pauseScreen.SetActive(true);
            volumeBox.SetActive(true);
        }
        else if (isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;

            pauseScreen.SetActive(false);
            volumeBox.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Physics.gravity = originalGravity;

        if (isNewBestScore)
        {
            ScoreManager.instance.SetBestScore(score);
            if (!isNameGiven)
                ScoreManager.instance.SetScoreName(string.Empty);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GameOver()
    {
        isGameActive = false;

        finalScoreText.SetText("Score: " + score);
        isNewBestScore = ScoreManager.instance.CheckBestScore(score);
        
        scoreText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);

        gameOverScreen.SetActive(true);
    }

    void ShowBestScore()
    {
        int bestScore = ScoreManager.instance.GetBestScore();
        string bestScoreName = " - " + ScoreManager.instance.GetScoreName();

        if (bestScoreName.Length <= 3)
            bestScoreName = string.Empty;

        bestScoreText.SetText("Best Score: " + bestScore + bestScoreName);
    }

    public void GetScoreName()
    {
        if (isNewBestScore)
        {
            isNameGiven = true;
            ScoreManager.instance.SetBestScore(score);
            ScoreManager.instance.SetScoreName(nameBox.text);
            ShowBestScore();
        }
    }

    public bool GetGameActive()
    {
        return isGameActive;
    }
}
