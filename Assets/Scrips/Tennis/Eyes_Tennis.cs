using UnityEngine;

public class Eyes_Tennis : MonoBehaviour {

    [SerializeField] private GameObject centerObject;
    [HideInInspector] public static GameObject ball;
    [SerializeField] private GameObject owner;
    [SerializeField] private float radius = 0.25f;
    private Vector2 eyeCenter;
    private float ownerStartPosY;


    private void Start() {
        if(ball == null) ball = centerObject;
        ownerStartPosY = owner.transform.position.y;
        eyeCenter = transform.position;
    }

    private void Update() {
        eyeCenter.y = ownerStartPosY + (owner.transform.position.y - ownerStartPosY);
        Vector2 direction = (ball.transform.position - transform.position).normalized;
        Vector2 newPos = eyeCenter + (direction * radius);

        transform.position = Vector2.Lerp(transform.position, newPos, .2f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}