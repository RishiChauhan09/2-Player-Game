using UnityEngine;

public class ObstacleRotation_MiniGolf : MonoBehaviour {

    [SerializeField] private float speedOfRotation = 75f;

    private Vector3 angleToRotate = new Vector3(0, 0, 1);

    private void Update() {
        transform.Rotate(angleToRotate * speedOfRotation * Time.deltaTime);
    }
}