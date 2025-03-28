using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITeam : MonoBehaviour
{
    [SerializeField] private Team team;
    public Team GetTeam() {
        return team;
    }

    public void SetTeam(Team team) {
        this.team = team;
    }
}
