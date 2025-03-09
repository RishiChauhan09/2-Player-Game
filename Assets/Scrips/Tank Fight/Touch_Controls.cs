using UnityEngine;
using UnityEngine.EventSystems;

public class Touch_Controls : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CannonController.Player player;
    private CannonController controller;

    public delegate void OnPlayerTouch(CannonController.Player side);
    public static event OnPlayerTouch onPlayerTouchScreen;

    private void Start()
    {
        FindMatchingController();
    }

    private void FindMatchingController()
    {
        CannonController[] controllers = FindObjectsOfType<CannonController>();
        foreach (CannonController ctrl in controllers)
        {
            if (ctrl.player == player)
            {
                controller = ctrl;
                break;
            }
        }

        if (controller == null)
        {
            Debug.LogError($"No CannonController found for {player} player!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Touch detected for {player}");

        if (controller == null)
        {
            FindMatchingController();
            Debug.Log("Searching for controller again...");
        }

        if (controller != null)
        {
            onPlayerTouchScreen?.Invoke(player);
            Debug.Log($"Invoking event for {player}");
        }
        else
        {
            Debug.LogError($"No CannonController found for {player} player to shoot!");
        }
    }
}