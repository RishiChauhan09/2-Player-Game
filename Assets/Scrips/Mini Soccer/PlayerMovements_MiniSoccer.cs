using TMPro;
using UnityEngine;

public class PlayerMovements_MiniSoccer : MonoBehaviour {

    private const string ISRUNNING = "IsRunning";


    public enum PlayerPos { 
        Keeper, 
        Forward
    }

    public enum PlayerSide {
        Red,
        Blue
    }

    [SerializeField] private PlayerSide playerSide;
    [SerializeField] private PlayerPos position;

    [Header("Player Things")]
    [SerializeField] private Animator playerAnimator;

    [Space(2)]
    [Header("Values")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 400f;
    [SerializeField] private float rotationOffset = -90f;

    [Header("End Points")]
    [SerializeField] private float keeperYMin = 4.0f;
    [SerializeField] private float keeperYMax = 4.5f;
    [SerializeField] private float keeperXMax = .96f;
    [SerializeField] private float keeperXMin = -.96f;

    private bool isRunning = false;
    private PlayerInputActions playerInputActions;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.MiniSoccer.Enable();
    }

    private void Update() {
        CheckIfCanMove();
    }

    private void FixedUpdate() {
        HandleMovements();
    }


    private void HandleMovements() {
        Vector2 playerInput;
        if(playerSide == PlayerSide.Red) playerInput = -playerInputActions.MiniSoccer.RedMovements.ReadValue<Vector2>();
        else playerInput = playerInputActions.MiniSoccer.BlueMovements.ReadValue<Vector2>();

        playerInput.Normalize();
        rb.linearVelocity = playerInput * movementSpeed;

        if(playerInput == Vector2.zero) isRunning = false;
        else isRunning = true;

        playerAnimator.SetBool(ISRUNNING, isRunning);

        if(playerInput == Vector2.zero) return;

        float targetAngle = Mathf.Atan2(playerInput.y, playerInput.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle + rotationOffset, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }


    private void CheckIfCanMove() { 
        if(position == PlayerPos.Keeper && playerSide == PlayerSide.Red) {
            if(transform.position.x < keeperXMin) {
                transform.position = new Vector3(keeperXMin, transform.position.y, 0f);
            } else if(transform.position.x > keeperXMax) {
                transform.position = new Vector3(keeperXMax, transform.position.y, 0f);
            }

            if(transform.position.y < keeperYMin) {
                transform.position = new Vector3(transform.position.x, keeperYMin+.05f, 0f);
            } else if(transform.position.y > keeperYMax) {
                transform.position = new Vector3(transform.position.x, keeperXMax-.05f, 0f);
            }
        } else if(position == PlayerPos.Keeper && playerSide == PlayerSide.Blue) {
            if(transform.position.x < keeperXMin) {
                transform.position = new Vector3(keeperXMin, transform.position.y, 0f);
            } else if(transform.position.x > keeperXMax) {
                transform.position = new Vector3(keeperXMax, transform.position.y, 0f);
            }

            if(transform.position.y < -keeperYMax) {
                transform.position = new Vector3(transform.position.x, -keeperYMax+.05f, 0f);
            } else if(transform.position.y > -keeperYMin) {
                transform.position = new Vector3(transform.position.x, -keeperYMin-.05f, 0f);
            }
        }
    }

}