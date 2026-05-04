using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FlappyCheckers : MonoBehaviour
{

    // Game Objects
    public GameObject Bike;
    public GameObject LightStops;
    private GameObject LightHolder;

    // Background
    public Renderer Background;
    public Material normalGravity;
    public Material LowGravity;
    public Material HighGravity;

    // Bike
    public float SpeedVertically;
    public float Jump = 5f;
    public float Gravity = 20f;

    // Light Stops
    public float LightStopSpawn;
    public float LightIntervals = 2f;
    public int LightCount;
    public float Speed = 2f;

    // Gravity
    float gravityTimer;
    public float gravityInterval = 10f;
    enum GravityMode
    {
        Normal, Low, High
    }
    private GravityMode currentGravity;

    // Score
    public Text ScoreText;
    public int Score;

    // Game Over UI
    public GameObject GameOverPanel;
    public Text FinalScoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Score = 0;
        ScoreText.text = "SCORE: " + Score.ToString();

        LightCount = 0;

        if (LightHolder != null)
            Destroy(LightHolder);

        LightHolder = new GameObject("LightHolder");
        LightHolder.transform.parent = this.transform;

        SpeedVertically = 0f;
        LightStopSpawn = 0f;

        ApplyGravity(GravityMode.Normal);
    }

    // Update is called once per frame
    void Update()
    {
        SpeedVertically += -Gravity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpeedVertically = 0;
            SpeedVertically += Jump;
        }

        Bike.transform.position += Vector3.up * SpeedVertically * Time.deltaTime;

        gravityTimer += Time.deltaTime;

        if (gravityTimer >= gravityInterval)
        {
            ChangeGravityMode();
            gravityTimer = 0f;
        }

        LightStopSpawn -= Time.deltaTime;

        if (LightStopSpawn <= 0)
        {
            LightStopSpawn = LightIntervals;

            GameObject pipe = Instantiate(LightStops);
            pipe.transform.parent = LightHolder.transform;
            pipe.transform.name = LightCount++.ToString();

            pipe.transform.position += Vector3.right * 12;
            pipe.transform.position += Vector3.up * Mathf.Lerp(4, 9, Random.value);
        }

        LightHolder.transform.position += Speed * Time.deltaTime * Vector3.left;

        float speedToRange = Mathf.InverseLerp(-10, 10, SpeedVertically);
        float noseAngle = Mathf.Lerp(-30, 30, speedToRange);
        Bike.transform.rotation = Quaternion.Euler(0, 90, noseAngle);

        foreach (Transform pipe in LightHolder.transform)
        {
            if (pipe.position.x < 0)
            {
                int pipeId = int.Parse(pipe.name);
                if (pipeId > Score)
                {
                    Score = pipeId;
                    ScoreText.text = "SCORE: " + Score.ToString();
                }
            }

            if (pipe.position.x < -12)
            {
                Destroy(pipe.gameObject);
            }
        }
    }

    void ChangeGravityMode()
    {
        int mode = Random.Range(0, 3);

        switch (mode)
        {
            case 0:
                ApplyGravity(GravityMode.Normal);
                break;
            case 1:
                ApplyGravity(GravityMode.Low);
                break;
            case 2:
                ApplyGravity(GravityMode.High);
                break;
        }
    }

    void ApplyGravity(GravityMode mode)
    {
        currentGravity = mode;

        switch (mode)
        {
            case GravityMode.Normal:
                Gravity = 20f;
                Jump = 5f;
                Background.material = normalGravity;
                break;

            case GravityMode.Low:
                Gravity = 10f;
                Jump = 4f;
                Background.material = LowGravity;
                break;

            case GravityMode.High:
                Gravity = 45f;
                Jump = 10f;
                Background.material = HighGravity;
                break;
        }
    }

    void RestGame()
    {
        Score = 0;
        ScoreText.text = "SCORE: " + Score.ToString();
        LightCount = 0;

        Destroy(LightHolder);
        LightHolder = new GameObject("Holder");
        LightHolder.transform.parent = this.transform;

        SpeedVertically = 0f;
        Bike.transform.position = Vector3.up * 5;

        LightStopSpawn = 0f;

        ApplyGravity(GravityMode.Normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameOver(Score);
    }

    public void GameOver(int finalScore)
    {
        Time.timeScale = 0f;
        if (GameOverPanel != null) GameOverPanel.SetActive(true);
        if (FinalScoreText != null) FinalScoreText.text = "Final Score: " + finalScore.ToString();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        if (GameOverPanel != null) GameOverPanel.SetActive(false);
        RestGame();
    }

    public void SubmitScore()
    {
        string bubbleIOUrl = "https://gravity-checkers-prom.bubbleapps.io/version-test/index?score=" + Score;
        Application.OpenURL(bubbleIOUrl);
    }

}
