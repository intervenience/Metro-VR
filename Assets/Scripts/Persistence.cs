
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MetroVR {

    public class Persistence : MonoBehaviour {

        public SaveData saveData;
        private const string folderName = "Metro VR";
        private const string saveFileName = "save.dat";

        void Awake () {
            if (!Directory.Exists (Path.Combine (Application.persistentDataPath, folderName))) {
                Directory.CreateDirectory (Path.Combine (Application.persistentDataPath, folderName));
            }

            //Load ();

            Debug.Log (saveData.levelStage);
        }

        public void Save () {
            var data = JsonUtility.ToJson (saveData);

            StreamWriter sw;
            FileStream fs = File.Open (Path.Combine (Application.persistentDataPath, folderName, saveFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Debug.Log ("File path = " + Path.Combine (Application.persistentDataPath, folderName, saveFileName));
            sw = new StreamWriter (fs);

            try {
                Debug.Log (data);
                sw.Write (data);
            } catch (System.Exception e) {
                Debug.LogError ("Something potentially went wrong saving.\n" + e);
            } finally {
                sw.Close ();
                fs.Close ();
            }
        }

        public void Load () {
            //read data to string

            if (File.Exists (Path.Combine (Application.persistentDataPath, folderName, saveFileName))) {
                StreamReader sr;
                FileStream fs = File.Open (Path.Combine (Application.persistentDataPath, saveFileName, saveFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);

                sr = new StreamReader (fs);

                try {
                    string fileData = sr.ReadToEnd ();
                    if (fileData.Length > 0) {
                        saveData = JsonUtility.FromJson <SaveData> (fileData);
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
