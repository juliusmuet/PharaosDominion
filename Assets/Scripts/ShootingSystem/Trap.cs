using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Trap : ITeam
{
    [SerializeField] private float noExplosionTimeAtStart = 5f;
    [SerializeField] private float triggerRadius = 1.5f;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject explosionParticleEffect;
    [SerializeField] private int damage = 100;

    private float doNotExplodeBefore;
    private void Awake() {
        doNotExplodeBefore = Time.time + noExplosionTimeAtStart;
    }

    private void Update() {
        if (!PlayerMovement.Instance || Time.time < doNotExplodeBefore)
            return;

        if(Util.PointInCircle(PlayerMovement.Instance.transform.position, transform.position, triggerRadius)) {
            anim.SetTrigger("fuse");
        }
    }

    public void Explode() {
        Instantiate(explosionParticleEffect, transform.position, Quaternion.identity);
        LinkedList<Hitbox> hits = SpacialGrouping.currentGrouping.CollisionWithAll(hitbox);

        foreach (Hitbox hit in hits) {
            Entity ent = hit.GetComponent<Entity>();
            if (ent != null)
                ent.Damage(damage);
        }

        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }


}
