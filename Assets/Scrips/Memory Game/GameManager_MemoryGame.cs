using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager_MemoryGame : MonoBehaviour {

    public enum PlayerSide {
        Blue,
        Red,
        NoSide,
        Both
    }

    [SerializeField] private GameObject countdownBG;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int totalPointsReq = 3;

    [Space(2)]
    [Header("Values")]
    [SerializeField] private float timeToShowOneButton = .5f;
    [SerializeField] private int startNumber = 3;


    [Space(2)]
    [Header("Buttons")]
    [SerializeField] private Button[] blueButtons;
    [SerializeField] private Button[] redButtons;
    [SerializeField] private Sprite resetSprite;
    [Space(1)]
    [SerializeField] private Sprite redNomralButtonSprite;
    [SerializeField] private Sprite blueNormalButtonSprite;
    [Space(1)]
    [SerializeField] private Sprite redShowSeqSprite;
    [SerializeField] private Sprite blueShowSeqSprite;

    [Space(2)]
    [Header("Screens")]
    [SerializeField] private GameObject redWinnerScreen;
    [SerializeField] private GameObject blueWinnerScreen;
    [SerializeField] private TMP_Text blueScoreText;
    [SerializeField] private TMP_Text redScoreText;
    

    private int bluePoints;
    private int redPoints;

    private int seqNum;
    private int blueClicks = 0;
    private int redClicks = 0;


    private List<int> mainSeq = new List<int>();

    private void Awake() {
        Buttons_MemroyGame.onButtonClicked += OnButtonClicked;
    }


    private void Start() {
        seqNum = startNumber;

        redWinnerScreen.SetActive(false);
        blueWinnerScreen.SetActive(false);

        bluePoints = 0;
        redPoints = 0;

        StartCoroutine(NewWave(0));
    }



    private IEnumerator Countdown() {
        countdownBG.SetActive(true);
        countdownText.gameObject.SetActive(true);
        canvas.sortingOrder = 5;

        AudioManager.instance.PlaySound("Countdown");
        for(int i = 3; i > 0; i--) {
            countdownText.text = i.ToString();

            countdownText.transform.localScale = Vector3.one * 1.2f;
            countdownText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);

            yield return new WaitForSeconds(.75f);
        }
        countdownText.gameObject.SetActive(false);
        countdownBG.SetActive(false);

        canvas.sortingOrder = 0;
    }


    IEnumerator DisplaySequence() { 
        for(int i = 0; i < seqNum; i++) {
            AudioManager.instance.PlaySound("ShowSeq");
            blueButtons[mainSeq[i]].image.sprite = blueShowSeqSprite;
            redButtons[mainSeq[i]].image.sprite = redShowSeqSprite;
            yield return new WaitForSeconds(timeToShowOneButton);

            blueButtons[mainSeq[i]].image.sprite = blueNormalButtonSprite;
            redButtons[mainSeq[i]].image.sprite = redNomralButtonSprite;
            yield return new WaitForSeconds(timeToShowOneButton);
        }

        
    }

    private IEnumerator NewWave(int lSeqNum) {
        seqNum += lSeqNum;

        blueClicks = 0;
        redClicks = 0;

        foreach(Button btn in blueButtons) {
            btn.interactable = false;
        }
        foreach(Button button in redButtons) {
            button.interactable = false;
        }

        yield return StartCoroutine(Countdown());
        yield return StartCoroutine(ResetSeqSprite());

        // generating random sequence
        for(int i = 0; i < seqNum; i++) {
            int randomValue = Random.Range(0, 9);

            if(i < mainSeq.Count) {
                mainSeq[i] = randomValue;
            } else {
                mainSeq.Add(randomValue);
            }

        }

        yield return StartCoroutine(DisplaySequence()); // displaying all sequence 

        foreach(Button btn in blueButtons) {
            btn.interactable = true;
        }
        foreach(Button button in redButtons) {
            button.interactable = true;
        }

    }

    

    private async void OnButtonClicked(PlayerSide side, int buttonNo) {
        if(side == PlayerSide.Red) {
            if(mainSeq[redClicks] != buttonNo) {
                AudioManager.instance.PlaySound("WrongSeq");
                redClicks = -1;
                StartCoroutine(ResetSeqSprite(PlayerSide.Red));

                if(blueClicks == -1) {
                    await Task.Delay(3300); // waiting for reset sequence to finish
                    StartCoroutine(NewWave(0));
                }

            } else if(mainSeq[redClicks]  == buttonNo){
                redClicks++;
            }


            if(seqNum == redClicks) {

                redPoints++;
                AudioManager.instance.PlaySound("Point");
                redScoreText.text = redPoints.ToString();
                if(redPoints == totalPointsReq) {
                    redWinnerScreen.SetActive(true);
                    Manager_MainMenu.redMainPoints++;
                    StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                    enabled = false;
                    return;
                }
                StartCoroutine(NewWave(1));
            }


        } else {
            if(mainSeq[blueClicks] != buttonNo) {
                AudioManager.instance.PlaySound("WrongSeq");
                blueClicks = -1;
                StartCoroutine(ResetSeqSprite(PlayerSide.Blue));

                if(redClicks == -1) {
                    await Task.Delay(3300); // waiting for reset sequence to finish
                    StartCoroutine(NewWave(0));
                }

            } else if(mainSeq[blueClicks] == buttonNo) {
                blueClicks++;
            }


            if(seqNum == blueClicks) {

                AudioManager.instance.PlaySound("Point");
                bluePoints++;
                blueScoreText.text = bluePoints.ToString();
                if(bluePoints == totalPointsReq) {
                    blueWinnerScreen.SetActive(true);
                    Manager_MainMenu.blueMainPoints++;
                    StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
                    enabled = false;
                    return;
                }
                StartCoroutine(NewWave(1));
            }
        }
    }


    private IEnumerator ResetSeqSprite(PlayerSide side = PlayerSide.NoSide) {

        if(side == PlayerSide.Red) {
            for(int i = 0; i < 3; i++) {
                foreach(Button btn in redButtons) {
                    btn.interactable = false;
                    btn.gameObject.GetComponent<Image>().sprite = resetSprite;
                }

                yield return new WaitForSeconds(.75f);

                foreach(Button btn in redButtons) {
                    btn.gameObject.GetComponent<Image>().sprite = redNomralButtonSprite;
                }
                yield return new WaitForSeconds(.35f);

            }


        } else if(side == PlayerSide.Blue) {
            for(int i = 0; i < 3; i++) {
                foreach(Button btn in blueButtons) {
                    btn.interactable = false;
                    btn.gameObject.GetComponent<Image>().sprite = resetSprite;
                }

                yield return new WaitForSeconds(.75f);

                foreach(Button btn in blueButtons) {
                    btn.gameObject.GetComponent<Image>().sprite = blueNormalButtonSprite;
                }
                yield return new WaitForSeconds(.35f);

            }


        } else if(side == PlayerSide.Both) {
            for(int i = 0; i < 3; i++) {
                for(int j = 0; j < 9; j++) {
                    blueButtons[j].interactable = false;
                    redButtons[j].interactable = false;

                    blueButtons[j].gameObject.GetComponent<Image>().sprite = resetSprite;
                    redButtons[j].gameObject.GetComponent<Image>().sprite = resetSprite;
                }

                yield return new WaitForSeconds(.75f);

                for(int j = 0; j < 9; j++) {
                    blueButtons[j].gameObject.GetComponent<Image>().sprite = blueNormalButtonSprite;
                    redButtons[j].gameObject.GetComponent<Image>().sprite = redNomralButtonSprite;
                }

                yield return new WaitForSeconds(.35f);

            }
        }
    }


}