using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnPlayer : MonoBehaviour
{
    public void Update() {
        PlayerMovement player = PlayerMovement.Instance;
        if (!player) return;

        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
    }
}
