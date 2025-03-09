using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager_MiniGolf;

public class GolfBall_Golf : MonoBehaviour {

    private const string HOLE_TAG = "Hole";

    [SerializeField] private Rigidbody2D rb;

    [Header("Values")]
    [SerializeField] private float force = 5f;
    [SerializeField] private float ballRadius = .1f;
    [SerializeField] private float dragLimit = 1.5f;
    [SerializeField] private float velocityOffset = .1f;

    [Space(2)]
    [Header("Others")]
    [SerializeField] private GameObject border;
    [SerializeField] private LineRenderer backLine;

    private PlayerInputActions playerInputActions;
    private Camera mainCamera;
    private bool isTouching;
    [SerializeField] public static bool turnPlayed;

    public delegate void OnChangeTurn();
    public static event OnChangeTurn onChangeTurn;

    public delegate void OnPlayerScore(PlayerTurn playerScored);
    public static event OnPlayerScore onPlayerScore;


    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.MiniGolf.Enable();

        playerInputActions.MiniGolf.PrimaryTouch.performed += PrimaryTouch;
        playerInputActions.MiniGolf.PrimaryTouch.canceled += TouchCanceled;
    }

    private void Start() {
        mainCamera = Camera.main;

        border.SetActive(false);
        backLine.enabled = false;
        turnPlayed = false;
    }

    private void OnEnable() {
        playerInputActions.MiniGolf.Enable();
    }

    private void OnDisable() {
        playerInputActions.MiniGolf.Disable();
    }


    private Vector2 PrimaryPosition() {
        return Utils.ScreenToWorld(mainCamera, playerInputActions.MiniGolf.PrimaryPosition.ReadValue<Vector2>());
    }


    private void PrimaryTouch(InputAction.CallbackContext obj) {
        if(turnPlayed) return;

        Vector2 pos = PrimaryPosition();
        Vector2 playerPos = transform.position;

        if(Vector2.Distance(pos, playerPos) < ballRadius) {
            backLine.SetPosition(0, playerPos);
            isTouching = true;
            border.SetActive(true);
            backLine.enabled = true;
        }
    }

    private void TouchCanceled(InputAction.CallbackContext obj) {
        if(turnPlayed || !isTouching) return;
        turnPlayed = true;
        isTouching = false;

        Vector2 startPos = backLine.GetPosition(0);
        Vector2 lastPos = backLine.GetPosition(1);
        Vector2 distance = lastPos - startPos;

        Vector2 finalForce = distance * force;
        rb.AddForce(-finalForce, ForceMode2D.Impulse);

        border.SetActive(false);
        backLine.enabled = false;
    }


    private void Update() {
        if(isTouching) {
            Vector2 startPos = transform.position;
            Vector2 currentPos = PrimaryPosition();
            Vector2 distance = currentPos - startPos;

            if(distance.magnitude <= dragLimit) {
                backLine.SetPosition(1, currentPos);
            } else {
                Vector2 lastPos = startPos + (distance.normalized * dragLimit);
                backLine.SetPosition(1, lastPos);
            }
        }

        if(rb.linearVelocity.magnitude < velocityOffset && turnPlayed) {
            onChangeTurn?.Invoke();
            turnPlayed = false;
        }

    }


    private async void OnTriggerEnter2D(Collider2D collision) {
        if(!turnPlayed) return;

        if(collision.gameObject.tag == HOLE_TAG) {
            AudioManager.instance.PlaySound("Goal");
            transform.DOMove(collision.gameObject.transform.position, 1f).SetEase(Ease.Linear);
            transform.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear);

            await Task.Delay(1000); // 1 second delay

            onPlayerScore?.Invoke(playerTurn);
            onChangeTurn?.Invoke();

            turnPlayed = false;
            transform.localScale = Vector3.one;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.instance.PlaySound("Hit");
    }

}