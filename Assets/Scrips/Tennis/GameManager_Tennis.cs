using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager_Tennis : MonoBehaviour {

    [SerializeField] private GameObject ballPrefab;

    [Space(1)]
    [HideInInspector] public static int redScore = 0;
    [HideInInspector] public static int blueScore = 0;

    [Space(1)]
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text blueScoreText;

    [Space(1)]
    [SerializeField] private Transform redBallSpawnPoint;
    [SerializeField] private Transform blueBallSpawnPoint;

    [Space(1)]
    [SerializeField] private GameObject redPlayerWinnerScreen;
    [SerializeField] private GameObject bluePlayerWinnerScreen;

    [Space(1)]
    [SerializeField] private GameObject countdownBG;
    [SerializeField] private TMP_Text countdownText;


    private void Awake() {
        Ball_Tennis.onPonitScored += OnPointScored;
    }

    private void OnDisable() {
        Ball_Tennis.onPonitScored -= OnPointScored;
    }

    private void Start() {
        GameStartRandomTurn();
        Utils.ChangeToLandScape();
        redScore = 0;
        blueScore = 0;
        redPlayerWinnerScreen.SetActive(false);
        bluePlayerWinnerScreen.SetActive(false);

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        Time.timeScale = 0;
        AudioManager.instance.PlaySound("Countdown");
        countdownBG.SetActive(true);
        countdownText.gameObject.SetActive(true);

        AudioManager.instance.PlaySound("Countdown");
        for(int i = 3; i > 0; i--) {
            countdownText.text = i.ToString();

            countdownText.transform.localScale = Vector3.one * 1.2f;
            countdownText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);

            yield return new WaitForSecondsRealtime(.75f);
        }
        countdownText.gameObject.SetActive(false);
        countdownBG.SetActive(false);

        Time.timeScale = 1;
    }


    private void GameStartRandomTurn() {
        bool redTurn = Random.value > .5f;

        if(redTurn) {
            Instantiate(ballPrefab, redBallSpawnPoint.position, Quaternion.identity);
        } else {
            Instantiate(ballPrefab, blueBallSpawnPoint.position, Quaternion.identity);
        }
    }


    private void OnPointScored(PlayerMovements_Tennis.PlayerSide pointSide) {
        if(pointSide == PlayerMovements_Tennis.PlayerSide.Red) {
            redScore++;
            redScoreText.text = redScore.ToString();
            if(redScore == 3) {
                redPlayerWinnerScreen.SetActive(true);
                Manager_MainMenu.redMainPoints++;
                Eyes_Tennis.ball = redPlayerWinnerScreen;
                StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                return;
            }
        }else {
            blueScore++;
            blueScoreText.text = blueScore.ToString();
            if(blueScore == 3) {
                bluePlayerWinnerScreen.SetActive(true);
                Manager_MainMenu.blueMainPoints++;
                Eyes_Tennis.ball = bluePlayerWinnerScreen;
                StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                return;
            }
        }

        ResetRound(pointSide);
    }


    private void ResetRound(PlayerMovements_Tennis.PlayerSide pointSide) {
        if(pointSide == PlayerMovements_Tennis.PlayerSide.Red) {
            Instantiate(ballPrefab, blueBallSpawnPoint.position, Quaternion.identity);
        } else { 
            Instantiate(ballPrefab, redBallSpawnPoint.position, Quaternion.identity);
        }
    }


}