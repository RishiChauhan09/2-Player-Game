using UnityEngine;

public class Bullet : MonoBehaviour
{
    public CannonController.Player ownerPlayer { get; set; }
    private Game_Manager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<Game_Manager>();
        if (gameManager == null)
        {
            Debug.LogError("Game_Manager not found in Bullet! Make sure it's in the scene.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CannonController tankController = collision.collider.GetComponent<CannonController>();

        if (tankController == null)
        {
            tankController = collision.collider.GetComponentInParent<CannonController>();
        }

        if (tankController != null && gameManager != null)
        {
            if (tankController.player != ownerPlayer)
            {
                Debug.Log($"Bullet from {ownerPlayer} hit {tankController.player} tank!");
                gameManager.TakeDamage(tankController.player);
                Destroy(gameObject);
            }
        }
    }
}
