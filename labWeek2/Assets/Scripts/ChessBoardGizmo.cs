using UnityEngine;

public class ChessBoardGizmo : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 8;
    public float squareSize = 1f;

    [Header("Grid Color")]
    public Color outlineColor = Color.black;

    private void OnDrawGizmos()
    {
        // Draw in board-local space so the grid follows this transform.
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = outlineColor;

        float halfSquare = squareSize * 0.5f;
        Vector3 cellSize = new Vector3(squareSize, 0.01f, squareSize);

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                // Place each wire cell at the center of its square.
                Vector3 center = new Vector3(
                    col * squareSize + halfSquare,
                    0f,
                    row * squareSize + halfSquare
                );

                Gizmos.DrawWireCube(center, cellSize);
            }
        }

        // Reset to avoid affecting gizmos drawn by other objects.
        Gizmos.matrix = Matrix4x4.identity;
    }
}