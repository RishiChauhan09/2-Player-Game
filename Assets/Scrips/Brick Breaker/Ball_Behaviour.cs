using UnityEngine;

public class Ball_Behaviour : MonoBehaviour
{
    public float initialSpeed = 5f;
    [SerializeField] private float maxVelocity = 12;
    public Vector2 initialDirection = new Vector2(0.5f, 1f);
    private int FallRange = 5;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialDirection.Normalize();
        rb.linearVelocity = initialDirection * initialSpeed;
    }

    private void Update()
    {
        if (transform.position.y < -FallRange)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.BallLost(Player_Movement.PlayerSide.Blue, gameObject);
            }
            Destroy(gameObject);
        }
        else if (transform.position.y > FallRange)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.BallLost(Player_Movement.PlayerSide.Red, gameObject);
            }
            Destroy(gameObject);
        }

        if (rb.linearVelocity.magnitude < initialSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * initialSpeed;
        }

        HandleGravity();
    }

    // handling gravity so that there is no things like U turn 
    private void HandleGravity() { 
        if(transform.position.y > 0) {
            rb.gravityScale = -1;
        } else {
            rb.gravityScale = 1;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            AudioManager.instance.PlaySound("BoxBlast");
            Destroy(collision.gameObject);
        }

        Player_Movement player = collision.gameObject.GetComponent<Player_Movement>();
        if (player != null)
        {
            Vector2 currentVelocity = rb.linearVelocity;
            if (player.playerSide == Player_Movement.PlayerSide.Blue)
            {
                float velocity = Mathf.Abs(currentVelocity.y) + 2f;
                if(Mathf.Abs(currentVelocity.y) + 2f > maxVelocity) velocity = maxVelocity;
                rb.linearVelocity = new Vector2(currentVelocity.x, velocity);
            }
            else // Red one keliye
            {
                float velocity = -Mathf.Abs(currentVelocity.y) - 2f;
                if(-Mathf.Abs(currentVelocity.y) - 2f < -maxVelocity) velocity = -maxVelocity;
                rb.linearVelocity = new Vector2(currentVelocity.x, velocity);
            }
        }
    }
}