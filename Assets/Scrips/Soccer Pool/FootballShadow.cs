using UnityEngine;

public class FootballShadow : MonoBehaviour {

    [SerializeField] private GameObject football;
    [SerializeField] private Vector3 offSet;

    private void Start() {
        transform.position = football.transform.position + offSet;
    }

    private void Update() {
        transform.position = football.transform.position + offSet;
    }

}