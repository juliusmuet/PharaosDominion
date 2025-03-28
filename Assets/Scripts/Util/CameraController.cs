using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public Camera cam;

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject darknessMask;
    public bool isControlledFromOutside = false;

    public void Awake() {
        Instance = this;
        //Debug.Log("Cam Awake");
    }

    public void Update() {
        if (WorldMapFactory.isInWorldmap || isControlledFromOutside) return;
        //Debug.Log("Update");
        if (!PlayerMovement.Instance || !LevelData.Instance)
            return;
        float ratio = 16 / 9f;
        Bounds lvlBounds = LevelData.Instance.GetLevelBoundsWorldSpace();
        this.transform.position = new Vector3(
            Mathf.Clamp(PlayerMovement.Instance.transform.position.x, lvlBounds.min.x + cam.orthographicSize * ratio, lvlBounds.max.x - cam.orthographicSize * ratio),
            Mathf.Clamp(PlayerMovement.Instance.transform.position.y, lvlBounds.min.y + cam.orthographicSize, lvlBounds.max.y - cam.orthographicSize), -10);
    }

    public Vector3 GetMouseWorld() {
        Vector3 res = cam.ScreenToWorldPoint(Input.mousePosition);
        res.z = 0;

        return res;
    }

    public void Shake() {
        anim.SetTrigger("shake");
    }

    public void SetDarknessEnabled(bool flag) {
        darknessMask.SetActive(flag);
    }
}
