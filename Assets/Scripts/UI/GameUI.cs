using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    private bool escape_pressed;

    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            if (!escape_pressed) {
                escape_pressed = true;
                ToggleMenu();
            }
        } else {
            escape_pressed = false;
        }
    }

    public void ToggleMenu() {
        var restart = GetComponentInChildren<RestartGame>(true);
        var quit = GetComponentInChildren<Quit>(true);
        if (!restart.gameObject.activeInHierarchy) {
            restart.gameObject.SetActive(true);
        } else if(restart.gameObject.activeInHierarchy && !quit.gameObject.activeInHierarchy) {
            // nothing...
        } else {
            restart.gameObject.SetActive(false);
        }
        quit.gameObject.SetActive(restart.gameObject.activeInHierarchy);
    }

    public void SetPlayerDeath() {
        var you_died = GetComponentInChildren<YouDied>(true);
        you_died.gameObject.SetActive(true);
        you_died.Show(() => {
            var restart = GetComponentInChildren<RestartGame>(true);
            if (!restart.gameObject.activeInHierarchy) {
                restart.gameObject.SetActive(true);
                //restart.Show();
            }
        });
    }
}
