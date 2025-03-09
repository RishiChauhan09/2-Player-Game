using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager_KnifeThrower : MonoBehaviour
{
    [SerializeField] private GameObject[] redKnives;
    [SerializeField] private GameObject[] blueKnives;
    public static int noRedKnifes = 3;
    public static int noBlueKnifes = 3;
    [SerializeField] private Transform redSpawnPoint;
    [SerializeField] private Transform blueSpawnPoint;
    [SerializeField] private GameObject redKnifePrefab;
    [SerializeField] private GameObject blueKnifePrefab;
    [SerializeField] private GameObject BlueWinnerScreen;
    [SerializeField] private GameObject RedWinnerScreen;

    private int redKnivesHit = 0;
    private int blueKnivesHit = 0;

    private bool initialKnivesSpawned = false;

    private void Awake()
    {
        TouchSence_KnifeThrower.onPlayerTouchScreen += OnPlayerTouchScreen;
        InitializeGame();
    }

    private void Start()
    {
        SpawnInitialKnives();
    }

    private void OnDestroy()
    {
        TouchSence_KnifeThrower.onPlayerTouchScreen -= OnPlayerTouchScreen;
    }

    private void InitializeGame()
    {
        noRedKnifes = 3;
        noBlueKnifes = 3;

        redKnivesHit = 0;
        blueKnivesHit = 0;

        UpdateKnivesUI();

        initialKnivesSpawned = false;
    }

    private void SpawnInitialKnives()
    {
        if (initialKnivesSpawned)
        {
            Debug.LogWarning("Attempted to spawn initial knives more than once!");
            return;
        }

        Player_Knife[] existingKnives = FindObjectsOfType<Player_Knife>();
        bool redKnifeExists = false;
        bool blueKnifeExists = false;

        foreach (Player_Knife knife in existingKnives)
        {
            if (knife.Side == Player_Knife.PlayerSide.Red) redKnifeExists = true;
            if (knife.Side == Player_Knife.PlayerSide.Blue) blueKnifeExists = true;
        }

        if (!redKnifeExists && redSpawnPoint != null && redKnifePrefab != null)
        {
            GameObject redKnife = Instantiate(
                redKnifePrefab,
                redSpawnPoint.position,
                Quaternion.Euler(0, 0, 180));

            Player_Knife redKnifeComponent = redKnife.GetComponent<Player_Knife>();
            if (redKnifeComponent != null)
            {
                redKnifeComponent.Side = Player_Knife.PlayerSide.Red;
            }

            Debug.Log("Spawned initial Red knife");
        }

        if (!blueKnifeExists && blueSpawnPoint != null && blueKnifePrefab != null)
        {
            GameObject blueKnife = Instantiate(
                blueKnifePrefab, blueSpawnPoint.position, Quaternion.identity);

            Player_Knife blueKnifeComponent = blueKnife.GetComponent<Player_Knife>();
            if (blueKnifeComponent != null)
            {
                blueKnifeComponent.Side = Player_Knife.PlayerSide.Blue;
            }

            Debug.Log("Spawned initial Blue knife");
        }

        initialKnivesSpawned = true;
    }

    private void UpdateKnivesUI()
    {
        for (int i = 0; i < redKnives.Length; i++)
        {
            if (redKnives[i] != null)
                redKnives[i].SetActive(i < noRedKnifes);
        }

        for (int i = 0; i < blueKnives.Length; i++)
        {
            if (blueKnives[i] != null)
                blueKnives[i].SetActive(i < noBlueKnifes);
        }
    }

    private void OnPlayerTouchScreen(Player_Knife.PlayerSide side)
    {
        UpdateKnivesUI();
        CheckGameOver();
    }

    public void KnifeHitBoard(Player_Knife.PlayerSide side)
    {
        if (side == Player_Knife.PlayerSide.Red)
            redKnivesHit++;
        else
            blueKnivesHit++;

        Debug.Log($"{side} player scored a hit! Total hits: {(side == Player_Knife.PlayerSide.Red ? redKnivesHit : blueKnivesHit)}");
    }

    public void KnifeHitKnife(Player_Knife.PlayerSide side)
    {
        Debug.Log($"{side} knife hit another knife. No points awarded.");
    }

    private void CheckGameOver()
    {
        bool gameOver = (noRedKnifes <= 0 && noBlueKnifes <= 0);
        if (gameOver)
        {
            Debug.Log("Game over condition met. Showing results...");
            ShowGameResults();
        }
    }

    private void ShowGameResults()
    {
        if (redKnivesHit > blueKnivesHit)
        {
            RedWinnerScreen.SetActive(true);
            Manager_MainMenu.blueMainPoints++;
        }
        else if (blueKnivesHit > redKnivesHit)
        {
            BlueWinnerScreen.SetActive(true);
            Manager_MainMenu.redMainPoints++;
        }

        StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
    }
}