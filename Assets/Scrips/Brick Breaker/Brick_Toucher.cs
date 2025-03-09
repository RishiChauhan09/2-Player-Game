using UnityEngine;
using UnityEngine.EventSystems;

public class Brick_Toucher : MonoBehaviour, IDragHandler
{
    public enum PlayerSide
    {
        Blue,
        Red
    }

    [SerializeField] private PlayerSide playerSide;

    public delegate void PaddleTouchEvent(PlayerSide side, Vector2 screenPos);
    public static event PaddleTouchEvent onPaddleDrag;




    public void OnDrag(PointerEventData eventData)
    {
        if (IsValidSide(eventData.position))
        {
            onPaddleDrag?.Invoke(playerSide, eventData.position);
        }
    }


    private bool IsValidSide(Vector2 screenPos)
    {
        if (playerSide == PlayerSide.Blue)
            return (screenPos.y < Screen.height / 2f);
        else //Red one keliye
            return (screenPos.y >= Screen.height / 2f);
    }
}
