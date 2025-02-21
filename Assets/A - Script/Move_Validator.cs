using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveValidator : MonoBehaviour
{
    [Tooltip("Referencja do komponentu generuj¹cego planszê (HexGridGenerator)")]
    public HexGridGenerator gridGenerator;

    /// <summary>
    /// Sprawdza, czy ruch w zadanym kierunku (reprezentowanym wektorem axial) jest mo¿liwy.
    /// Ruch jest mo¿liwy, jeœli:
    /// - Kafelek mo¿e siê przesun¹æ do pustej komórki, lub
    /// - Kafelek mo¿e po³¹czyæ siê z s¹siadem o tej samej wartoœci.
    /// </summary>
    /// <param name="direction">Wektor kierunku (axial)</param>
    /// <returns>True, jeœli przynajmniej jeden ruch w tym kierunku jest mo¿liwy, w przeciwnym razie false.</returns>
    public bool IsMovePossible(Vector2Int direction)
    {
        foreach (HexCell cell in gridGenerator.cells)
        {
            // Rozpatrujemy tylko komórki, które maj¹ kafelek.
            if (cell.currentTile != null)
            {
                // Pobieramy bezpoœredniego s¹siada w zadanym kierunku.
                HexCell neighbor = gridGenerator.GetCellAt(cell.q + direction.x, cell.r + direction.y);
                if (neighbor != null)
                {
                    // Jeœli s¹siad jest pusty, kafelek mo¿e siê przesun¹æ.
                    if (neighbor.currentTile == null)
                    {
                        return true;
                    }

                    // Jeœli s¹siad ma kafelek, sprawdzamy mo¿liwoœæ scalenia (merge).
                    int currentValue = cell.currentTile.GetComponent<Tile>().value;
                    int neighborValue = neighbor.currentTile.GetComponent<Tile>().value;
                    if (currentValue == neighborValue)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Sprawdza, czy ruch jest mo¿liwy w któregokolwiek z szeœciu kierunków.
    /// Przydatne np. do okreœlenia, czy gra powinna siê zakoñczyæ.
    /// </summary>
    /// <returns>True, jeœli ruch jest mo¿liwy w co najmniej jednym kierunku, w przeciwnym razie false.</returns>
    public bool IsAnyMovePossible()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, -1),  // Northwest
        new Vector2Int(1, -1),  // Northeast
        new Vector2Int(1, 0),   // East
        new Vector2Int(0, 1),   // Southeast
        new Vector2Int(-1, 1),  // Southwest
        new Vector2Int(-1, 0)   // West
        };

        foreach (Vector2Int dir in directions)
        {
            if (IsMovePossible(dir))
            {
                return true;
            }
        }

        return false;
    }
}
