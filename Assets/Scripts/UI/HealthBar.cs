using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform entityTransform;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Color enemyColor;
    [SerializeField] private Color playerTeamColor;
    [SerializeField] private Color playerColor;

    private float scaleX, scaleY, scaleZ;

    public void updateHealthBar(int currentHP, int maxHP)
    {
        float fillAmount = (float) currentHP / maxHP;
        healthBarFill.fillAmount = fillAmount;
    }

    private void Start()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;

        GameObject parent = transform.parent.gameObject;
        Entity parentEntity = parent.GetComponent<Entity>();
        if (parent.CompareTag("Player")) healthBarFill.color = playerColor;
        else if (parentEntity.GetTeam() == Team.PLAYER) healthBarFill.color = playerTeamColor;
        else if (parentEntity.GetTeam() == Team.ENEMY) healthBarFill.color = enemyColor;
    }

    private void Update()
    {
        if (entityTransform == null) return;

        if (entityTransform.localScale.x == 1)
        {
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
        if (entityTransform.localScale.x == -1)
        {
            transform.localScale = new Vector3(-scaleX, scaleY, scaleZ);
        }
    }
}
