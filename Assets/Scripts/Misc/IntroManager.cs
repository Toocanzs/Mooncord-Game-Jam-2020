using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private float skip_held_time;

    private void Update() {
        //Input.GetKey()
    }

    private void EndIntro() {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
