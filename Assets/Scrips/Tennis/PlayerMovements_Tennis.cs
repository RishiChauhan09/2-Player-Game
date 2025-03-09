using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerMovements_Tennis : MonoBehaviour {


    public enum PlayerSide {
        Red, 
        Blue
    }
    [Header("Player things")]
    [SerializeField] private PlayerSide playerSide;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Rigidbody2D racketRB;
    [SerializeField] private GameObject racket;

    [Space(2)]
    [Header("Values")]
    [SerializeField] private float playerForce = 5f;
    [SerializeField] private float additionalRacketForce = .05f;
    [SerializeField] private float rotationAngle = 7.5f;
    [SerializeField] private float racketDuration;

    [Space(2)]
    [Header("Other")]
    [SerializeField] private Ease racketGoingBackEase;
    [SerializeField] private Ease racketGoingFrontEase;


    [SerializeField] private ParticleSystem jumpParticle;


    private void Awake() {
        TouchManager_Tennis.onPlayerPress += PlayerTouchedScreen;
        Ball_Tennis.onBallTouchRacket += OnBallTouchRacket;
    }

    private void OnDisable() {
        TouchManager_Tennis.onPlayerPress -= PlayerTouchedScreen;
        Ball_Tennis.onBallTouchRacket -= OnBallTouchRacket;

    }

    private void OnBallTouchRacket(PlayerSide side) {
        if(playerSide != side) return;

        if(side == PlayerSide.Red) {
            racket.transform.DORotate(new Vector3(0, 0, rotationAngle), racketDuration).SetEase(racketGoingBackEase)
                .OnComplete(() => {
                    racket.transform.DORotate(Vector3.zero, racketDuration).SetEase(racketGoingFrontEase);
                });
        } else {
            racket.transform.DORotate(new Vector3(0, 0, -rotationAngle), racketDuration).SetEase(racketGoingBackEase)
                .OnComplete(() => {
                    racket.transform.DORotate(Vector3.zero, racketDuration).SetEase(racketGoingFrontEase);
                });
        }
    }

    private void PlayerTouchedScreen(PlayerSide side) {
        if(playerSide != side) return;

        jumpParticle.Play();

        playerRB.linearVelocity = Vector3.zero;
        racketRB.linearVelocity = Vector3.zero;

        playerRB.AddForce(Vector2.up * playerForce, ForceMode2D.Impulse);
        racketRB.AddForce(Vector2.up * (playerForce + additionalRacketForce), ForceMode2D.Impulse);
    }

}