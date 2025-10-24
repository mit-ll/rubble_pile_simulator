// DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
//  
// This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
//  
// © 2024 Massachusetts Institute of Technology.
// Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
//  
// The software/firmware is provided to you on an As-Is basis
//  
// Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.


using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SceneToSTLExporter : MonoBehaviour
{
    public static void ExportSceneToSTL()
    {
        string dataPath = Application.persistentDataPath;
        string folderPath = Path.Combine(dataPath, "SceneData");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"GroundTruth_{DateTime.Now.ToString("yy-MM-dd-HH-mm")}.stl");

        string path = filePath;

        Mesh combinedMesh = CombineSceneMeshes();
        ExportMeshToSTL(combinedMesh, path);
        Debug.Log("Scene exported to " + path);
    }
    
    

    static Mesh CombineSceneMeshes()
    {
        DebrisSpawner debrisSpawner = GameObject.FindObjectOfType<DebrisSpawner>();

        List<MeshFilter> meshes = new List<MeshFilter>();

        if (debrisSpawner != null)
        {
            List<GameObject> debrisObjects = debrisSpawner.GetDebrisObj();
            foreach (var debrisObject in debrisObjects)
            {
                meshes.Add(debrisObject.GetComponent<MeshFilter>());
            }
        }
        MeshFilter[] meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
        if (meshes.Count > 0)
        {
            meshFilters = meshes.ToArray();
        }
        
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // for large meshes
        finalMesh.CombineMeshes(combine);
        return finalMesh;
    }

    static void ExportMeshToSTL(Mesh mesh, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            // Write 80-byte header
            writer.Write(new byte[80]);

            int numTriangles = mesh.triangles.Length / 3;
            writer.Write(numTriangles);

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];
                Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                // Write normal
                writer.Write(normal.x);
                writer.Write(normal.y);
                writer.Write(normal.z);

                // Write vertices
                foreach (var v in new Vector3[] { v1, v2, v3 })
                {
                    writer.Write(v.x);
                    writer.Write(v.y);
                    writer.Write(v.z);
                }

                // Attribute byte count
                writer.Write((ushort)0);
            }
        }
    }
}