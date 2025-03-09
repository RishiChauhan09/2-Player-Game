using UnityEngine;

public class Racket_Tennis : MonoBehaviour {

    [SerializeField] private Vector3 powerShotOffset;
    [SerializeField] private float powerShotThreashHold = .2f;

    public bool IsPowerShot(Collision2D collision) {
        Vector3 powerShotPoint = transform.position + powerShotOffset;

        Vector2 collisionPoint = collision.GetContact(0).point;

        if(Mathf.Abs(collisionPoint.y - powerShotPoint.y) < powerShotThreashHold) return true;
        else return false;
    }

    // getting power shot area 
    // checking for just height
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + powerShotOffset, powerShotThreashHold);
    }

}