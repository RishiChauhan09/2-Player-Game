using TMPro;
using UnityEngine;

public class Football_SoccerPool : MonoBehaviour {

    private const string RED_GOAL_TAG = "Red Goal";
    private const string BLUE_GOAL_TAG = "Blue Goal";

    [Header("Scores")]
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private TMP_Text redScore;

    [SerializeField] private GameObject footballShadow;

    public delegate void OnGoalScored(PlayerMovements_SoccerPool.PlayerSide side, GameObject goalSide);
    public static event OnGoalScored onGoalScored;

    public delegate void OnGameFinished(PlayerMovements_SoccerPool.PlayerSide side);
    public static event OnGameFinished onGameFinished;

    private void LateUpdate() {
        footballShadow.transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.z * -1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(GameManager_SoccerPool.goalScored) return;

        if(collision.gameObject.tag == RED_GOAL_TAG) {

            // blue scored
            AudioManager.instance.PlaySound("Goal");
            GameManager_SoccerPool.previousPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Red;
            GameManager_SoccerPool.currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Null;

            blueScore.text = (int.Parse(blueScore.text) + 1).ToString();

            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if(int.Parse(blueScore.text) == 3) onGameFinished?.Invoke(PlayerMovements_SoccerPool.PlayerSide.Blue);
            else onGoalScored?.Invoke(PlayerMovements_SoccerPool.PlayerSide.Blue, collision.gameObject);

        } else if(collision.gameObject.tag == BLUE_GOAL_TAG) {

            // red scored to goal 
            AudioManager.instance.PlaySound("Goal");

            GameManager_SoccerPool.previousPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Blue;
            GameManager_SoccerPool.currentPlayerTurn = PlayerMovements_SoccerPool.PlayerSide.Null;

            redScore.text = (int.Parse(redScore.text) + 1).ToString();

            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if(int.Parse(redScore.text) == 3) onGameFinished?.Invoke(PlayerMovements_SoccerPool.PlayerSide.Red);
            else onGoalScored?.Invoke(PlayerMovements_SoccerPool.PlayerSide.Red, collision.gameObject);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(!GameManager_SoccerPool.isSettingUp)  AudioManager.instance.PlaySound("Hit");
    }

}