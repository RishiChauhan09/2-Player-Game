using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour {

    public static Vector2 ScreenToWorld(Camera camera, Vector3 pos) {
        pos.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(pos);
    }


    // getting all game children objects
    public static List<GameObject> GetAllChildren(GameObject parent) {
        Transform[] childObjects = parent.GetComponentsInChildren<Transform>();

        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < childObjects.Length; i++) {
            children.Add(childObjects[i].gameObject);
        }

        return children;
    }


    public static IEnumerator WaitAndLoadScene(string name) {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(name);
        Debug.Log("Scene changed");
        ChangeToPortrait();

    }


    public static void ChangeToLandScape() {
        int width = 1600;
        int height = 720;
        Screen.SetResolution(width, height, true);

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }


    public static void ChangeToPortrait() {
        int width = 720;
        int height = 1600;
        Screen.SetResolution(width, height, true);

        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
    }

}