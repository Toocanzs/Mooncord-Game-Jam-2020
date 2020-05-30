using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> enemy_types = new List<GameObject>();
    private List<Objective> active_objectives = new List<Objective>();

    void Start()
    {
        var enemy_spawn_points = GetComponentsInChildren<EnemySpawn>().ToList();
        // @TEMP
        enemy_spawn_points.ForEach((EnemySpawn spawn) => {
            var enemy_instance = Instantiate<GameObject>(enemy_types[0], spawn.transform);
            // make new spawns objectives in order to exit room
            enemy_instance.AddComponent<ObjectiveEnemy>();
        });
        
    }

    void Update()
    {
        
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

    public void SetActive() {

    }

    public bool CanExit() {
        return active_objectives.Count == 0;
    }

    public void ExitRoom() {
        // @OTOD: whatever here...
        RoomManager.ExitCurrentRoom();

    }
}
