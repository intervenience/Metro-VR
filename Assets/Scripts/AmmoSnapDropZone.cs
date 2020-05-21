
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    public class AmmoSnapDropZone : VRTK_SnapDropZone {

        [SerializeField] GameObject akMagPrefab, ak2012MagPrefab, shotgunShellPrefab, pistolAmmoPrefab;

        protected override void OnEnable () {
            base.OnEnable ();

            HandController.OnPickedUpRangedWeapon += HandController_OnPickedUpRangedWeapon;
        }

        protected override void OnDisable () {
            base.OnDisable ();

            HandController.OnPickedUpRangedWeapon -= HandController_OnPickedUpRangedWeapon;
        }

        Item heldWeapon;

        void HandController_OnPickedUpRangedWeapon (Item item, HandLeftOrRight leftOrRight) {
            GameObject temp;
            switch (heldWeapon.itemName) {
                case "Kalashnikov":
                    if (PlayerInventory.Instance.sevensixtwoAmmo > 0) {
                        temp = Instantiate (akMagPrefab);
                        var mag = temp.GetComponent<Magazine> ();
                        var rounds = PlayerInventory.Instance.sevensixtwoAmmo < mag.MaxAmmo ? PlayerInventory.Instance.sevensixtwoAmmo : mag.MaxAmmo;
                        mag.currentAmmo = rounds;
                        PlayerInventory.Instance.sevensixtwoAmmo -= rounds;

                        Destroy (currentSnappedObject);
                        ForceSnap (temp);
                    }
                    break;
                case "AK 2012":
                    if (PlayerInventory.Instance.sevensixtwoAmmo > 0) {
                        temp = Instantiate (ak2012MagPrefab);
                        var mag = temp.GetComponent<Magazine> ();
                        var rounds = PlayerInventory.Instance.sevensixtwoAmmo < mag.MaxAmmo ? PlayerInventory.Instance.sevensixtwoAmmo : mag.MaxAmmo;
                        mag.currentAmmo = rounds;
                        PlayerInventory.Instance.sevensixtwoAmmo -= rounds;

                        Destroy (currentSnappedObject);
                        ForceSnap (temp);
                    }
                    break;
                case "Shotgun":
                    if (PlayerInventory.Instance.shotgunShells > 0) {
                        temp = Instantiate (shotgunShellPrefab);
                        PlayerInventory.Instance.shotgunShells -= 1;

                        Destroy (currentSnappedObject);
                        ForceSnap (temp);
                    }
                    break;
            }
        }

    }

}
