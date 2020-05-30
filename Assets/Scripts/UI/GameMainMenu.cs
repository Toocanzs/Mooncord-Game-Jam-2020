using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameMainMenu : MonoBehaviour
{
    public void PlayClicked() {
        SceneManager.LoadScene("IntroScene", LoadSceneMode.Single);
    }
}
