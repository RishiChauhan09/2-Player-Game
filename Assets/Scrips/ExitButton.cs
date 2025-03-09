using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {

    [SerializeField] private float holdTime = 1f;
    [SerializeField] private GameObject buttonBackground;

    private float counter;
    private bool isPressing;

    private void Awake() {
        isPressing = false;
        counter = 0;
    }


    private void Update() {
        if(isPressing) {
            counter += Time.deltaTime * holdTime;
            if(counter >= .7f) { // .5 because 180 he turn karna hai 
                ExitButtonTaskPerform();
                counter = .7f;
            }
        }

        if(!isPressing && counter > 0) {
            counter -= Time.deltaTime;
            if(counter < 0) {
                counter = 0;
            }
        }

        
        buttonBackground.GetComponent<Image>().fillAmount = counter; 
    }

    private void ExitButtonTaskPerform() {
        counter = 0;
        isPressing = false;
        Utils.ChangeToPortrait();
        Manager_MainMenu.LoadScene("Main Menu");
    }


    public void ExitButtonPreformed() {
        isPressing = true;
    }

    public void ExitButtonCanceled() {
        isPressing = false;
    }

}
