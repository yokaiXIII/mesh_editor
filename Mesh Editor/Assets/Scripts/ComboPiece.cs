using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Collider))] // Ensure the GameObject has MeshFilter and MeshRenderer components
public class ComboPiece : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter; // MeshFilter component to hold the mesh
    [SerializeField] private MeshRenderer _meshRenderer; // MeshRenderer component to render the
                                                         // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<Triangle> _triangles; // List of triangles for the mesh
    public List<Triangle> Triangles => _triangles; // Public property to access the list of triangles
    [SerializeField] private bool _debug = false; // Flag to enable or disable debug drawing

    public Vector3 StartPosition { get; set; } // Public property to access the start position

    private Collider _collider; // Collider for the triangle, if needed
    public Collider Collider => _collider; // Public property to access the collider
    void Start()
    {
        StartPosition = this.transform.position; // Set the start position to the current position of the GameObject
        _meshFilter = GetComponent<MeshFilter>(); // Get the MeshFilter component attached to this GameObject
        _meshRenderer = GetComponent<MeshRenderer>(); // Get the MeshRenderer component attached to this GameObject
        _collider = GetComponent<Collider>(); // Get the MeshRenderer component attached to this GameObject
        Vector3[] vertices = _meshFilter.mesh.vertices; // Initialize the list of vertices
        int[] meshTriangles = _meshFilter.mesh.triangles; // Initialize the list of triangles
        Triangle triangle = new Triangle(); // Create a new Triangle object
        for (int i = 1; i < meshTriangles.Length; i++)
        {
            triangle.AddVertex(vertices[meshTriangles[i - 1]]); // Add the vertex to the triangle
            if (i % 3 == 0)
            {
                _triangles.Add(triangle); // Add the triangle to the list of triangles
                triangle = new Triangle(); // Create a new Triangle
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_triangles == null || _triangles.Count == 0 || _debug == false)
        {
            return; // Exit if there are no triangles to draw
        }
        for (int i = 0; i < _triangles.Count; i++)
        {
            for (int j = 0; j < _triangles[i].vertices.Count; j++)
            {
                Gizmos.color = Color.red; // Set the color for Gizmos
                Vector3 vertex = _triangles[i].vertices[j]; // Get the vertex from the triangle
                Gizmos.DrawSphere(this.transform.position + vertex, 0.01f); // Draw a sphere at the vertex position

                Gizmos.color = Color.cyan; // Set the color for Gizmos
                Gizmos.DrawLine(this.transform.position + _triangles[i].vertices[j], this.transform.position + _triangles[i].vertices[j >= (_triangles[i].vertices.Count - 1) ? 0 : (j + 1)]); // Draw the lines for the triangle vertices
            }

        }
    }
}
[Serializable]
public class Triangle
{
    [SerializeField] public List<Vector3> vertices; // List of vertices
    private List<int> _brokenVertices = new List<int>(); // List of broken vertices, if any
    public List<int> BrokenVertices => _brokenVertices; // List of broken vertices, if any
    public bool IsBroken => _brokenVertices.Count > 0; // Check if the triangle is broken
    public bool IsCompleteBroken => _brokenVertices.Count >= 3; // Check if the triangle is completely broken
    public Triangle()
    {
        vertices = new List<Vector3>(); // Initialize the vertices
    }

    public void AddVertex(Vector3 vertex)
    {
        if (vertices == null || vertices.Count < 3)
        {
            vertices.Add(vertex); // Add the vertex to the list
        }
        else
        {
            Debug.LogError("Triangle already has 3 vertices.");
        }
    }

    public void AddBrokenVertex(int vertexIndex)
    {
        if (!_brokenVertices.Contains(vertexIndex))
        {
            _brokenVertices.Add(vertexIndex); // Add the broken vertex index to the list
        }
    }
    public void ClearBrokenVertices()
    {
        _brokenVertices.Clear(); // Clear the list of broken vertices
    }
}