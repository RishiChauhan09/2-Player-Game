using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Ball_Tennis : MonoBehaviour {

    private const string RED_GOAL = "Red Goal";
    private const string BLUE_GOAL = "Blue Goal";

    [Header("All Ball Things")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float forceToAdd;

    [SerializeField] private float timeToDestoryImage = .4f;
    [SerializeField] private float force = 14;
    private float normalGravity;

    [Space(2)]
    [Header("Power Shot")]
    [SerializeField] private float powerShotForce = 16f;
    [SerializeField] private float powerShotGravity = 1f;

    [Space(2)]
    [Header("Other Things")]
    [SerializeField] private GameObject sparkleImage;

    [Space(2)]
    [Header("Particle System")]
    [SerializeField] private ParticleSystem powerShotTrailParticleSystem;
    [SerializeField] private ParticleSystem powerShotParticleSystem;


    public delegate void OnPointScored(PlayerMovements_Tennis.PlayerSide pointSide);
    public static event OnPointScored onPonitScored;

    public delegate void OnBallTouchRacket(PlayerMovements_Tennis.PlayerSide side);
    public static event OnBallTouchRacket onBallTouchRacket;


    private void Awake() {
        Eyes_Tennis.ball = gameObject;
    }

    private void Start() {
        normalGravity = rb.gravityScale;

        powerShotParticleSystem.Clear();
        powerShotParticleSystem.Stop();
        powerShotTrailParticleSystem.Clear();
        powerShotTrailParticleSystem.Stop();

        if(transform.position.x < 0) {
            rb.AddForce(Vector2.right * forceToAdd, ForceMode2D.Impulse);
        } else {
            rb.AddForce(Vector2.left * forceToAdd, ForceMode2D.Impulse);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("trigger entered");
        if(collision.tag == RED_GOAL) {
            onPonitScored?.Invoke(PlayerMovements_Tennis.PlayerSide.Blue);
            Destroy(gameObject, 2f);
        } else if(collision.tag == BLUE_GOAL) {
            onPonitScored?.Invoke(PlayerMovements_Tennis.PlayerSide.Red);
            Destroy(gameObject, 2f);
        }
    }

    /// <summary>
    /// 1. using normal it made good for x but for y it was giving pretty bad results 
    /// 2. using height it was default x = 1 or -1 so it was good for y 
    /// 3. so i mixed bot and here final outcome
    /// 
    /// here in unity im using 2 colliders 1 polygon for upper racket and box for lower when i tried using polygon for whole the results were not as expected and were bad 
    /// and also 
    /// let the force be 14 not too high not too low and im not inceasing the force after every turn 
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            Vector2 ballPosition = transform.position;
            Vector2 racketPos = collision.transform.position;
            float racketHeight = collision.collider.bounds.size.y;

            float posX;

            // getting the racket script and checking for power shot
            bool isPowerShot = collision.gameObject.GetComponent<Racket_Tennis>().IsPowerShot(collision);
            onBallTouchRacket?.Invoke(transform.position.x < 0 ? PlayerMovements_Tennis.PlayerSide.Red : PlayerMovements_Tennis.PlayerSide.Blue);
            rb.linearVelocity = Vector2.zero;
            Vector2 collisionPoint = collision.GetContact(0).point;

            if(powerShotTrailParticleSystem.isPlaying) {
                rb.gravityScale = normalGravity;
                powerShotTrailParticleSystem.Stop();
            }

            if(isPowerShot) {
                AudioManager.instance.PlaySound("Powershot");
                //Debug.Log("power shot");
                //powerShotParticleSystem.Play();

                rb.gravityScale = powerShotGravity;
                rb.linearVelocity = Vector2.zero;
                //rb.simulated = false;
                //gameObject.GetComponent<CircleCollider2D>().isTrigger = true;

                //onPowerShot?.Invoke(transform.position.x < 0 ? PlayerMovements_Tennis.PlayerSide.Red : PlayerMovements_Tennis.PlayerSide.Blue);

                //transform.rotation = Quaternion.identity;
                //rb.simulated = true;

                //gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
                //powerShotParticleSystem.Stop();
                powerShotTrailParticleSystem.Play();

                Vector2 powerShotDirection = transform.position.x < 0 ? Vector2.left : Vector2.right;
                //rb.linearVelocity = Vector2.zero;
                rb.AddForce(powerShotDirection * powerShotForce, ForceMode2D.Impulse);
                return;

            }

            // if not power shot then : 
            AudioManager.instance.PlaySound("Hit");
            Vector2 normal = collision.contacts[0].normal;

            if(Mathf.Abs(normal.x) < 0.9f) {
                posX = normal.x > 0 ? .9f : -.9f;
            } else {
                posX = normal.x;
            }

            GameObject sparkle = Instantiate(sparkleImage, collisionPoint, Quaternion.identity);
            Destroy(sparkle, timeToDestoryImage);

            float posY = (ballPosition.y - racketPos.y) / racketHeight;

            Vector2 direction = new Vector2(posX, posY);
            direction.Normalize();

            rb.AddForce(direction * force, ForceMode2D.Impulse);

        }
    }

    private void OnDestroy() {
        Debug.Log(transform.position);
    }

}