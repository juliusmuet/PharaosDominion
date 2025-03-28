using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjButton : MonoBehaviour
{
    public static bool areInteractable = true;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Color hoverColor = Color.red;

    [SerializeField] private float hoverRadius = 0.2f;
    [SerializeField] private bool useRectBoundsInstead = false;
    [SerializeField] private Rect buttonBounds = new Rect(0, 0, 1, 1);


    [SerializeField] private bool disableAfterPress = true;
    private Color defaultColor;

    private void Awake() {
        defaultColor = renderer.color;
    }

    protected abstract void performAction();
    protected virtual void Update() {
        if (!areInteractable)
            return;

        if ((!useRectBoundsInstead && Util.MouseInCircle(transform.position, hoverRadius)) || (useRectBoundsInstead && buttonBounds.Contains(CameraController.Instance.GetMouseWorld() - transform.position))) {
            renderer.color = hoverColor;
            if (Input.GetMouseButtonDown(0)) {
                performAction();
                AudioManager.instance.PlaySoundFX(AudioManager.instance.audioButtonFX);
                if (disableAfterPress) {
                    this.enabled = false;
                }
            }
        } else {
            renderer.color = defaultColor;
        }
    }

    public void OnDrawGizmosSelected() {
        if (!useRectBoundsInstead)
            Gizmos.DrawWireSphere(transform.position, hoverRadius);
        else
            Gizmos.DrawWireCube(transform.position + Util.V2ToV3(buttonBounds.center), buttonBounds.size);
    }
}
