using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class GameManager_MiniSoccer : MonoBehaviour {

    [HideInInspector] public static int redPoints;
    [HideInInspector] public static int bluePoints;

    [SerializeField] private int goalsToScore;
    [SerializeField] private Canvas canvas;
    [Header("Points")]
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private TMP_Text redScore;

    [Space(2)]
    [Header("Winner Screens")]
    [SerializeField] private GameObject blueWinnerScreen;
    [SerializeField] private GameObject redWinnerScreen;

    [Space(2)]
    [Header("Players and Their Pos")]
    [SerializeField] private GameObject football;
    [SerializeField] private GameObject[] redPlayers;
    [SerializeField] private GameObject[] bluePlayers;
    [SerializeField] private Transform[] redPos;
    [SerializeField] private Transform[] bluePos;

    [Space(2)]
    [Header("Other Things")]
    [SerializeField] private GameObject countdownBG;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject[] joysticks;

    private void Awake() {
        Football_MiniSoccer.onGoalScored += OnGoalScored;
    }

    private void Start() {
        blueWinnerScreen.SetActive(false);
        redWinnerScreen.SetActive(false);

        redPoints = 0;
        bluePoints = 0;

        foreach(GameObject joystick in joysticks) joystick.GetComponent<OnScreenStick>().enabled = false;
        StartCoroutine(Countdown());
    }


    private IEnumerator Countdown() {

        countdownBG.SetActive(true);
        countdownText.gameObject.SetActive(true);

        AudioManager.instance.PlaySound("Countdown");
        for(int i = 3; i > 0; i--) {
            countdownText.text = i.ToString();

            countdownText.transform.localScale = Vector3.one * 1.2f;
            countdownText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);

            yield return new WaitForSeconds(.75f);
        }
        countdownText.gameObject.SetActive(false);
        countdownBG.SetActive(false);

        foreach(GameObject joystick in joysticks) joystick.GetComponent<OnScreenStick>().enabled = true;
    }


    private async void OnGoalScored(PlayerMovements_MiniSoccer.PlayerSide side) {
        AudioManager.instance.PlaySound("Goal");
        if(side == PlayerMovements_MiniSoccer.PlayerSide.Blue) {
            bluePoints++; 
            blueScore.text = (int.Parse(blueScore.text) + 1).ToString();

            await Task.Delay(500);
            if(bluePoints == goalsToScore) GameEnd(PlayerMovements_MiniSoccer.PlayerSide.Blue);
            else RestartGame();
            
        } else if(side == PlayerMovements_MiniSoccer.PlayerSide.Red) {
            redPoints++;
            redScore.text = (int.Parse(redScore.text) + 1).ToString();

            await Task.Delay(500);
            if(redPoints == goalsToScore) GameEnd(PlayerMovements_MiniSoccer.PlayerSide.Red);
            else RestartGame();
        }

    }

    private void RestartGame() {

        foreach(GameObject joystick in joysticks) joystick.GetComponent<OnScreenStick>().enabled = false;

        for(int i = 0; i < bluePlayers.Length; i++) {
            bluePlayers[i].transform.position = bluePos[i].position;
            bluePlayers[i].transform.rotation = bluePos[i].rotation;
            bluePlayers[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            redPlayers[i].transform.position = redPos[i].position;
            redPlayers[i].transform.rotation = redPos[i].rotation;
            redPlayers[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        football.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        football.transform.position = Vector3.zero;

        StartCoroutine(Countdown());
    }

    private void GameEnd(PlayerMovements_MiniSoccer.PlayerSide side) {
        canvas.sortingOrder = 5;
        if(side == PlayerMovements_MiniSoccer.PlayerSide.Blue) {
            Manager_MainMenu.blueMainPoints++;
            blueWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        } else {
            Manager_MainMenu.redMainPoints++;
            redWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
    }


    

}