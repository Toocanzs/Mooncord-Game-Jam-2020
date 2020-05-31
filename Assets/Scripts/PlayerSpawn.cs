using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomRelativeDirection { NORTH, EAST, SOUTH, WEST };

public class PlayerSpawn : MonoBehaviour
{
    public RoomRelativeDirection spawn_direction;
}
