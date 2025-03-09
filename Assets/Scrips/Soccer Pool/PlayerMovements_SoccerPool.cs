using System;
using UnityEngine;

public class PlayerMovements_SoccerPool : MonoBehaviour {

    public enum PlayerSide {
        Red, 
        Blue,
        Null
    }

    private Rigidbody2D playerRB;
    [SerializeField] private LineRenderer frontLineRenderer;

    [Space(2)]
    [Header("Values")]
    [SerializeField] private float force = 5f;
    [SerializeField] private float dragLimit = 1.5f;
    [SerializeField] private float mainCameraSize = 5f;
    [SerializeField] private float cameraZoomOut = 1.5f;
    [SerializeField] private float timeToCameraZoomIn = 1f;
    [SerializeField] private float playerRadius = 0.5f;
    [SerializeField] private float velocityOffset = 0.1f;
    [SerializeField] private float turnCancelOffset = 0.2f;

    [Header("Other Player Things")]
    [SerializeField] public GameObject playerTurnOutline;
    [SerializeField] private PlayerSide playerSide;
    [SerializeField] private ExitButton exitButton;
    [SerializeField] private GameObject scoreBoard;


    [HideInInspector] public bool turnPlayed = false;
    private Camera mainCamera;
    private bool isTouching;
    private LineRenderer backLIneRenderer;
    private PlayerInputActions inputActions;
    private bool cameraZoomingIn = false;
    private float elapsedTimeForCameraToZoomIn;
    private float forceAddedToPlayer = 0f;

    public static event Action onPlayerSlowDown;
    public static event Action onPlayerReleaseTouch;

    private void Awake() {
        playerRB = GetComponent<Rigidbody2D>();
        backLIneRenderer = GetComponent<LineRenderer>();
        inputActions = new PlayerInputActions();
        mainCamera = Camera.main; // first camera component 
    }


    private void OnEnable() {
        inputActions.SoccerPool.Enable();
    }


    private void OnDisable() {
        inputActions.SoccerPool.Disable();
    }


    private void Start() {
        backLIneRenderer.enabled = false;
        backLIneRenderer.positionCount = 2;

        frontLineRenderer.enabled = false;
        frontLineRenderer.positionCount = 2;

        inputActions.SoccerPool.PrimaryTouch.started += PrimaryTouch_started;
        inputActions.SoccerPool.PrimaryTouch.canceled += PrimaryTouch_canceled;
    }


    private Vector2 PrimaryPosition() {
        return Utils.ScreenToWorld(mainCamera, inputActions.SoccerPool.PrimaryPosition.ReadValue<Vector2>());
    }

    private void PrimaryTouch_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        Vector2 pos = PrimaryPosition();
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

        if(Vector2.Distance(pos, playerPos) < playerRadius && GameManager_SoccerPool.currentPlayerTurn == playerSide && playerRB.linearVelocity.magnitude <= velocityOffset) {
            backLIneRenderer.enabled = true;
            frontLineRenderer.enabled = true;
            isTouching = true;
            backLIneRenderer.SetPosition(0, transform.position);
            frontLineRenderer.SetPosition(0, transform.position);
            exitButton.gameObject.SetActive(false);
            scoreBoard.SetActive(false);
        }
    }


    private void PrimaryTouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if(!isTouching) return;
        exitButton.gameObject.SetActive(true);
        scoreBoard.SetActive(true);
        backLIneRenderer.enabled = false;
        frontLineRenderer.enabled = false;
        isTouching = false;
        cameraZoomingIn = true;
        elapsedTimeForCameraToZoomIn = 0f;

        if((backLIneRenderer.GetPosition(1) - backLIneRenderer.GetPosition(0)).magnitude <= turnCancelOffset) return;
        else forceAddedToPlayer = (backLIneRenderer.GetPosition(1) - backLIneRenderer.GetPosition(0)).magnitude;
        AddForceToPlayer();
        turnPlayed = true;
        onPlayerReleaseTouch?.Invoke();
    }


    private void AddForceToPlayer() {
        Vector2 startPos = backLIneRenderer.GetPosition(0);
        Vector2 currentPos = backLIneRenderer.GetPosition(1);

        Vector2 distance = currentPos - startPos;
        Vector2 finalForce = force * distance; 
        playerRB.AddForce(-finalForce, ForceMode2D.Impulse);
    }


    private void Update() {
        if(isTouching) {
            Vector2 startPos = transform.position;
            Vector2 currentPos = PrimaryPosition();
            Vector2 distance = currentPos - startPos;
            if(distance.magnitude <= dragLimit) {
                backLIneRenderer.SetPosition(1, currentPos);

                Vector2 frontLineDistance = new Vector2(transform.position.x - distance.x, transform.position.y - distance.y);
                frontLineRenderer.SetPosition(1, frontLineDistance);

                mainCamera.orthographicSize = mainCameraSize + distance.magnitude;
            } else {
                Vector2 limitVector = startPos + (distance.normalized * dragLimit);
                backLIneRenderer.SetPosition(1, limitVector);

                Vector2 frontLineDistance = startPos - (distance.normalized * dragLimit);
                frontLineRenderer.SetPosition(1, frontLineDistance);

                mainCamera.orthographicSize = mainCameraSize + cameraZoomOut;
            }
        }

        
        if(cameraZoomingIn) {
            elapsedTimeForCameraToZoomIn += Time.deltaTime;
            float t = elapsedTimeForCameraToZoomIn/timeToCameraZoomIn;
            mainCamera.orthographicSize = Mathf.Lerp(mainCameraSize + forceAddedToPlayer, mainCameraSize, t);

            if(mainCamera.orthographicSize == mainCameraSize) {
                elapsedTimeForCameraToZoomIn = 0f;
                cameraZoomingIn = false;
            }
        }

        if(turnPlayed && playerRB.linearVelocity.magnitude <= velocityOffset) {
            turnPlayed = false;
            onPlayerSlowDown?.Invoke();
        }

    }

}