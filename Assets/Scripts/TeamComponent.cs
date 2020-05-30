using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Player, Enemy }

public class TeamComponent: MonoBehaviour {

    [SerializeField]
    private Team _team;

    public Team Team {
        get { return _team; }
    }
}
