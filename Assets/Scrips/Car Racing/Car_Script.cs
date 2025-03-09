using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Script : MonoBehaviour
{
    public enum PlayerSide
    {
        Blue,
        Red
    }

    public PlayerSide playerSide;

    public Transform player;
    public float speed = 5.0f;
    private bool touchStart = false;
    private Vector2 pointA;
    private Vector2 pointB;

    public Transform circle;
    public Transform outerCircle;
    public Transform fixedJoystickPositionBlue;
    public Transform fixedJoystickPositionRed;

    public Transform bluePlayer;
    public Transform redPlayer;

    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(GameStart());

        if (playerSide == PlayerSide.Blue)
        {
            circle.transform.position = fixedJoystickPositionBlue.position;
            outerCircle.transform.position = fixedJoystickPositionBlue.position;
        }
        else if (playerSide == PlayerSide.Red)
        {
            circle.transform.position = fixedJoystickPositionRed.position;
            outerCircle.transform.position = fixedJoystickPositionRed.position;
        }
    }

    void Update()
    {
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(
                    new Vector3(touchPos.x, touchPos.y, Mathf.Abs(Camera.main.transform.position.z))
                );

                if (playerSide == PlayerSide.Blue &&
                    Vector2.Distance(touchPosition, fixedJoystickPositionBlue.position) < outerCircle.GetComponent<CircleCollider2D>().radius)
                {
                    pointA = touchPosition;
                    touchStart = true;
                    circle.GetComponent<SpriteRenderer>().enabled = true;
                    outerCircle.GetComponent<SpriteRenderer>().enabled = true;
                }
                else if (playerSide == PlayerSide.Red &&
                    Vector2.Distance(touchPosition, fixedJoystickPositionRed.position) < outerCircle.GetComponent<CircleCollider2D>().radius)
                {
                    pointA = touchPosition;
                    touchStart = true;
                    circle.GetComponent<SpriteRenderer>().enabled = true;
                    outerCircle.GetComponent<SpriteRenderer>().enabled = true;
                }
            }

            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                if (touchStart)
                {
                    Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(
                        new Vector3(touchPos.x, touchPos.y, Mathf.Abs(Camera.main.transform.position.z))
                    );
                    pointB = touchPosition;
                }
            }
            else
            {
                touchStart = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (touchStart)
        {
            Vector2 offset = pointB - pointA;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

            MoveCharacter(direction);

            circle.transform.position = new Vector2(pointA.x + direction.x, pointA.y + direction.y);
        }
        else
        {
            circle.GetComponent<SpriteRenderer>().enabled = false;
            outerCircle.GetComponent<SpriteRenderer>().enabled = false;

            if (isMoving)
            {
                if (playerSide == PlayerSide.Blue)
                    AudioManager.instance.StopSound("Car Racer");
                else if (playerSide == PlayerSide.Red)
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
                if (playerSide == PlayerSide.Blue)
                    AudioManager.instance.PlaySound("Car Racer");
                else if (playerSide == PlayerSide.Red)
                    AudioManager.instance.PlaySound("Car Racer");
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                if (playerSide == PlayerSide.Blue)
                    AudioManager.instance.StopSound("Car Racer");
                else if (playerSide == PlayerSide.Red)
                    AudioManager.instance.StopSound("Car Racer");
                isMoving = false;
            }
        }

        currentPlayer.Translate(direction * speed * Time.deltaTime, Space.World);

        if (direction.sqrMagnitude > 0)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offsetAngle = -90f;
            currentPlayer.rotation = Quaternion.Euler(0, 0, targetAngle + offsetAngle);
        }
    }
    IEnumerator GameStart()
    {
        AudioManager.instance.PlaySound("Game Start");
        yield return new WaitForSecondsRealtime(3);
    }
}
