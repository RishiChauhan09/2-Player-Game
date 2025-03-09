using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamesButton_MainMenu : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private bool willChangeScene = true;
    [SerializeField] private string gameSceneName;
    [SerializeField] private bool isLandScape = false;
    [SerializeField] private float scale = 1.05f;
    [SerializeField] private bool shouldResize = false;

    private void Start() {
        if(willChangeScene) {
            gameObject.GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.instance.PlaySound("UIClick");
                Manager_MainMenu.LoadScene(gameSceneName);
                if(isLandScape) Utils.ChangeToLandScape();
            });
        } else {
            gameObject.GetComponent<Button>().onClick.AddListener(() => {
                AudioManager.instance.PlaySound("UIClick");
            });
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.instance.PlaySound("UIClick");
        transform.DOScale(scale, 0.075f).SetEase(Ease.InQuad);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(shouldResize) {
            transform.DOScale(1f, 0.075f);
        }
    }
}