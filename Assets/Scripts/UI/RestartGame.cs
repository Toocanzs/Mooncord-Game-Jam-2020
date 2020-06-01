using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;

public class RestartGame : MonoBehaviour
{
    public void Show() {

    }

    public void Restart() {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
