using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public List<Image> hearts = new List<Image>();
    public Sprite full_heart_sprite;
    public Sprite half_heart_sprite;

    void Update()
    {
        var player = PlayerCharacter.GetPlayerCharacter();
        if (player) {
            var player_health = player.gameObject.GetComponent<HealthComponent>();
            if (player_health.max_health / 2 > hearts.Count) {
                // @TODO: donno if we care to do this..
            }
            for(int i = 0; i < hearts.Count; i++) {
                SetHearts(i, player_health.Health);
            }
        }
    }

    private void SetHearts(int index, int value) {
        var full_heart_value = (index * 2) + 2;
        hearts[index].enabled = true;
        if(value >= full_heart_value) {
            hearts[index].sprite = full_heart_sprite;
        } else if(value == full_heart_value - 1) {
            hearts[index].sprite = half_heart_sprite;
        } else {
            hearts[index].enabled = false;
        }
    }
}
