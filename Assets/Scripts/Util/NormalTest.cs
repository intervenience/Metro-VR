using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTest : MonoBehaviour {

    Camera cam;

    void Start() {
        cam = GetComponent<Camera> ();
        //Mesh mesh = GetComponent<MeshFilter> ().mesh;
        //foreach (Vector3 normal in mesh.normals) {
        //    Debug.Log (normal);
        //}
    }

    RaycastHit hit;

    /*void Update () {
        if (Input.GetMouseButtonDown (0)) {
            Debug.Log ("Clicked " + Input.mousePosition);
            

            if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 15f)) {
                Mesh mesh = hit.collider.GetComponent <MeshFilter> ().sharedMesh;
                List<Vector3> verts = new List<Vector3> (mesh.vertices);
                int[] tris = mesh.triangles;
                Debug.Log (verts.Count + " " + tris.Length);
                Debug.Log (hit.triangleIndex * 3 + 0);
                Vector3 p0 = verts[tris[hit.triangleIndex * 3 + 0]];
                Vector3 p1 = verts[tris[hit.triangleIndex * 3 + 1]];
                Vector3 p2 = verts[tris[hit.triangleIndex * 3 + 2]];
                Debug.Log (p0 + " " + p1 + " " + p2);
            }
        }
    }*/
    void Update () {
        RaycastHit hit;
        if (Input.GetMouseButtonDown (0)) {
            if (!Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit))
                return;

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Mesh mesh = hit.collider.GetComponent<MeshFilter> ().sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            //Debug.Log (hit.triangleIndex);
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            

            Vector3 gp0 = new Vector3 (Random.Range (p0.x, p1.x),
                                       Random.Range (p0.y, p1.y),
                                       Random.Range (p0.z, p1.z));
            Vector3 gp1 = new Vector3 (Random.Range (p1.x, p2.x),
                                       Random.Range (p1.y, p2.y),
                                       Random.Range (p1.z, p2.z));
            //gp2 engine, gp2
            Vector3 gp2 = new Vector3 (Random.Range (p2.x, p0.x),
                                       Random.Range (p2.y, p0.y),
                                       Random.Range (p2.z, p0.z));

            List<int> addedTris = new List<int> ();
            List<Vector3> vertPoints = new List<Vector3> () {
                p0, gp0, p1, gp1, p2, gp2
            };

            for (int i = 0; i < vertPoints.Count; i++) {
                Mesh createdTri1 = new Mesh ();
                createdTri1.Clear ();
                createdTri1.name = "CreatedTri " + i.ToString ();
                if (i != vertPoints.Count - 1) {
                    createdTri1.SetVertices (new List<Vector3> () { hit.barycentricCoordinate, vertPoints[i], vertPoints[i + 1]});
                } else {
                    createdTri1.SetVertices (new List<Vector3> () { hit.barycentricCoordinate, vertPoints[i], vertPoints[0] });
                }
                
                List<int> tris = new List<int> ();
                for (int j = 0; j < createdTri1.vertices.Length; j++) {
                    tris.Add (j);
                }
                createdTri1.SetTriangles (tris, 0);

                GameObject createdGo = new GameObject ("Created object " + i.ToString ());
                createdGo.transform.position = hit.collider.transform.position;
                createdGo.AddComponent<MeshFilter> ().mesh = createdTri1;
                createdGo.AddComponent<MeshRenderer> ();
            }



            Transform hitTransform = hit.collider.transform;
            p0 = hitTransform.TransformPoint (p0);
            p1 = hitTransform.TransformPoint (p1);
            p2 = hitTransform.TransformPoint (p2);

            Debug.DrawLine (p0, p1, Color.red, 2f);
            Debug.DrawLine (p1, p2, Color.red, 2f);
            Debug.DrawLine (p2, p0, Color.red, 2f);

            //hit.collider.gameObject.SetActive (false);
        }
        
    }
}
