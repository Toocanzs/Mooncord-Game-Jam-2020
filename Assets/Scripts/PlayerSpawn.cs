using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnDirection { NORTH, EAST, SOUTH, WEST };

public class PlayerSpawn : MonoBehaviour
{
    public SpawnDirection spawn_direction;
}
