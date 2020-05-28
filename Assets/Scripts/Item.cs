
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
        public ItemTypes itemType;
        public HandPosition handPosition;

    }

}
