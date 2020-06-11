
using UnityEngine;

namespace MetroVR {

    public enum ItemTypes {
        LargeWeapon,
        SmallWeapon,
        MeleeWeapon,
        Misc
    }

    public class Item : VRTK.VRTK_InteractableObject {

        [Header ("Item Properties")]
        public string itemName;
        [SerializeField] private int id;
        public int ID => id;
        public ItemTypes itemType;
        public HandPosition handPosition;

    }

}
