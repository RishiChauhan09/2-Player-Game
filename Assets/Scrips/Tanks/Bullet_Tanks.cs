using UnityEngine;

public class Bullet_Tanks : MonoBehaviour {

    [SerializeField] private float forceOfBuller = 4f;

    // events and delegate
    public delegate void OnBulletHitPlayer(PlayerMovements_Tanks.PlayerSide side);
    public static event OnBulletHitPlayer onBulletHitPlayer;

    private void Start() {
        GetComponent<Rigidbody2D>().AddForce(transform.up * forceOfBuller);
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.gameObject.tag == "Player") {
            onBulletHitPlayer?.Invoke(collision.gameObject.GetComponent<PlayerMovements_Tanks>().playerSide);
            AudioManager.instance.PlaySound("TankDamage");
        }

        Destroy(gameObject);
    }


}
