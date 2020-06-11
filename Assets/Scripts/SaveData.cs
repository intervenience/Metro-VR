
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    [System.Serializable]
    public struct SaveData {

        public int levelStage;

        public Vector3 playspacePosition;
        public Vector3 playerLocalPosition;
        public Vector3 playspaceRotation;

        public List<ItemData> itemData;
        public List<WorldObjectData> worldObjects;
        public List<MobPositionData> mobs;
    }

    [System.Serializable]
    public struct ItemData {
        public string gameObjectName;
        public int itemId;
        public int holsterId;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public struct WorldObjectData {
        public string gameObjectName;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public struct MobPositionData {
        public int id;
        public string gameObjectName;
        public Vector3 position;
        public Vector3 rotation;
        public float hp;
        public bool isActive;
    }

}
