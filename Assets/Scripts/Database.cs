
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR { 

    //Refer to the Editor/DatabaseInspector.cs for this files visibility in the inspector
    [System.Serializable]
    [CreateAssetMenu (fileName = "Database")]
    public class Database : ScriptableObject {

        public List <ItemDb> items = new List<ItemDb> ();
        public List <MagazineDb> magazines = new List<MagazineDb> ();
        public List<MobsDb> mobs = new List<MobsDb> ();

    }

    [System.Serializable]
    public class ItemDb {

        public int id;
        public string gameObjectName;
        public GameObject prefab;
        public int magId;
        public bool magIn;

    }

    [System.Serializable]
    public class MagazineDb {

        public int id;
        public string gameObjectName;
        public GameObject prefab;

    }

    [System.Serializable]
    public class MobsDb {

        public int id;
        public string mobName;
        public GameObject prefab;

    }

}
