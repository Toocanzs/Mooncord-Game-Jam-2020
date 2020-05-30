using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSyringe : Weapon
{
    protected override void Fire() {
        base.Fire();
        if (!projectile) {
            Debug.LogError("WeaponSyringe requires projectile to be set!");
        }
        var fire_direction = GetFireDirection();
        //Debug.Log("Fire direction: " + fire_direction + " mag: " + fire_direction.magnitude);
        if (!owner) {
            Debug.LogError("Weapon missing owner!");
            return;
        }
        var projectile_instance = Instantiate<GameObject>(projectile, owner.transform);
        var projectile_component = projectile_instance.GetComponent<WeaponProjectile>();
        if (!projectile_component) {
            Debug.LogError("No WeaponProjectile component found on instantiated projectile!");
            return;
        }
        var owner_team_component = owner.GetComponent<TeamComponent>();
        if (!owner_team_component) {
            Debug.LogError("Owner missing TeamComponent!");
            return;
        }
        WeaponProjectileProperties projectile_properties;
        projectile_properties.direction = fire_direction;
        projectile_properties.team = owner_team_component.Team;
        projectile_component.SetProperties(projectile_properties);
    }

}
