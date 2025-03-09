using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Buttons_MemroyGame : MonoBehaviour {

    [SerializeField] private int numberOfButton;
    [SerializeField] private GameManager_MemoryGame.PlayerSide playerSide;

    public delegate void OnButtonClicked(GameManager_MemoryGame.PlayerSide side, int buttonNo);
    public static event OnButtonClicked onButtonClicked;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void Start() {
        button.onClick.AddListener(() => {
            gameObject.transform.DOScale(new Vector3(2.8f, 2.8f, 1f), 0.2f).OnComplete(() => {
                gameObject.transform.DOScale(Vector3.one * 3, 0.2f);
            });
            onButtonClicked?.Invoke(playerSide, numberOfButton);
        });
    }


}