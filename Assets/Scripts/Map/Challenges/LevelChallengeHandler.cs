using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LevelChallengeHandler : MonoBehaviour
{
    //public LevelChallengeHandler Instance { get; private set; }
    [SerializeField] private GameObject warningStrayBulletPrefab;
    [SerializeField] private GameObject trapPrefab;

    public void Start() {
        //Instance = this;

        ApplySpecialChallenges();

        ChallengeController.Instance.MoveChallengesToRewards();
        timeLastStrayBullet = Time.time + 4;
    }

    private float timeLastStrayBullet;

    public void ApplySpecialChallenges() {
        if (ChallengeController.Instance.darkness > 0) {
            CameraController.Instance.SetDarknessEnabled(true);
        }

        if (ChallengeController.Instance.timeSpeedUpModificator != 1) {
            Time.timeScale = ChallengeController.Instance.timeSpeedUpModificator;
        }

        if (ChallengeController.Instance.trapAmount > 0) {
            for(int i = 0; i < ChallengeController.Instance.trapAmount; i++) {

                int tryCount = 0;
                Bounds bounds = LevelData.Instance.GetLevelBoundsWorldSpace();
                Vector3 pos;

                while (true) {
                    pos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pos, out hit, 0.1f, NavMesh.AllAreas)) {
                        break;
                    }
                    if (++tryCount > 50) {
                        pos = Util.INVALID_VEC;
                        break;
                    }
                }
                if (pos == Util.INVALID_VEC)
                    continue;

                Instantiate(trapPrefab, pos, Quaternion.identity);
            }
        }
    }  

    public void OnWin() {
        if (ChallengeController.Instance.darkness > 0) {
            CameraController.Instance.SetDarknessEnabled(false);
        }
        if (ChallengeController.Instance.timeSpeedUpModificator != 1) {
            Time.timeScale = 1f;
        }
    }


    public void Update() {
        
        if (LevelManager.levelFinished) {
            OnWin();
            ChallengeController.Instance.OnLevelEnd();
            Destroy(this);
            return;
        }
        
        if (!PlayerMovement.Instance) return;
        
        if (ChallengeController.Instance == null) {
            return; // THIS SHOULD NOT HAPPEN 
        }
       
        if (ChallengeController.Instance.stryBullets > 0) {
            
            float strayBulletDelay = 2f - ChallengeController.Instance.stryBullets * 0.5f;
            if (Time.time - timeLastStrayBullet > strayBulletDelay) {
                Vector3 pos = Vector3.zero;
                int tryCount = 0;
                while (true) {
                    float angle = Random.Range(0, 2 * Mathf.PI);
                    pos = PlayerMovement.Instance.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(3, 10);

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pos, out hit, 0.1f, NavMesh.AllAreas)) {
                        break;
                    }

                    if (++tryCount > 50)
                        return;
                }
                Instantiate(warningStrayBulletPrefab, pos, Quaternion.identity);
                timeLastStrayBullet = Time.time;
            }

        }
    }
}
