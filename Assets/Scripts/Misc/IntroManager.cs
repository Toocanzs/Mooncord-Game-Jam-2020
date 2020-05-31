using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class IntroManager : MonoBehaviour
{
    public List<GameObject> intro_text = new List<GameObject>();
    private int intro_index;
    private float skip_held_time;
    private SkipIndicator skip_indicator_component;
    private GameObject current_intro_text_intance;

    private void Start() {
        skip_indicator_component = FindObjectOfType<SkipIndicator>();
        StartIntro();
    }

    private void StartIntro() {
        SpawnNextIntroElement();
    }

    private void SpawnNextIntroElement() {
        if (intro_index < intro_text.Count) {
            var canvas_component = FindObjectOfType<Canvas>();
            if (!canvas_component) {
                Debug.LogError("Missing canvas component in scene!!");
                return;
            }
            if (current_intro_text_intance) {
                Destroy(current_intro_text_intance);
            }
            current_intro_text_intance = Instantiate(intro_text[intro_index], canvas_component.gameObject.transform);
            var text_element_component = current_intro_text_intance.GetComponent<TextElement>();
            if (!text_element_component) {
                Debug.LogWarning("Unable to find TextElement component on intro text instance");
                return;
            }
            text_element_component.OnComplete += (caller, args) => { SpawnNextIntroElement(); };
            text_element_component.StartHidden(true);
            var text_length = text_element_component.GetTextLength();
            text_element_component.RevealText(text_length / 15);
            intro_index++;
        } else {
            Destroy(current_intro_text_intance);
            EndIntro();
        }
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
