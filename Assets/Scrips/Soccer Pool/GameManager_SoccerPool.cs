using System.Threading.Tasks;
using UnityEngine;

public class GameManager_SoccerPool : MonoBehaviour {

    private const string BLUE_HEX = "#5D6FFF";
    private const string RED_HEX = "#FF8A88";
    private Color blueColor;
    private Color redColor;


    public static PlayerMovements_SoccerPool.PlayerSide previousPlayerTurn;
    public static PlayerMovements_SoccerPool.PlayerSide currentPlayerTurn;

    [Space(2)]
    [Header("Teams")]
    [SerializeField] private GameObject[] blueTeam;
    [SerializeField] private GameObject[] redTeam;
    [SerializeField] private GameObject football;

    [Space(2)]
    [Header("Postions")]
    [SerializeField] private Transform[] blueTeamPositions;
    [SerializeField] private Transform[] redTeamPositions;
    [SerializeField] private Transform footballPosition;

    [Space(2)]
    [Header("UIs")]
    [SerializeField] private GameObject redWinnerScreen;
    [SerializeField] private GameObject blueWinnerScreen;

    [Space(2)]
    [Header("Celebration Particle System")]
    [SerializeField] private ParticleSystem blueWinnerParticleSystem;
    [SerializeField] private ParticleSystem redWinnerParticleSystem;

    [Space(2)]
    [Header("Other Things")]
    [SerializeField] private float timeToReset = 1.5f;
    [SerializeField] private float timeToWaitAfterReset = 1.5f;

    private Camera mainCamera;
    [HideInInspector] public static bool goalScored;
    [HideInInspector] public static bool isSettingUp;


    private void Awake() {
        // add all functions to events 
        Football_SoccerPool.onGoalScored += OnGoalScored;
        PlayerMovements_SoccerPool.onPlayerSlowDown += OnPlayerSlowDown;
        PlayerMovements_SoccerPool.onPlayerReleaseTouch += PlayerReleaseTouch;
        Football_SoccerPool.onGameFinished += OnGameFinished;
    }

    private void Start() {

        // commented for future use if player positions are not correct or any bug in player position in start of game

/*        // setting up all the positions
        for(int i = 0; i < blueTeam.Length; i++) {
            blueTeam[i].transform.position = blueTeamPositions[i].transform.position;
        }

        for(int i = 0; i < blueTeam.Length; i++) {
            blueTeam[i].transform.position = blueTeamPositions[i].transform.position;
        }

        football.transform.position = footballPosition.transform.position;
*/

        mainCamera = Camera.main;

        if(ColorUtility.TryParseHtmlString(BLUE_HEX, out blueColor)) { }
        if(ColorUtility.TryParseHtmlString(RED_HEX, out redColor)) { }

        // setting winner screens disable 
        redWinnerScreen.SetActive(false);
        blueWinnerScreen.SetActive(false);  

        // setting particle system off
        blueWinnerParticleSystem.gameObject.SetActive(false);
        redWinnerParticleSystem.gameObject.SetActive(false);

        // setting up the player turn 
        currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Blue;
        previousPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Red;


        // setting outline of all players 
        foreach(GameObject player in blueTeam) {
            player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(true);
            mainCamera.backgroundColor = blueColor;   
        }

        foreach(GameObject player in redTeam) {
            player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(false);
        }

        goalScored = false;
    }


    private void OnDisable() {
        PlayerMovements_SoccerPool.onPlayerSlowDown -= OnPlayerSlowDown;
        PlayerMovements_SoccerPool.onPlayerReleaseTouch -= PlayerReleaseTouch;
    }


    private void PlayerReleaseTouch() {
        previousPlayerTurn = currentPlayerTurn;
        currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Null;

        if(previousPlayerTurn == PlayerMovements_SoccerPool.PlayerSide.Blue) {
            foreach(GameObject player in blueTeam) {
                player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(false);
            } 
        } else if (previousPlayerTurn == PlayerMovements_SoccerPool.PlayerSide.Red) {
            foreach(GameObject player in redTeam) {
                player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(false);
            }
        }

    }


