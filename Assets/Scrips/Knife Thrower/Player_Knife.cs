using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Player_Knife : MonoBehaviour
{
    public enum PlayerSide
    {
        Blue,
        Red
    }

    [SerializeField] private PlayerSide playerSide;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private Transform knifeSpawnPoint;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private float destroyBoundary = 6f;
    [SerializeField] private ParticleSystem playerParticleSystem;

    private Rigidbody2D rb;
    private bool canShoot = true;
    private bool isActive = true;

    private bool collisionProcessed = false;

    private static Transform blueKnifeSpawnPoint;
    private static Transform redKnifeSpawnPoint;
    private static GameObject blueKnifePrefab;
    private static GameObject redKnifePrefab;

    private static bool spawnScheduledBlue = false;
    private static bool spawnScheduledRed = false;

    private static GameObject currentBlueKnife;
    private static GameObject currentRedKnife;

    private static Player_Knife spawnManager;

    private bool hasScheduledSpawn = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        TouchSence_KnifeThrower.onPlayerTouchScreen += OnPlayerTouchScreen;
        rb.gravityScale = 0;
        Debug.Log($"Initialized {playerSide} knife: {gameObject.name}");
        if (spawnManager == null)
            spawnManager = this;
    }

    private void Start()
    {
        if (playerSide == PlayerSide.Blue)
        {
            if (blueKnifeSpawnPoint == null)
            {
                blueKnifeSpawnPoint = knifeSpawnPoint;
                blueKnifePrefab = knifePrefab;
            }
            if (currentBlueKnife == null)
            {
                currentBlueKnife = gameObject;
                Debug.Log($"Registered as current Blue knife: {gameObject.name}");
            }
            else
            {
                Debug.Log("NOT registering as current Blue knife, because one already exists");
            }
        }
        else
        {
            if (redKnifeSpawnPoint == null)
            {
                redKnifeSpawnPoint = knifeSpawnPoint;
                redKnifePrefab = knifePrefab;
            }
            if (currentRedKnife == null)
            {
                currentRedKnife = gameObject;
                Debug.Log($"Registered as current Red knife: {gameObject.name}");
            }
            else
            {
                Debug.Log("NOT registering as current Red knife, because one already exists");
            }
        }
    }

    private void OnDestroy()
    {
        TouchSence_KnifeThrower.onPlayerTouchScreen -= OnPlayerTouchScreen;
        Debug.Log($"Destroying {playerSide} knife: {gameObject.name}");

        if (playerSide == PlayerSide.Blue && currentBlueKnife == gameObject)
        {
            Debug.Log("Clearing current Blue knife reference on destroy");
            currentBlueKnife = null;
        }
        else if (playerSide == PlayerSide.Red && currentRedKnife == gameObject)
        {
            Debug.Log("Clearing current Red knife reference on destroy");
            currentRedKnife = null;
        }
    }

    private void Update()
    {
        CheckBoundaries();
    }

    private void CheckBoundaries()
    {
        bool shouldDestroy = playerSide == PlayerSide.Blue
            ? transform.position.y < -destroyBoundary
            : transform.position.y > destroyBoundary;

        if (shouldDestroy)
        {
            Debug.Log($"{playerSide} knife out of bounds, destroying: {gameObject.name}");
            if (gameObject == GetCurrentKnife())
            {
                Debug.Log($"{playerSide} knife was current, clearing reference and scheduling spawn");
                SetCurrentKnife(null);
                ScheduleSpawn();
            }
            Destroy(gameObject);
        }
    }

    private async void OnPlayerTouchScreen(PlayerSide side)
    {
        if (!canShoot || side != playerSide) return;
        if (GetCurrentKnife() != gameObject)
        {
            Debug.Log($"Ignoring touch - not the current {playerSide} knife");
            return;
        }
        if (playerSide == PlayerSide.Blue)
        {
            if (GameManager_KnifeThrower.noBlueKnifes <= 0) return;
            GameManager_KnifeThrower.noBlueKnifes--;
        }
        else
        {
            if (GameManager_KnifeThrower.noRedKnifes <= 0) return;
            GameManager_KnifeThrower.noRedKnifes--;
        }
        Debug.Log($"Throwing {playerSide} knife");
        await ThrowKnife();
    }

    private async Task ThrowKnife()
    {
        float actualThrowForce = playerSide == PlayerSide.Red ? -throwForce : throwForce;
        rb.gravityScale = playerSide == PlayerSide.Red ? -1 : 1;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * actualThrowForce, ForceMode2D.Impulse);
        canShoot = false;
        await Task.Delay(Mathf.RoundToInt(cooldownTime * 1000));
        canShoot = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionProcessed) return;
        collisionProcessed = true;

        if (!isActive) return;
        isActive = false;
        Debug.Log($"{playerSide} knife collision with {collision.collider.tag}");

        switch (collision.collider.tag)
        {
            case "Board":
                AudioManager.instance.PlaySound("KnifeToWin");
                HandleBoardCollision(collision);
                break;
            case "Knife":
                AudioManager.instance.PlaySound("KnifeToKnife");
                if (this.GetInstanceID() < collision.gameObject.GetInstanceID())
                {
                    HandleKnifeCollision();
                }
                break;
        }
    }

    private void HandleBoardCollision(Collision2D collision)
    {
        playerParticleSystem.Play();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(collision.collider.transform);
        boxCollider.offset = new Vector2(boxCollider.offset.x, -0.4f);
        boxCollider.size = new Vector2(boxCollider.size.x, 1.2f);

        GameManager_KnifeThrower gameManager = FindAnyObjectByType<GameManager_KnifeThrower>();
        if (gameManager != null)
            gameManager.KnifeHitBoard(playerSide);

        if (!hasScheduledSpawn && gameObject == GetCurrentKnife())
        {
            hasScheduledSpawn = true;
            Debug.Log($"Board collision: {playerSide} knife was current, clearing reference and scheduling spawn");
            SetCurrentKnife(null);
            ScheduleSpawn();
        }
    }

    private void HandleKnifeCollision()
    {
        rb.linearVelocity = new Vector2(0f, -2f * (playerSide == PlayerSide.Red ? -1 : 1));

        GameManager_KnifeThrower gameManager = FindAnyObjectByType<GameManager_KnifeThrower>();
        if (gameManager != null)
            gameManager.KnifeHitKnife(playerSide);

        if (!hasScheduledSpawn && gameObject == GetCurrentKnife())
        {
            hasScheduledSpawn = true;
            Debug.Log($"Knife collision: {playerSide} knife was current, clearing reference and scheduling spawn");
            SetCurrentKnife(null);
            ScheduleSpawn();
        }
    }


    private GameObject GetCurrentKnife()
    {
        return playerSide == PlayerSide.Blue ? currentBlueKnife : currentRedKnife;
    }

    private void SetCurrentKnife(GameObject knife)
    {
        if (playerSide == PlayerSide.Blue)
        {
            currentBlueKnife = knife;
            Debug.Log("Set new current Blue knife: " + (knife != null ? knife.name : "null"));
        }
        else
        {
            currentRedKnife = knife;
            Debug.Log("Set new current Red knife: " + (knife != null ? knife.name : "null"));
        }
    }

    private void ScheduleSpawn()
    {
        if (playerSide == PlayerSide.Blue)
        {
            if (spawnScheduledBlue) return;
            spawnScheduledBlue = true;
        }
        else
        {
            if (spawnScheduledRed) return;
            spawnScheduledRed = true;
        }
        spawnManager.StartCoroutine(SpawnNewKnifeCoroutine(playerSide));
    }

    private IEnumerator SpawnNewKnifeCoroutine(PlayerSide side)
    {
        yield return new WaitForSeconds(0.1f);
        SpawnNewKnife(side);
    }

    private static void SpawnNewKnife(PlayerSide side)
    {
        if (side == PlayerSide.Blue)
        {
            spawnScheduledBlue = false;
            if (currentBlueKnife != null)
            {
                Debug.LogWarning("Attempted to spawn new Blue knife when one already exists!");
                return;
            }
            int remainingKnives = GameManager_KnifeThrower.noBlueKnifes;
            if (remainingKnives <= 0)
            {
                Debug.Log("Blue player has no more knives left.");
                return;
            }
            Debug.Log($"Spawning new Blue knife. Remaining: {remainingKnives}");
            Vector3 spawnPosition = blueKnifeSpawnPoint.position;
            Quaternion spawnRotation = Quaternion.identity;
            GameObject newKnife = Instantiate(blueKnifePrefab, spawnPosition, spawnRotation);
            Player_Knife knifeComponent = newKnife.GetComponent<Player_Knife>();
            if (knifeComponent != null)
            {
                knifeComponent.Side = PlayerSide.Blue;
                Rigidbody2D newRb = newKnife.GetComponent<Rigidbody2D>();
                if (newRb != null)
                {
                    newRb.bodyType = RigidbodyType2D.Dynamic;
                    newRb.gravityScale = 0;
                }
                currentBlueKnife = newKnife;
                Debug.Log($"New Blue knife registered: {newKnife.name}");
            }
            else
            {
                Debug.LogError("Blue knife prefab doesn't have Player_Knife component!");
                Destroy(newKnife);
            }
        }
        else
        {
            spawnScheduledRed = false;
            if (currentRedKnife != null)
            {
                Debug.LogWarning("Attempted to spawn new Red knife when one already exists!");
                return;
            }
            int remainingKnives = GameManager_KnifeThrower.noRedKnifes;
            if (remainingKnives <= 0)
            {
                Debug.Log("Red player has no more knives left.");
                return;
            }
            Debug.Log($"Spawning new Red knife. Remaining: {remainingKnives}");
            Vector3 spawnPosition = redKnifeSpawnPoint.position;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, 180);
            GameObject newKnife = Instantiate(redKnifePrefab, spawnPosition, spawnRotation);
            Player_Knife knifeComponent = newKnife.GetComponent<Player_Knife>();
            if (knifeComponent != null)
            {
                knifeComponent.Side = PlayerSide.Red;
                Rigidbody2D newRb = newKnife.GetComponent<Rigidbody2D>();
                if (newRb != null)
                {
                    newRb.bodyType = RigidbodyType2D.Dynamic;
                    newRb.gravityScale = 0;
                }
                currentRedKnife = newKnife;
                Debug.Log($"New Red knife registered: {newKnife.name}");
            }
            else
            {
                Debug.LogError("Red knife prefab doesn't have Player_Knife component!");
                Destroy(newKnife);
            }
        }
    }
    public PlayerSide Side
    {
        get { return playerSide; }
        set { playerSide = value; }
    }
}
