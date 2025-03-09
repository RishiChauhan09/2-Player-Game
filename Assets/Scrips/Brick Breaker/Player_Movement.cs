using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    public enum PlayerSide { Blue, Red }

    [Header("Player Settings")]
    public PlayerSide playerSide;
    public float speed = 5f;
    public float maxX = 1.72f;

    private float targetX;

    private void OnEnable()
    {
        Brick_Toucher.onPaddleDrag += HandlePaddleDrag;
    }

    private void OnDisable()
    {
        Brick_Toucher.onPaddleDrag -= HandlePaddleDrag;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x - targetX) > 0.01f)
        {
            float newX = Mathf.MoveTowards(transform.position.x,targetX,speed * Time.deltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    private void HandlePaddleDrag(Brick_Toucher.PlayerSide side, Vector2 screenPos)
    {
        if (!IsOurSide(side)) return;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        float clampedX = Mathf.Clamp(worldPos.x, -maxX, maxX);
        targetX = clampedX;
    }

    private bool IsOurSide(Brick_Toucher.PlayerSide side)
    {
        return (side == (Brick_Toucher.PlayerSide)playerSide);
    }
}
