using System.Collections.Generic;
using UnityEngine;

public class CombinerManager : MonoBehaviour
{
    [SerializeField] private List<ComboPiece> _objectsToCombine; // Array of objects to combine

    [SerializeField] private List<Vector3> _collisionPoints; // List of collision points

    [SerializeField] private List<Vector3> _brokenVertices; // List of vertices for the triangles


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComboPiece[] objects = Object.FindObjectsByType<ComboPiece>(FindObjectsSortMode.None); // Find all ComboPiece objects in the scene
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                _objectsToCombine.Add(objects[i]); // Add each ComboPiece object to the list of objects to combine
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoved = CheckIfMoved();
        if (!isMoved)
        {
            return; // Exit if no piece has moved
        }

        GetCollisionPoints(); // Get the collision points for the current frame
        CheckVerticesState();
        
    }

    private bool CheckIfMoved()
    {
        bool isMoved = false;
        for (int i = 0; i < _objectsToCombine.Count; i++)
        {
            if (_objectsToCombine[i].StartPosition != _objectsToCombine[i].transform.position)
            {
                isMoved = true; // Set isMoved to true if any piece has moved
                break; // Exit the loop early if a piece has moved
            }
        }
        return isMoved; // Return whether any piece has moved
    }

    void OnDrawGizmos()
    {
        if (_collisionPoints == null || _collisionPoints.Count == 0)
        {
            return; // Exit if there are no collision points to draw
        }

        for (int i = 0; i < _collisionPoints.Count; i++)
        {
            Gizmos.color = Color.red; // Set the color for Gizmos
            Gizmos.DrawSphere(_collisionPoints[i], 0.01f); // Draw a sphere at each collision point
        }
        for (int i = 0; i < _brokenVertices.Count; i++)
        {
            Gizmos.color = Color.cyan; // Set the color for Gizmos
            // Gizmos.DrawSphere(_brokenVertices[i], 0.01f); // Draw a sphere at each collision point
        }
    }

    private void GetCollisionPoints()
    {
        for (int pieceIndex = 0; pieceIndex < _objectsToCombine.Count; pieceIndex++)
        {
            _collisionPoints.Clear(); // Clear the list of collision points before checking for collisions
            _objectsToCombine[pieceIndex].StartPosition = _objectsToCombine[pieceIndex].transform.position; // Update the start position to the current position of the GameObject

            for (int triangleIndex = 0; triangleIndex < _objectsToCombine[pieceIndex].Triangles.Count; triangleIndex++)
            {
                for (int vertexIndex = 0; vertexIndex < _objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices.Count; vertexIndex++)
                {
                    Vector3 startVertex = _objectsToCombine[pieceIndex].gameObject.transform.position + _objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices[vertexIndex]; // Get the start vertex position
                    Vector3 endVertex = _objectsToCombine[pieceIndex].gameObject.transform.position + _objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices[vertexIndex >= (_objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices.Count - 1) ? 0 : (vertexIndex + 1)]; // Get the end vertex position
                    RaycastHit hitInfo;
                    // Raycast to get the collision point
                    if (Physics.Raycast(startVertex, endVertex - startVertex, out hitInfo, Vector3.Distance(startVertex, endVertex)))
                    {
                        // Check if the hit object is a ComboPiece
                        if (hitInfo.collider.gameObject.TryGetComponent(out ComboPiece hitComboPiece))
                        {
                            if (hitComboPiece != _objectsToCombine[pieceIndex]) // Ensure the hit ComboPiece is not the same as the current piece
                            {
                                _collisionPoints.Add(hitInfo.point); // Add the hit point to the collision points list
                            }
                        }
                    }
                }
            }
        }
    }
    private void CheckVerticesState()
    {
        if(_collisionPoints == null || _collisionPoints.Count == 0)
        {
            return; // Exit if there are no collision points to check
        }

        _brokenVertices.Clear(); // Clear the list of broken vertices before checking for collisions
        for (int pieceIndex = 0; pieceIndex < _objectsToCombine.Count; pieceIndex++)
        {
            for (int triangleIndex = 0; triangleIndex < _objectsToCombine[pieceIndex].Triangles.Count; triangleIndex++)
            {
                for (int vertexIndex = 0; vertexIndex < _objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices.Count; vertexIndex++)
                {
                    Vector3 startVertex = _objectsToCombine[pieceIndex].gameObject.transform.position + _objectsToCombine[pieceIndex].Triangles[triangleIndex].vertices[vertexIndex]; // Get the start vertex position
                    RaycastHit hitInfo;

                    if (Physics.Raycast(startVertex, _objectsToCombine[pieceIndex].gameObject.transform.position + Vector3.up, out hitInfo))
                    {
                        Debug.DrawRay(startVertex, _objectsToCombine[pieceIndex].gameObject.transform.position + Vector3.up, Color.red); // Draw a ray for debugging
                        // Check if the hit object is a ComboPiece
                        if (hitInfo.collider.gameObject.TryGetComponent(out ComboPiece hitComboPiece))
                        {
                            if (hitComboPiece != _objectsToCombine[pieceIndex]) // Ensure the hit ComboPiece is not the same as the current piece
                            {
                                _brokenVertices.Add(hitInfo.point); // Add the hit point to the broken vertices list
                            }
                        }
                    }
                }
            }
        }
    }
}
