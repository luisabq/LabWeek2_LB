using UnityEngine;

public enum ChessPieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public enum ChessPieceColor
{
    White,
    Black
}

[ExecuteAlways]
public class ChessPiece : MonoBehaviour
{
    private static readonly Color BlackPieceColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private static readonly Vector2Int[] KnightOffsets =
    {
        new Vector2Int(1, 2),
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, -2),
        new Vector2Int(-1, -2),
        new Vector2Int(-2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-1, 2)
    };

    [Header("Piece Settings")]
    public ChessPieceType pieceType = ChessPieceType.Pawn;
    public ChessPieceColor pieceColor = ChessPieceColor.White;

    [Tooltip("Extra color tint applied to the piece.")]
    public Color tint = Color.white;

    [Header("Sprites")]
    public Sprite pawnSprite;
    public Sprite rookSprite;
    public Sprite knightSprite;
    public Sprite bishopSprite;
    public Sprite queenSprite;
    public Sprite kingSprite;

    [Header("Board Settings")]
    public int boardSize = 8;
    public float squareSize = 1f;

    [Header("Sprite Child")]
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        UpdateVisual();
    }

    private void OnValidate()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Auto-find a child sprite renderer so setup is less fragile in the editor.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = GetSpriteForType(pieceType);

        // Keep side colors consistent in every piece prefab.
        spriteRenderer.color = pieceColor == ChessPieceColor.White ? Color.white : BlackPieceColor;
    }

    private Sprite GetSpriteForType(ChessPieceType type)
    {
        switch (type)
        {
            case ChessPieceType.Pawn:
                return pawnSprite;

            case ChessPieceType.Rook:
                return rookSprite;

            case ChessPieceType.Knight:
                return knightSprite;

            case ChessPieceType.Bishop:
                return bishopSprite;

            case ChessPieceType.Queen:
                return queenSprite;

            case ChessPieceType.King:
                return kingSprite;

            default:
                return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (squareSize <= 0f || boardSize <= 0)
        {
            return;
        }

        // Draw move previews in local board space so markers follow object transforms.
        Gizmos.matrix = transform.localToWorldMatrix;

        // Reuse the inspector tint so preview color is easy to control while editing.
        Gizmos.color = tint;

        int currentCol = GetCurrentColumn();
        int currentRow = GetCurrentRow();
        Vector3 markerSize = new Vector3(squareSize * 0.8f, 0.05f, squareSize * 0.8f);

        switch (pieceType)
        {
            case ChessPieceType.Pawn:
                DrawPawnMoves(currentCol, currentRow, markerSize);
                break;

            case ChessPieceType.Rook:
                DrawRookMoves(currentCol, currentRow, markerSize);
                break;

            case ChessPieceType.Knight:
                DrawKnightMoves(currentCol, currentRow, markerSize);
                break;

            case ChessPieceType.Bishop:
                DrawBishopMoves(currentCol, currentRow, markerSize);
                break;

            case ChessPieceType.Queen:
                DrawRookMoves(currentCol, currentRow, markerSize);
                DrawBishopMoves(currentCol, currentRow, markerSize);
                break;

            case ChessPieceType.King:
                DrawKingMoves(currentCol, currentRow, markerSize);
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;
    }

    private int GetCurrentColumn()
    {
        return Mathf.FloorToInt(transform.localPosition.x / squareSize);
    }

    private int GetCurrentRow()
    {
        return Mathf.FloorToInt(transform.localPosition.z / squareSize);
    }

    private bool IsInsideBoard(int col, int row)
    {
        return col >= 0 && col < boardSize && row >= 0 && row < boardSize;
    }

    private void DrawMove(int currentCol, int currentRow, int offsetX, int offsetZ, Vector3 markerSize)
    {
        int targetCol = currentCol + offsetX;
        int targetRow = currentRow + offsetZ;

        // Skip markers that would land outside the board bounds.
        if (!IsInsideBoard(targetCol, targetRow))
        {
            return;
        }

        Vector3 center = new Vector3(offsetX * squareSize, 0.05f, offsetZ * squareSize);
        Gizmos.DrawWireCube(center, markerSize);
    }

    private void DrawRookMoves(int currentCol, int currentRow, Vector3 markerSize)
    {
        for (int i = 1; i < boardSize; i++)
        {
            DrawMove(currentCol, currentRow, i, 0, markerSize);
            DrawMove(currentCol, currentRow, -i, 0, markerSize);
            DrawMove(currentCol, currentRow, 0, i, markerSize);
            DrawMove(currentCol, currentRow, 0, -i, markerSize);
        }
    }

    private void DrawBishopMoves(int currentCol, int currentRow, Vector3 markerSize)
    {
        for (int i = 1; i < boardSize; i++)
        {
            DrawMove(currentCol, currentRow, i, i, markerSize);
            DrawMove(currentCol, currentRow, i, -i, markerSize);
            DrawMove(currentCol, currentRow, -i, i, markerSize);
            DrawMove(currentCol, currentRow, -i, -i, markerSize);
        }
    }

    private void DrawKingMoves(int currentCol, int currentRow, Vector3 markerSize)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                DrawMove(currentCol, currentRow, x, z, markerSize);
            }
        }
    }

    private void DrawKnightMoves(int currentCol, int currentRow, Vector3 markerSize)
    {
        for (int i = 0; i < KnightOffsets.Length; i++)
        {
            Vector2Int move = KnightOffsets[i];
            DrawMove(currentCol, currentRow, move.x, move.y, markerSize);
        }
    }

    private void DrawPawnMoves(int currentCol, int currentRow, Vector3 markerSize)
    {
        // White pawns move forward (+z), black pawns move backward (-z).
        int direction = pieceColor == ChessPieceColor.White ? 1 : -1;

        // Show one-step and two-step forward options.
        DrawMove(currentCol, currentRow, 0, direction, markerSize);
        DrawMove(currentCol, currentRow, 0, direction * 2, markerSize);

        // Show diagonal capture squares.
        DrawMove(currentCol, currentRow, -1, direction, markerSize);
        DrawMove(currentCol, currentRow, 1, direction, markerSize);
    }
}