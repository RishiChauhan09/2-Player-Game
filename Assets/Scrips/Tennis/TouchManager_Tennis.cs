using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager_Tennis : MonoBehaviour, IPointerDownHandler {

    [SerializeField] private PlayerMovements_Tennis.PlayerSide playerSide;

    public delegate void OnPlayerPress(PlayerMovements_Tennis.PlayerSide side);
    public static event OnPlayerPress onPlayerPress;

    public void OnPointerDown(PointerEventData eventData) {
        onPlayerPress?.Invoke(playerSide);
    }

}