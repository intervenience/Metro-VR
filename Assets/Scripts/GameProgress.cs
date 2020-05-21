using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgress : MonoBehaviour {

    List<Transform> gameObjectives;
    int stage = 0;

    [SerializeField] Transform target;

    void OnTriggerEnter (Collider collider) {

    }

    public void StageCompleted (Transform objective) {
        if (target == objective) {

        }
    }

}
