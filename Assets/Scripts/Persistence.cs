
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MetroVR {

    public class Persistence : MonoBehaviour {

        public SaveData saveData;
        private const string saveFileName = "save.json";
        private const string tempFileName = "temp.json";

        void Awake () {
            if (!Directory.Exists (Application.persistentDataPath)) {
                Directory.CreateDirectory (Application.persistentDataPath);
            }

            //Load ();
        }

        public void Save () {
            var data = JsonUtility.ToJson (saveData);

            StreamWriter sw;

            //Create a backup
            if (File.Exists (Path.Combine (Application.persistentDataPath, saveFileName))) {
                File.Copy (Path.Combine (Application.persistentDataPath, saveFileName), Path.Combine (Application.persistentDataPath, tempFileName));
                File.Delete (Path.Combine (Application.persistentDataPath, saveFileName));
            }

            FileStream fs = File.Open (Path.Combine (Application.persistentDataPath, saveFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Debug.Log ("File path = " + Path.Combine (Application.persistentDataPath, saveFileName));
            sw = new StreamWriter (fs);

            try {
                Debug.Log (data);
                sw.Write (data);
            } catch (System.Exception e) {
                Debug.LogError ("Something potentially went wrong saving.\n" + e);

                //In an error we revert the backup filename to the real savefile name
                if (File.Exists (Path.Combine (Application.persistentDataPath, saveFileName))) {
                    File.Copy (Path.Combine (Application.persistentDataPath, tempFileName), Path.Combine (Application.persistentDataPath, saveFileName));
                }
            } finally {
                sw.Close ();
                fs.Close ();
            }

            //Delete the temporary file if it still exists (if the save is successful, this will be true)
            if (File.Exists (Path.Combine (Application.persistentDataPath, tempFileName))) {
                File.Delete (Path.Combine (Application.persistentDataPath, tempFileName));
            }
        }

        public void Load () {
            //read data to string

            if (File.Exists (Path.Combine (Application.persistentDataPath, saveFileName))) {
                StreamReader sr;
                FileStream fs = File.Open (Path.Combine (Application.persistentDataPath, saveFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);

                sr = new StreamReader (fs);

                try {
                    string fileData = sr.ReadToEnd ();
                    Debug.Log (fileData);
                    if (fileData.Length > 0) {
                        saveData = JsonUtility.FromJson <SaveData> (fileData);
                        Debug.Log (saveData.playspacePosition);
                    }
                } catch (System.Exception e) {
                    Debug.LogError ("Error loading save data. \n" + e);
                } finally {
                    sr.Close ();
                    fs.Close ();
                }
            }
        }

    }

}
