using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private float skip_held_time;
    private SkipIndicator skip_indicator_component;

    private void Start() {
        skip_indicator_component = Object.FindObjectOfType<SkipIndicator>();
    }

    private void Update() {
        if(Input.GetKey(KeyCode.Escape))
        {
            skip_held_time += Time.deltaTime;
        } else {
            skip_held_time = Mathf.Max(0f, skip_held_time - Time.deltaTime);
        }
        if (skip_indicator_component) {
            skip_indicator_component.SetProgress(skip_held_time / 0.5f);
        }

        if(skip_held_time >= 0.50f) {
            Debug.Log("Intro skipped");
            EndIntro();
        }
    }

    private void EndIntro() {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
