using UnityEngine;

public class Metal_BrickBreaker : MonoBehaviour {


    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Ball") {
            AudioManager.instance.PlaySound("HitMetal");
        }
    }

}