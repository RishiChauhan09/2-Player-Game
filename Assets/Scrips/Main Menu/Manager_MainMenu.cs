using DG.Tweening;
using System.IO.IsolatedStorage;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager_MainMenu : MonoBehaviour {

    public static int redMainPoints = 0;
    public static int blueMainPoints = 0;

    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private TMP_Text bluePointsText;
    [SerializeField] private TMP_Text redPointsText;

    [Space(2)]
    [Header("Middle Panel")]
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private float timeToHideScroll = .5f;
    [SerializeField] private float timeToMoveScroll = .5f;
    [SerializeField] private float leftScrollValue = 650;
    [SerializeField] private float rightScrollValue = 750;
    private float timeOfScrollValueChanged;
    private bool isScrollling;

    [Space(2)]
    [Header("Panels things")]
    [SerializeField] private float yPosHide = -3100f;
    [SerializeField] private float timeForPanels = .25f;
    [SerializeField] private GameObject alphaPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private Ease panelsCurve;

    private void Start() {
        // just in case if there is any error then it will handle it of not showing main menu screen
        mainMenuScreen.SetActive(true); 
        Utils.ChangeToPortrait();
        bluePointsText.text = blueMainPoints.ToString();
        redPointsText.text = redMainPoints.ToString();

        scroll.onValueChanged.AddListener(value => {
            scroll.GetComponent<RectTransform>().DOAnchorPosX(leftScrollValue, timeToMoveScroll);
            timeOfScrollValueChanged = Time.time;
            isScrollling = true;
        });
    }

    private void Update() {
        if(isScrollling) {
            if(Time.time - timeOfScrollValueChanged > timeToHideScroll) {
                scroll.GetComponent<RectTransform>().DOAnchorPosX(rightScrollValue, timeToMoveScroll);
            }
        }
    }

    public static void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void PanelButton(string panel) {
        alphaPanel.SetActive(true);
        if(panel == "Exit") {
            exitPanel.GetComponent<RectTransform>().DOAnchorPosY(0, timeForPanels).SetEase(panelsCurve);
        } else if(panel == "Settings") {
            settingsPanel.GetComponent<RectTransform>().DOAnchorPosY(0, timeForPanels).SetEase(panelsCurve);
        }
    }

    public void BackToMainMenu(string panel) {
        if(panel == "Exit") {
            exitPanel.GetComponent<RectTransform>().DOAnchorPosY(yPosHide, timeForPanels).SetEase(panelsCurve);
        } else if(panel == "Settings") {
            settingsPanel.GetComponent<RectTransform>().DOAnchorPosY(yPosHide, timeForPanels).SetEase(panelsCurve);
        }
        alphaPanel.SetActive(false);
    }

    public void QuitApplication() {
        Application.Quit();
    }

    public void ResetScores() {
        blueMainPoints = 0;
        redMainPoints = 0;
        bluePointsText.text = blueMainPoints.ToString();
        redPointsText.text = redMainPoints.ToString();
    }
}