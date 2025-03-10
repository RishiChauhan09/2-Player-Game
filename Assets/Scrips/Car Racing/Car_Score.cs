using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Splines.ExtrusionShapes;

public class Car_Score : MonoBehaviour
{
    [Header("Score Settings")]
    public int blueCarScore = 0;
    public int redCarScore = 0;
    private float cooldownTime = 2.2f;

    [Header("UI References")]
    public Text blueScoreText;
    public Text redScoreText;
    public GameObject BlueWinnerScreen;
    public GameObject RedWinnerScreen;

    private bool blueCooldown = false;
    private bool redCooldown = false;


    void Start()
    {
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (blueScoreText != null)
            blueScoreText.text = blueCarScore.ToString();
        if (redScoreText != null)
            redScoreText.text = redCarScore.ToString();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Car_Script car = other.GetComponent<Car_Script>();
        if (car != null)
        {
            if (car.playerSide == Car_Script.PlayerSide.Blue && !blueCooldown && blueCarScore < 7)
            {
                blueCarScore++;
                StartCoroutine(BlueCooldown());
            }
            else if (car.playerSide == Car_Script.PlayerSide.Red && !redCooldown && redCarScore < 7)
            {
                redCarScore++;
                StartCoroutine(RedCooldown());
            }
            UpdateScoreUI();
            Winner();
        }
    }

    IEnumerator BlueCooldown()
    {
        blueCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        blueCooldown = false;
    }

    IEnumerator RedCooldown()
    {
        redCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        redCooldown = false;
    }

    void Winner()
    {
        if (blueCarScore >= 5)
        {
            BlueWinnerScreen.SetActive(true);
            Manager_MainMenu.blueMainPoints++;
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }
        else if (redCarScore >= 5)
        {
            RedWinnerScreen.SetActive(true);
            Manager_MainMenu.redMainPoints++;
            StartCoroutine(Utils.WaitAndLoadScene("Main Menu"));
        }

        
    }
}