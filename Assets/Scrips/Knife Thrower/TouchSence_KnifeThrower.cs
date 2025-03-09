using UnityEngine;
using UnityEngine.EventSystems;

public class TouchSence_KnifeThrower : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Player_Knife.PlayerSide playerSide;

    public delegate void OnPlayerTouch(Player_Knife.PlayerSide side);
    public static event OnPlayerTouch onPlayerTouchScreen;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPlayerTouchScreen?.Invoke(playerSide);
    }

}