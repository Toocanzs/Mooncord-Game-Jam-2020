using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Rigidbody2D character_rb;
    private Animator animator;
    private List<SpriteRenderer> sprite_renderers = new List<SpriteRenderer>();

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        character_rb = GetComponent<Rigidbody2D>();
        sprite_renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    void Update()
    {
        if (!character_rb) {
            Debug.LogWarning("Missing rigidbody on character!");
            return;
        }

        var vx = Mathf.Abs(character_rb.velocity.x);
        var vy = Mathf.Abs(character_rb.velocity.y);
        if(vx + vy > 0.01f) {
            animator.SetBool("walking", true);
            var mag = vx + vy;
            var char_movement = GetComponent<CharacterMovement>();
            var mag_alpha = mag / char_movement.move_speed;
            var walk_animation_speed = Mathf.Lerp(1f, 2f, mag_alpha);
            animator.SetFloat("walk_speed", walk_animation_speed);
        } else {
            animator.SetBool("walking", false);
        }
        if(vx > 0.01f) {
            if(character_rb.velocity.x > 0) {
                sprite_renderers.ForEach(x => x.flipX = false);
            } else {
                sprite_renderers.ForEach(x => x.flipX = true);
            }
        }
    }
}
