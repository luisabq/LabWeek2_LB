using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChessPiece))]
public class ChessPieceEditor : Editor
{
    private void OnSceneGUI()
    {
        ChessPiece piece = (ChessPiece)target;
        Transform pieceTransform = piece.transform;
        Transform board = pieceTransform.parent;

        // Draw a tinted border around the current square for quick visual feedback.
        float squareSize = piece.squareSize;
        float halfSquare = squareSize * 0.5f;

        Handles.color = piece.tint;
        Handles.DrawWireCube(pieceTransform.position, new Vector3(squareSize, 0.08f, squareSize));

        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPosition = Handles.PositionHandle(pieceTransform.position, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pieceTransform, "Move Chess Piece");

            if (squareSize <= 0f || piece.boardSize <= 0)
            {
                pieceTransform.position = newWorldPosition;
                return;
            }

            if (board != null)
            {
                Vector3 local = board.InverseTransformPoint(newWorldPosition);

                // Snap to the nearest square center, then clamp so pieces cannot leave the board.
                int col = Mathf.RoundToInt((local.x - halfSquare) / squareSize);
                int row = Mathf.RoundToInt((local.z - halfSquare) / squareSize);

                col = Mathf.Clamp(col, 0, piece.boardSize - 1);
                row = Mathf.Clamp(row, 0, piece.boardSize - 1);

                float snappedX = col * squareSize + halfSquare;
                float snappedZ = row * squareSize + halfSquare;

                pieceTransform.localPosition = new Vector3(snappedX, 0f, snappedZ);
            }
            else
            {
                pieceTransform.position = newWorldPosition;
            }
        }
    }
}