using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Script : MonoBehaviour
{
    public enum PlayerSide
    {
        Blue,
        Red
    }

    [Header("Player Setup")]
    public PlayerSide playerSide;
    public Transform bluePlayer;
    public Transform redPlayer;

    [Header("Joystick Setup")]
    public Transform circle;
    public Transform outerCircle;
    public Transform fixedJoystickPositionBlue;
    public Transform fixedJoystickPositionRed;
    public float speed = 5.0f;

    private bool touchStart = false;
    private Vector2 pointA;
    private Vector2 pointB;
    private int activeTouchId = -1;
    private bool isMoving = false;

    [SerializeField] private GameObject CountDownBG;
    [SerializeField] private TMP_Text CountDownText;

    void Start()
    {
        if (playerSide == PlayerSide.Blue)
        {
            circle.position = fixedJoystickPositionBlue.position;
            outerCircle.position = fixedJoystickPositionBlue.position;
        }
        else
        {
            circle.position = fixedJoystickPositionRed.position;
            outerCircle.position = fixedJoystickPositionRed.position;
        }

        StartCoroutine(Countdown());
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        foreach (var touch in Touchscreen.current.touches)
        {
            var phase = touch.phase.ReadValue();
            int currentTouchId = touch.touchId.ReadValue();

            if (activeTouchId == -1 && phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Vector2 screenPos = touch.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z))
                );

                Vector3 joystickPos = (playerSide == PlayerSide.Blue)
                    ? fixedJoystickPositionBlue.position
                    : fixedJoystickPositionRed.position;

                float radius = outerCircle.GetComponent<CircleCollider2D>().radius;
                if (Vector2.Distance(worldPos, joystickPos) < radius)
                {
                    activeTouchId = currentTouchId;
                    touchStart = true;
                    pointA = worldPos;

                    circle.GetComponent<SpriteRenderer>().enabled = true;
                    outerCircle.GetComponent<SpriteRenderer>().enabled = true;
                }
            }

            if (activeTouchId == currentTouchId)
            {
                if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    Vector2 screenPos = touch.position.ReadValue();
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                        new Vector3(screenPos.x, screenPos.y, Mathf.Abs(Camera.main.transform.position.z))
                    );
                    pointB = worldPos;
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                         phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    activeTouchId = -1;
                    touchStart = false;
                }
            }
        }
    }

    private IEnumerator Countdown()
    {

        CountDownBG.SetActive(true);
        CountDownText.gameObject.SetActive(true);

        AudioManager.instance.PlaySound("Game Start");
        for (int i = 3; i > 0; i--)
        {
            CountDownText.text = i.ToString();

            CountDownText.transform.localScale = Vector3.one * 1.2f;
            CountDownText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);

            yield return new WaitForSeconds(.75f);
        }
        CountDownText.gameObject.SetActive(false);
        CountDownBG.SetActive(false);

        foreach (var touch in Touchscreen.current.touches) circle.GetComponent<SpriteRenderer>().enabled = false;
        foreach (var touch in Touchscreen.current.touches) outerCircle.GetComponent<SpriteRenderer>().enabled = false;
    }

    void FixedUpdate()
    {
        if (touchStart)
        {
            Vector2 offset = pointB - pointA;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

            MoveCharacter(direction);
            circle.position = pointA + direction;
        }
        else
        {
            circle.GetComponent<SpriteRenderer>().enabled = false;
            outerCircle.GetComponent<SpriteRenderer>().enabled = false;

            if (isMoving)
            {
                AudioManager.instance.StopSound("Car Racer");
                isMoving = false;
            }
        }
    }

    void MoveCharacter(Vector2 direction)
    {
        if (direction.sqrMagnitude > 1)
            direction.Normalize();

        Transform currentPlayer = (playerSide == PlayerSide.Blue) ? bluePlayer : redPlayer;

        if (direction.sqrMagnitude > 0.01f)
        {
            if (!isMoving)
            {
                AudioManager.instance.PlaySound("Car Racer");
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                AudioManager.instance.StopSound("Car Racer");
                isMoving = false;
            }
        }

        currentPlayer.Translate(direction * speed * Time.deltaTime, Space.World);

        if (direction.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = -90f;
            currentPlayer.rotation = Quaternion.Euler(0, 0, angle + offsetAngle);
        }
    }
}
