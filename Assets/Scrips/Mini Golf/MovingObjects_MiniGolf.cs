using UnityEngine;

public class MovingObjects_MiniGolf : MonoBehaviour {

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private float speed = .75f;

    private bool movingToB = false;
    private float t = 0;

    private void Update() {

        t += Time.deltaTime * speed * (movingToB ? 1 : -1);

        if(t >= 1) {
            t = 1;
            movingToB = false;
        } else if(t <= 0) {
            t = 0;
            movingToB = true;
        }

        transform.position = Vector3.Lerp(point1.position, point2.position, t);

    }


}