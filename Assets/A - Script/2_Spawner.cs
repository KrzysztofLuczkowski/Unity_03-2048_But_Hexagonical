using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    [Tooltip("Prefab kafelka z numerem 2")]
    public GameObject tile2Prefab;

    [Tooltip("Referencja do komponentu generuj�cego plansz�")]
    public HexGridGenerator gridGenerator;
    public void SpawnTile2()
    {
        // Tworzymy list� pustych p�l (tych, kt�re nie maj� jeszcze numerycznego kafelka)
        List<HexCell> emptyCells = new List<HexCell>();
        foreach (HexCell cell in gridGenerator.cells)
        {
            if (cell.currentTile == null)
            {
                emptyCells.Add(cell);
            }
        }

        if (emptyCells.Count == 0)
        {
            return;
        }

        // Wybieramy losowo jedno z pustych p�l
        int randomIndex = Random.Range(0, emptyCells.Count);
        HexCell chosenCell = emptyCells[randomIndex];

        // Ustawiamy pozycj� spawnu z ustawion� osi� Z = -1 (aby kafelek pojawi� si� na wierzchu)
        Vector3 spawnPosition = chosenCell.transform.position;
        spawnPosition.z = -1;

        // Tworzymy kafelek 2 na wybranym miejscu
        GameObject newTile = Instantiate(tile2Prefab, spawnPosition, Quaternion.identity, chosenCell.transform);

        // Oznaczamy pole jako zaj�te
        chosenCell.currentTile = newTile;
    }
}