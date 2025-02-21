using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    [Tooltip("Prefab kafelka z numerem 2")]
    public GameObject tile2Prefab;

    [Tooltip("Referencja do komponentu generuj¹cego planszê")]
    public HexGridGenerator gridGenerator;
    public void SpawnTile2()
    {
        // Tworzymy listê pustych pól (tych, które nie maj¹ jeszcze numerycznego kafelka)
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

        // Wybieramy losowo jedno z pustych pól
        int randomIndex = Random.Range(0, emptyCells.Count);
        HexCell chosenCell = emptyCells[randomIndex];

        // Ustawiamy pozycjê spawnu z ustawion¹ osi¹ Z = -1 (aby kafelek pojawi³ siê na wierzchu)
        Vector3 spawnPosition = chosenCell.transform.position;
        spawnPosition.z = -1;

        // Tworzymy kafelek 2 na wybranym miejscu
        GameObject newTile = Instantiate(tile2Prefab, spawnPosition, Quaternion.identity, chosenCell.transform);

        // Oznaczamy pole jako zajête
        chosenCell.currentTile = newTile;
    }
}