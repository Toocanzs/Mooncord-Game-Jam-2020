using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct RoomSetupProperties {
    public RoomRelativeDirection enter_from;
};

public class Room : MonoBehaviour
{
    public List<GameObject> enemy_types = new List<GameObject>();
    private List<Objective> active_objectives = new List<Objective>();
    public bool room_setup_complete;

    public void SetupRoom(RoomSetupProperties properties)
    {
        var player = PlayerCharacter.GetPlayerCharacter();
        var desired_spawn_location = GetOppositeDirection(properties.enter_from);
        var player_spawn_points = GetComponentsInChildren<PlayerSpawn>().ToList();
        var player_spawn = player_spawn_points.Find((x) => x.spawn_direction == desired_spawn_location);
        if(player_spawn == null) {
            player_spawn = player_spawn_points[0];
        }
        player.transform.position = player_spawn.transform.position;

        var enemy_spawn_points = GetComponentsInChildren<EnemySpawn>().ToList();
        enemy_spawn_points.ForEach((EnemySpawn spawn) => {
            // @TODO: spawn more than one enemy type...
            var enemy_instance = Instantiate<GameObject>(enemy_types[0], spawn.transform);
            // make new spawns objectives in order to exit room
            enemy_instance.AddComponent<ObjectiveEnemy>();
            var ai_brain = enemy_instance.GetComponent<AIBrain>();
            // disable AI at start
            ai_brain.EnableBrain(false);
            var distance_to_player = Vector3.Distance(enemy_instance.transform.position, player.gameObject.transform.position);
            if (distance_to_player >= 10f) {
                ai_brain.SetEnableOnDistanceOrDamage();
            }
        });
        // @TODO: iterate over AI and set "wake nearby" on wake...

        room_setup_complete = true;
    }

    public void RegisterObjective(Objective objective) {
        active_objectives.Add(objective);
    }

    public void RemoveObjective(Objective objective) {
        active_objectives.Remove(objective);
        if (CanExit()) {
            var room_exits = GetComponentsInChildren<RoomExit>().ToList();
            room_exits.ForEach(x => x.SetExitable());
        }
    }

    private RoomRelativeDirection GetOppositeDirection(RoomRelativeDirection in_direciton) {
        switch (in_direciton) {
            case RoomRelativeDirection.NORTH:
                return RoomRelativeDirection.SOUTH;
            case RoomRelativeDirection.SOUTH:
                return RoomRelativeDirection.NORTH;
            case RoomRelativeDirection.EAST:
                return RoomRelativeDirection.WEST;
            case RoomRelativeDirection.WEST:
                return RoomRelativeDirection.EAST;
            default:
                return RoomRelativeDirection.NORTH;
        }
    }

    public void ActivateRoom() {
        if (!room_setup_complete) {
            Debug.LogError("Room not setup! Run SetupRoom first!");
        }
        var enemies = GetComponentsInChildren<AIBrain>().ToList();
        enemies.ForEach((e) => {
            if (!e.IsEnabledOnDistance()) {
                e.EnableBrain(true);
            }
        });
    }

    public bool CanExit() {
        return active_objectives.Count == 0;
    }

    public void ExitRoom(RoomExit exit) {
        // @OTOD: whatever room-specific stuff here...
        //exit.exit_direction;
        RoomManager.ExitCurrentRoom(this, exit);
    }
}
