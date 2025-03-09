using System.Threading.Tasks;
using Terresquall;
using UnityEngine;

public class PlayerMovements_Tanks : MonoBehaviour {

    public enum PlayerSide {
        Blue,
        Red
    }

    private const int RED_LEFT_JOYSTICK = 1;
    private const int RED_RIGHT_JOYSTICK = 2;
    private const int BLUE_LEFT_JOYSTICK = 3;
    private const int BLUE_RIGHT_JOYSTICK = 4;

    private const string SHOOTBULLET = "ShootBullet";
    

    [SerializeField] public PlayerSide playerSide;

    [Header("Tank Parts")]
    [SerializeField] private GameObject tankLowerPart;
    [SerializeField] private GameObject tankUpperPart;
    [SerializeField] private GameObject bullerPrefab;
    [SerializeField] private Transform positionToInstantiate;
    [SerializeField] private Animator tankAnimator;
    [SerializeField] private ParticleSystem smoke;

    [Space(2)]
    [Header("Values")]
    [SerializeField] private float upperRotationOffset = -90f;
    [SerializeField] private float rotationOffSet = -90f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float cooldownToShootBullet = .5f;

    private float cooldownShootBulletCounter = 0f;

    private void Awake() {
        Bullet_Tanks.onBulletHitPlayer += OnBulletHitPlayer;
    }


    private void Start() {
        smoke.gameObject.SetActive(false);
    }

    private void Update() {
        HandleMovement();
        HandleUpperTank();
    }

    private async void OnBulletHitPlayer(PlayerSide side) {
        await Task.Delay(500);
        if(playerSide == PlayerSide.Blue && GameManager_Tanks.bluePoints == 1) {
            smoke.gameObject.SetActive(true);
            smoke.Play();
        } else if(playerSide == PlayerSide.Red && GameManager_Tanks.redPoints == 1) {
            smoke.gameObject.SetActive(true);
            smoke.Play();
        }
    }

    private void HandleMovement() {

        Vector2 inputLeft;
        if(playerSide == PlayerSide.Blue) 
            inputLeft = VirtualJoystick.GetAxis(BLUE_LEFT_JOYSTICK);
        else 
            inputLeft = VirtualJoystick.GetAxis(RED_LEFT_JOYSTICK);

        inputLeft.Normalize();
        transform.position += new Vector3(inputLeft.x, inputLeft.y, 0f) * moveSpeed * Time.deltaTime;

        if(inputLeft == Vector2.zero) {
            if(playerSide == PlayerSide.Blue) GameManager_Tanks.blueMoving = false;
            if(playerSide == PlayerSide.Red) GameManager_Tanks.redMoving = false;

            if(!GameManager_Tanks.blueMoving && !GameManager_Tanks.redMoving)
                AudioManager.instance.StopSound("TankMoving");
            return; 
        }

        if(playerSide == PlayerSide.Blue) GameManager_Tanks.blueMoving = true;
        if(playerSide == PlayerSide.Red) GameManager_Tanks.redMoving = true;

        float targetAngle = Mathf.Atan2(inputLeft.y, inputLeft.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(tankLowerPart.transform.eulerAngles.z, targetAngle + rotationOffSet, Time.deltaTime * rotationSpeed);
        AudioManager.instance.PlaySound("TankMoving");
        tankLowerPart.transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void HandleUpperTank() {

        Vector2 inputRight;
        if(playerSide == PlayerSide.Blue) 
            inputRight = VirtualJoystick.GetAxis(BLUE_RIGHT_JOYSTICK);
        else 
            inputRight = VirtualJoystick.GetAxisRaw(RED_RIGHT_JOYSTICK);
        

        if(cooldownShootBulletCounter > 0) {
            cooldownShootBulletCounter -= Time.deltaTime;
            if(cooldownShootBulletCounter < 0) 
                cooldownShootBulletCounter = 0;
        }

        if(inputRight == Vector2.zero) return;

        float targetAngle = Mathf.Atan2(inputRight.y, inputRight.x) * Mathf.Rad2Deg;
        float angle = Mathf.MoveTowardsAngle(tankUpperPart.transform.eulerAngles.z, targetAngle + upperRotationOffset, Time.deltaTime * rotationSpeed);
        tankUpperPart.transform.rotation = Quaternion.Euler(0, 0, angle);


        if(cooldownShootBulletCounter > 0) return;
        else {
            cooldownShootBulletCounter = cooldownToShootBullet;

            tankAnimator.SetTrigger(SHOOTBULLET);
            GameObject bullet = Instantiate(bullerPrefab, positionToInstantiate.position, Quaternion.identity);
            AudioManager.instance.PlaySound("Fire");
            bullet.transform.up = tankUpperPart.transform.up;
        }

    }



}