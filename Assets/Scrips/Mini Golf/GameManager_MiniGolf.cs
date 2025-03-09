using TMPro;
using UnityEngine;

public class GameManager_MiniGolf : MonoBehaviour {

    public enum PlayerTurn {
        Blue,
        Red
    }

    [SerializeField] private MapsInfo_MiniGolf[] golfMaps;
    [SerializeField] private GameObject exitButton;

    [Space(2)]
    [SerializeField] private GameObject golfBall;

    [Space(2)]
    [Header("Others")]
    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private GameObject blueWinnerScreen;
    [SerializeField] private GameObject redWinnerScreen;

    [HideInInspector] public static int bluePoints = 0;
    [HideInInspector] public static int redPoints = 0;
    [HideInInspector] public static PlayerTurn playerTurn;
    private GameObject redMap;
    private GameObject blueMap;
    private GameObject hole;
    private Transform blueSpawn;
    private Transform redSpawn;


    private int currentMap;

    private void Awake() {
        GolfBall_Golf.onPlayerScore += OnPlayerScoreHole;
        GolfBall_Golf.onChangeTurn += OnTurnChange;
    }

    private void Start() {
        currentMap = -1;
        foreach(MapsInfo_MiniGolf maps in golfMaps) {
            maps.gameObject.SetActive(false);
        }

        redWinnerScreen.SetActive(false);
        blueWinnerScreen.SetActive(false);

        ChangeMap();
    }

    private void ChangeMap() {
        golfBall.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        golfBall.SetActive(false);

        hole = null;
        redMap = null;
        blueMap = null;
        redSpawn = null;
        blueSpawn = null;

        if(currentMap != -1) 
            golfMaps[currentMap].gameObject.SetActive(false);
        

        currentMap = Random.Range(0, golfMaps.Length);
        golfMaps[currentMap].gameObject.SetActive(true);

        blueSpawn = golfMaps[currentMap].blueSpawn;
        redSpawn = golfMaps[currentMap].redSpawn;
        blueMap = golfMaps[currentMap].blueMap.gameObject;
        redMap = golfMaps[currentMap].redMap.gameObject;
        hole = golfMaps[currentMap].hole;

        redMap.SetActive(false);
        blueMap.SetActive(true);

        playerTurn = PlayerTurn.Blue;

        hole.transform.position = redSpawn.position;

        golfBall.transform.position = blueSpawn.position;
        golfBall.SetActive(true);
    }


    private void OnTurnChange() {
        golfBall.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if(playerTurn == PlayerTurn.Blue) {
            golfBall.SetActive(false);

            blueMap.SetActive(false);
            redMap.SetActive(true);

            hole.transform.position = blueSpawn.position;
            playerTurn = PlayerTurn.Red;

            golfBall.transform.position = redSpawn.position;
            golfBall.SetActive(true);

        } else {
            playerTurn = PlayerTurn.Blue;
            ChangeMap();

        }
    }

    private void OnPlayerScoreHole(PlayerTurn playerScored) {
        if(playerScored == PlayerTurn.Red) {
            redPoints++;
            redScore.text = redPoints.ToString();
            if(redPoints - bluePoints == 2) {
                exitButton.SetActive(false);
                redWinnerScreen.SetActive(true);
                Manager_MainMenu.redMainPoints++;
                StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                golfBall.GetComponent<GolfBall_Golf>().enabled = false;
            }

        } else{
            bluePoints++;
            blueScore.text = bluePoints.ToString();
            if(bluePoints - redPoints == 2) {
                exitButton.SetActive(false);
                blueWinnerScreen.SetActive(true);
                Manager_MainMenu.blueMainPoints++;
                StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                golfBall.GetComponent<GolfBall_Golf>().enabled = false;
            }

        }
    }

}