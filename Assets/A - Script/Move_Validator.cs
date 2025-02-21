using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveValidator : MonoBehaviour
{
    [Tooltip("Referencja do komponentu generuj�cego plansz� (HexGridGenerator)")]
    public HexGridGenerator gridGenerator;

    /// <summary>
    /// Sprawdza, czy ruch w zadanym kierunku (reprezentowanym wektorem axial) jest mo�liwy.
    /// Ruch jest mo�liwy, je�li:
    /// - Kafelek mo�e si� przesun�� do pustej kom�rki, lub
    /// - Kafelek mo�e po��czy� si� z s�siadem o tej samej warto�ci.
    /// </summary>
    /// <param name="direction">Wektor kierunku (axial)</param>
    /// <returns>True, je�li przynajmniej jeden ruch w tym kierunku jest mo�liwy, w przeciwnym razie false.</returns>
    public bool IsMovePossible(Vector2Int direction)
    {
        foreach (HexCell cell in gridGenerator.cells)
        {
            // Rozpatrujemy tylko kom�rki, kt�re maj� kafelek.
            if (cell.currentTile != null)
            {
                // Pobieramy bezpo�redniego s�siada w zadanym kierunku.
                HexCell neighbor = gridGenerator.GetCellAt(cell.q + direction.x, cell.r + direction.y);
                if (neighbor != null)
                {
                    // Je�li s�siad jest pusty, kafelek mo�e si� przesun��.
                    if (neighbor.currentTile == null)
                    {
                        return true;
                    }

                    // Je�li s�siad ma kafelek, sprawdzamy mo�liwo�� scalenia (merge).
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
    /// Sprawdza, czy ruch jest mo�liwy w kt�regokolwiek z sze�ciu kierunk�w.
    /// Przydatne np. do okre�lenia, czy gra powinna si� zako�czy�.
    /// </summary>
    /// <returns>True, je�li ruch jest mo�liwy w co najmniej jednym kierunku, w przeciwnym razie false.</returns>
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
