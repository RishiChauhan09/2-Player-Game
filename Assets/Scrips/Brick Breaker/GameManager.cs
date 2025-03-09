using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int BlueLives = 4;
    public int RedLives = 4;

    public GameObject BlueWinnerScreen;
    public GameObject RedWinnerScreen;

    public GameObject ball1Prefab;
    public Transform ball1SpawnPoint;
    public Vector2 ball1InitialDirection = new Vector2(0.5f, 1f);

    public GameObject ball2Prefab;
    public Transform ball2SpawnPoint;
    public Vector2 ball2InitialDirection = new Vector2(-0.5f, 1f);

    public float respawnDelay = 1f;

    private List<GameObject> activeBalls = new List<GameObject>();

    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text blueScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (BlueWinnerScreen != null) BlueWinnerScreen.SetActive(false);
        if (RedWinnerScreen != null) RedWinnerScreen.SetActive(false);
    }

    private void Start()
    {
        redScore.text = RedLives.ToString();
        blueScore.text = BlueLives.ToString();
        SpawnAllBalls();
    }

    public void BallLost(Player_Movement.PlayerSide playerSide, GameObject ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
        }

        if (playerSide == Player_Movement.PlayerSide.Blue)
        {
            BlueLives--;
            blueScore.text = BlueLives.ToString();
        }
        else
        {
            RedLives--;
            redScore.text = RedLives.ToString();
        }

        if (BlueLives <= 0)
        {
            PlayerWins(Player_Movement.PlayerSide.Red);
        }
        else if (RedLives <= 0)
        {
            PlayerWins(Player_Movement.PlayerSide.Blue);
        }
        else if (activeBalls.Count == 0)
        {
            Invoke("SpawnAllBalls", respawnDelay);
        }
    }

    public void SpawnAllBalls()
    {
        activeBalls.Clear();

        if (ball1Prefab != null)
        {
            Vector3 spawnPos = (ball1SpawnPoint != null) ? ball1SpawnPoint.position : new Vector3(-1f, 0, 0);
            GameObject newBall1 = Instantiate(ball1Prefab, spawnPos, Quaternion.identity);

            Ball_Behaviour ball1Behavior = newBall1.GetComponent<Ball_Behaviour>();
            if (ball1Behavior != null)
            {
                ball1Behavior.initialDirection = ball1InitialDirection.normalized;
            }

            activeBalls.Add(newBall1);
        }
        else
        {
            Debug.LogError("Ball 1 prefab not set in GameManager!");
        }

        if (ball2Prefab != null)
        {
            Vector3 spawnPos = (ball2SpawnPoint != null) ? ball2SpawnPoint.position : new Vector3(1f, 0, 0);
            GameObject newBall2 = Instantiate(ball2Prefab, spawnPos, Quaternion.identity);

            Ball_Behaviour ball2Behavior = newBall2.GetComponent<Ball_Behaviour>();
            if (ball2Behavior != null)
            {
                ball2Behavior.initialDirection = ball2InitialDirection.normalized;
            }

            activeBalls.Add(newBall2);
        }
        else
        {
            Debug.LogError("Ball 2 prefab not set in GameManager!");
        }

    }

    public void PlayerWins(Player_Movement.PlayerSide winner)
    {

        foreach (GameObject ball in activeBalls)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
        }
        activeBalls.Clear();

        if (winner == Player_Movement.PlayerSide.Blue)
        {
            if (BlueWinnerScreen != null) BlueWinnerScreen.SetActive(true);
            Manager_MainMenu.blueMainPoints++;
        }
        else
        {
            if (RedWinnerScreen != null) RedWinnerScreen.SetActive(true);
            Manager_MainMenu.redMainPoints++;
        }

        AudioManager.instance.PlaySound("Finish");
        StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
    }
}