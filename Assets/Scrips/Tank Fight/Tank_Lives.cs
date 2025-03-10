using TMPro;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public int BlueTankLives = 3;
    public int RedTankLives = 3;
    public GameObject BlueWinnerScreen;
    public GameObject RedWinnerScreen;
    private bool isGameOver = false;

    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text blueScoreText;

    private void Start()
    {
        Utils.ChangeToLandScape();
        InitializeGame();
        blueScoreText.text = BlueTankLives.ToString();
        redScoreText.text = RedTankLives.ToString();
    }

    private void InitializeGame()
    {
        BlueTankLives = 6;
        RedTankLives = 6;
        isGameOver = false;
    }

    public void TakeDamage(CannonController.Player hitPlayer)
    {
        if (isGameOver) return;
        HandleTankHit(hitPlayer);
    }

    void HandleTankHit(CannonController.Player hitPlayer)
    {
        if (isGameOver) return;
        Debug.Log($"Player {hitPlayer} was hit!");

        if (CineMachineShake.Instance != null)
        {
            CineMachineShake.Instance.ShakeCamera(5f, .1f);
        }

        if (hitPlayer == CannonController.Player.Blue)
        {
            AudioManager.instance.PlaySound("Damage");
            BlueTankLives--;
            blueScoreText.text = BlueTankLives.ToString();
            if (BlueTankLives <= 0)
            {
                HandleGameOver(CannonController.Player.Red);
            }
        }
        else if (hitPlayer == CannonController.Player.Red)
        {
            AudioManager.instance.PlaySound("Damage");
            RedTankLives--;
            redScoreText.text = RedTankLives.ToString();
            if (RedTankLives <= 0)
            {
                HandleGameOver(CannonController.Player.Blue);
            }
        }

        if (BlueTankLives <= 1)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        if (RedTankLives <= 1)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void HandleGameOver(CannonController.Player winner)
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log($"Game Over! {winner} wins!");

        if (winner == CannonController.Player.Blue)
        {
            if (BlueWinnerScreen) BlueWinnerScreen.SetActive(true);
            Manager_MainMenu.blueMainPoints++;
            AudioManager.instance.PlaySound("Finish");
            Utils.ChangeToPortrait();
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
        else
        {
            if (RedWinnerScreen) RedWinnerScreen.SetActive(true);
            Manager_MainMenu.redMainPoints++;
            AudioManager.instance.PlaySound("Finish");
            Utils.ChangeToPortrait();
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
    }
}
