using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private bool player_input_enabled;
    private static ControlManager instance;

    private void Awake() {
        if (!instance) {
            instance = this;
        }
        // @TEMP;
        player_input_enabled = true;
    }

    public static bool IsInputEnabled() {
        return instance ? instance.player_input_enabled : true;
    }

    public static void SetInputEnabled(bool value) {
        instance.player_input_enabled = value;
    }
}
