using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathCutScene : MonoBehaviour
{
    public static BossDeathCutScene Instance { get; private set; }

    [SerializeField] private float cameraLerpDuration = 1.5f;
    [SerializeField] private new SpriteRenderer renderer;

    private Vector3 smoothDampVelocity = Vector3.zero;

    private void Awake() {
        Instance = this;
        CameraController.Instance.isControlledFromOutside = true;
        renderer.flipX = transform.position.x > 0;
    }

    public void Update() {
        CameraController.Instance.transform.position = Vector3.SmoothDamp(CameraController.Instance.transform.position, transform.position + new Vector3(0, 0, -10), ref smoothDampVelocity, cameraLerpDuration * Time.deltaTime);
    }
}
