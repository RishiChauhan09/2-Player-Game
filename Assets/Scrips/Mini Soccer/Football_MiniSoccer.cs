using UnityEngine;

public class Football_MiniSoccer : MonoBehaviour {

    private const string RED_GOAL_TAG = "Red Goal";
    private const string BLUE_GOAL_TAG = "Blue Goal";

    [SerializeField] private float forceFromPlayer;
    private Rigidbody2D rb;

    public delegate void OnGoalScored(PlayerMovements_MiniSoccer.PlayerSide side);
    public static event OnGoalScored onGoalScored;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.instance.PlaySound("Hit");
        if(collision.gameObject.tag == "Player") {
            Vector2 normal = Vector2.zero;

            foreach(ContactPoint2D contactPoint in collision.contacts) { // collision.contacts gives array
                normal = contactPoint.normal;
            }
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(normal * forceFromPlayer, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.gameObject.tag == RED_GOAL_TAG) {
            onGoalScored?.Invoke(PlayerMovements_MiniSoccer.PlayerSide.Blue);
        } else if(collision.gameObject.tag == BLUE_GOAL_TAG) {
            onGoalScored?.Invoke(PlayerMovements_MiniSoccer.PlayerSide.Red);
        }
    }

}