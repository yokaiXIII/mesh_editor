using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MeshCombiner : MonoBehaviour
{
    // Source Meshes you want to combine
    [SerializeField] private List<MeshFilter> _listMeshFilter;

    // Make a new mesh to be the target of the combine operation
    [SerializeField] private MeshFilter _targetMesh;

    [SerializeField] private Material _combinationMaterial;

    void Awake()
    {
        _combinationMaterial = Resources.Load<Material>("Materials/CombinationMaterial");
    }

    [ContextMenu("Combine Meshes")]
    private void CombineMesh()
    {
        //Make an array of CombineInstance.
        var combine = new CombineInstance[_listMeshFilter.Count];

        //Set Mesh And their Transform to the CombineInstance
        for (int i = 0; i < _listMeshFilter.Count; i++)
        {
            combine[i].mesh = _listMeshFilter[i].sharedMesh;
            combine[i].transform = _listMeshFilter[i].transform.localToWorldMatrix;
        }

        // Create a Empty Mesh
        var mesh = new Mesh();

        //Call targetMesh.CombineMeshes and pass in the array of CombineInstances.
        mesh.CombineMeshes(combine);

        //Assign the target mesh to the mesh filter of the combination game object.
        _targetMesh.mesh = mesh;

        // Save The Mesh To Location
        SaveMesh(_targetMesh.sharedMesh, gameObject.name, false, true);

        // Print Results
        print($"<color=#20E7B0>Combine Meshes was Successful!</color>");

        // DeleteObjects();
        CreateNewObjects();
        _targetMesh.mesh = null; // Clear the mesh reference to avoid memory leaks
    }


    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Meshes/Combined", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = makeNewInstance ? Instantiate(mesh) : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

    private void DeleteObjects()
    {
        foreach (var meshFilter in _listMeshFilter)
        {
            if (meshFilter != null)
            {
                DestroyImmediate(meshFilter.gameObject);
            }
        }

        _listMeshFilter.Clear();
    }

    private void CreateNewObjects()
    {
        GameObject newObject = new GameObject("CombinedMesh");
        MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
        MeshRenderer newMeshRenderer = newObject.AddComponent<MeshRenderer>();

        newMeshFilter.mesh = _targetMesh.sharedMesh;
        if(_combinationMaterial == null)
        {
            _combinationMaterial = Resources.Load<Material>("/Assets/Materials/CombinationMaterial");
            // _combinationMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Assets/Materials/CombinationMaterial.mat");
        }
        newMeshRenderer.materials = new Material[] { _combinationMaterial };

        // Optionally, set the position and rotation of the new object
        newObject.transform.position = Vector3.zero;
        newObject.transform.rotation = Quaternion.identity;

        print($"<color=#20E7B0>New Combined Object Created!</color>");
    }
}