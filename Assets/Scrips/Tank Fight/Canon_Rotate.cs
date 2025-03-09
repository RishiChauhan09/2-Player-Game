using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    public enum Player { Blue, Red }
    public Player player;
    public float rotationSpeed = 50f;
    public float minRotationAngle = -45f;
    public float maxRotationAngle = 45f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    [SerializeField] private Vector3 rotationOffset;
    private float bulletLifetime = 2f;
    private float lastShotTime = -Mathf.Infinity;
    private readonly float shootCooldown = 0.7f;
    private bool rotatingUp = true;

    private void Update()
    {
        HandleRotation();
        DestroyOutOfBoundsBullets();
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current != null)
        {
            var primaryTouch = Touchscreen.current.primaryTouch;
            if (primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPosition = primaryTouch.position.ReadValue();
                if (player == Player.Blue && touchPosition.x < Screen.width / 2)
                {
                    Shoot();
                }
                else if (player == Player.Red && touchPosition.x >= Screen.width / 2)
                {
                    Shoot();
                }
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (player == Player.Blue && mousePosition.x < Screen.width / 2)
            {
                Shoot();
            }
            else if (player == Player.Red && mousePosition.x >= Screen.width / 2)
            {
                Shoot();
            }
        }
    }

    void HandleRotation()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime * (rotatingUp ? 1 : -1);
        float newRotationAngle = transform.localEulerAngles.z + rotationAmount;
        if (newRotationAngle > 180) newRotationAngle -= 360;
        if (newRotationAngle < -180) newRotationAngle += 360;
        newRotationAngle = Mathf.Clamp(newRotationAngle, minRotationAngle, maxRotationAngle);
        transform.localEulerAngles = new Vector3(0, 0, newRotationAngle);

        if (Mathf.Approximately(newRotationAngle, maxRotationAngle))
        {
            rotatingUp = false;
        }
        else if (Mathf.Approximately(newRotationAngle, minRotationAngle))
        {
            rotatingUp = true;
        }
    }

    public void Shoot()
    {
        AudioManager.instance.PlaySound("Fire");
        if (Time.time < lastShotTime + shootCooldown)
        {
            Debug.Log($"{player} cannot shoot yet! Cooldown in progress.");
            return;
        }
        lastShotTime = Time.time;
        Debug.Log($"{player} is shooting!");

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet has no Rigidbody2D!");
            return;
        }

        Vector2 direction = player == Player.Blue ? bulletSpawnPoint.right : -bulletSpawnPoint.right;
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.ownerPlayer = player;
        }

        Destroy(bullet, bulletLifetime);
    }

    void DestroyOutOfBoundsBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Vector3 pos = bullet.transform.position;
            if (pos.y < -5.5f || pos.x < -10f || pos.x > 10f)
            {
                Destroy(bullet);
            }
        }
    }
}
