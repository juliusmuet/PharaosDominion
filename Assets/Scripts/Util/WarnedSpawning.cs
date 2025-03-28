using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarnedSpawning : MonoBehaviour
{
    [SerializeField] private float warnTime = 1f;
    [SerializeField] private GameObject spawnObject;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Animator anim;
    
    private float timeStart = 0f;

    public void Awake() {
        timeStart = Time.time;
        anim.SetFloat("speed", 1 / warnTime);
    }

    public void Update() {
        float elapseTime = Time.time - timeStart;
        if (elapseTime > warnTime) {
            Instantiate(spawnObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
