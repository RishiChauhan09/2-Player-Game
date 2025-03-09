using Terresquall;
using UnityEngine;

public class GameManager_Tanks : MonoBehaviour {

    [SerializeField] private GameObject[] bluePointsImages;
    [SerializeField] private GameObject[] redPointsImages;

    [SerializeField] private GameObject redWinnerScreen;
    [SerializeField] private GameObject blueWinnerScreen;

    [SerializeField] private GameObject[] allGamePads;

    public static int redPoints = 5;
    public static int bluePoints = 5;

    public static bool redMoving = false;
    public static bool blueMoving = false;

    private void Awake() {
        Bullet_Tanks.onBulletHitPlayer += OnBulletHigPlayer;
        AudioManager.Initilize();
    }

    private void Start() {
        redWinnerScreen.SetActive(false);
        blueWinnerScreen.SetActive(false);
    }

    private void OnBulletHigPlayer(PlayerMovements_Tanks.PlayerSide side) {
        CineMachineShake.Instance.ShakeCamera(5f, .1f);
        if(side == PlayerMovements_Tanks.PlayerSide.Blue) {
            bluePoints--;
            bluePointsImages[bluePoints].SetActive(false);
            if(bluePoints == 0) GameOver(PlayerMovements_Tanks.PlayerSide.Red);

        } else {
            redPoints--;
            redPointsImages[redPoints].SetActive(false);
            if(redPoints == 0) GameOver(PlayerMovements_Tanks.PlayerSide.Blue);
        }
    }


    private void GameOver(PlayerMovements_Tanks.PlayerSide sideWon) {
        AudioManager.instance.PlaySound("TankFinish");
        foreach(GameObject pads in allGamePads) {
            pads.GetComponent<VirtualJoystick>().enabled = false;
            Destroy(pads.GetComponent<VirtualJoystick>().gameObject);
        }
        if(sideWon == PlayerMovements_Tanks.PlayerSide.Red) {
            Manager_MainMenu.redMainPoints++;
            redWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        } else {
            Manager_MainMenu.blueMainPoints++;
            blueWinnerScreen.SetActive(true);
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
    }
}