    private async void OnPlayerSlowDown() {

        foreach(GameObject p in blueTeam) {
            await WaitForObjectToStop(p);
        }
        foreach(GameObject p in redTeam) {
            await WaitForObjectToStop(p);
        }
        await WaitForObjectToStop(football);

        if(goalScored) {
            // waiting for time because it should take time give outline to player 
            await Task.Delay((int)timeToWaitAfterReset * 1000); // multiplying my 1000 because it takes time in mili seconds
            goalScored = false;
        }

        if(previousPlayerTurn == PlayerMovements_SoccerPool.PlayerSide.Blue) {
            currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Red;
            mainCamera.backgroundColor = redColor;
            foreach(GameObject player in redTeam) {
                player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(true);
            }

        } else if(previousPlayerTurn == PlayerMovements_SoccerPool.PlayerSide.Red) {
            currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Blue;
            mainCamera.backgroundColor = blueColor;
            foreach(GameObject player in blueTeam) {
                player.GetComponent<PlayerMovements_SoccerPool>().playerTurnOutline.SetActive(true);
            }
        }

    }

    private async Task WaitForObjectToStop(GameObject obj) {
        while(obj.GetComponent<Rigidbody2D>().linearVelocity.magnitude >= 0.1f) {
            await Task.Yield();
        }
    }


    private async void OnGoalScored(PlayerMovements_SoccerPool.PlayerSide side, GameObject goalSide) {

        isSettingUp = true;
        football.GetComponent<TrailRenderer>().enabled = false;
        football.GetComponent<Rigidbody2D>().linearVelocity *= .1f;
        goalScored = true;

        if(side == PlayerMovements_SoccerPool.PlayerSide.Blue){ 
            blueWinnerParticleSystem.gameObject.SetActive(true);
            blueWinnerParticleSystem.Play();
        } else if(side == PlayerMovements_SoccerPool.PlayerSide.Red) {
            redWinnerParticleSystem.gameObject.SetActive(true);
            redWinnerParticleSystem.Play();
        }

        await Task.Delay(2000);
        if(side == PlayerMovements_SoccerPool.PlayerSide.Blue) blueWinnerParticleSystem.gameObject.SetActive(false);
        else if(side == PlayerMovements_SoccerPool.PlayerSide.Red) redWinnerParticleSystem.gameObject.SetActive(false);

        // resetting all position of players
        var task = new Task[blueTeam.Length + redTeam.Length + 1]; // + 1 for football
        for(int i = 0; i < blueTeam.Length; i++) {
            task[i] = SetUpPlayer(blueTeam[i], timeToReset, blueTeam[i].transform.position, blueTeamPositions[i].position);
        }
        for(int i = 0; i < redTeam.Length; i++) {
            // here im adding 5 because it has already taken blue team so adding 5
            task[i + 5] = SetUpPlayer(redTeam[i], timeToReset, redTeam[i].transform.position, redTeamPositions[i].position);
        }
        task[10] = SetUpPlayer(football, timeToReset, football.transform.position, footballPosition.position);

        await Task.WhenAll(task);
        goalSide.GetComponent<BoxCollider2D>().enabled = true;
        football.GetComponent<TrailRenderer>().enabled = true;
        isSettingUp = false;
    }


    private async Task SetUpPlayer(GameObject obj, float duration, Vector3 startPos, Vector3 endPos) {
        float elapsedTime = 0f;

        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;  
            float t = elapsedTime / duration;
            obj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            obj.transform.position = Vector3.Lerp(startPos, endPos, t);
            await Task.Yield();
        }

        obj.transform.position = endPos;
    }


    private void OnGameFinished(PlayerMovements_SoccerPool.PlayerSide side) {
        if(side == PlayerMovements_SoccerPool.PlayerSide.Blue) {
            Manager_MainMenu.blueMainPoints++;
            blueWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        } else if (side == PlayerMovements_SoccerPool.PlayerSide.Red) {
            Manager_MainMenu.redMainPoints++;
            redWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
    }



}