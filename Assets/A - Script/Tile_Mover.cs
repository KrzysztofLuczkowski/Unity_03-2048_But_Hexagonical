using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    [Tooltip("Referencja do komponentu generuj¹cego planszê")]
    public HexGridGenerator gridGenerator;

    [Tooltip("Lista prefabrykatów kafelków, ustawiona rosn¹co (2, 4, 8, 16, ...)")]
    public List<GameObject> tilePrefabs;

    [Tooltip("Referencja do komponentu do spawnowania nowych kafelków")]
    public TileSpawner tileSpawner;

    [Tooltip("Czas trwania animacji przesuniêcia pomiêdzy kolejnymi komórkami")]
    public float segmentDuration = 0.1f;

    [Tooltip("Referencja do komponentu, który sprawdza mo¿liwoœæ ruchu")]
    public MoveValidator moveValidator;

    [Tooltip("Referencja do komponentu, który sprawdza warunki przegranej")]
    public GameOverChecker gameOverChecker;

    private bool isMoving = false;
    public GameUI gameUI;

    void Update()
    {
        if (isMoving)
            return;

        Vector2Int? direction = null;

        if (Input.GetKeyDown(KeyCode.S))
            direction = new Vector2Int(0, -1);     // Northwest
        else if (Input.GetKeyDown(KeyCode.D))
            direction = new Vector2Int(1, -1);     // Northeast
        else if (Input.GetKeyDown(KeyCode.E))
            direction = new Vector2Int(1, 0);      // East
        else if (Input.GetKeyDown(KeyCode.W))
            direction = new Vector2Int(0, 1);      // Southeast
        else if (Input.GetKeyDown(KeyCode.Q))
            direction = new Vector2Int(-1, 1);     // Southwest
        else if (Input.GetKeyDown(KeyCode.A))
            direction = new Vector2Int(-1, 0);     // West

        if (direction.HasValue && moveValidator.IsMovePossible(direction.Value))
        {
            StartCoroutine(MoveTiles(direction.Value));
            gameUI.IncrementMoves();
            gameUI.PlayMoveSound();
        }
    }

    // Klasa pomocnicza przechowuj¹ca dane o ruchu kafelka
    private class TileMoveCommand
    {
        public GameObject tile;
        public List<Vector3> pathPositions;
        public HexCell originCell;
        public HexCell targetCell;
        public bool isMerge;
        public HexCell mergeTarget;
        public int tileValue;
    }

    // Coroutine obliczaj¹ca ruchy, animuj¹ca przesuniêcia oraz aktualizuj¹ca stan planszy
    IEnumerator MoveTiles(Vector2Int direction)
    {
        isMoving = true;
        List<TileMoveCommand> moveCommands = new List<TileMoveCommand>();
        HashSet<HexCell> mergedCells = new HashSet<HexCell>();

        // Pobieramy wszystkie komórki z kafelkami
        List<HexCell> cellsWithTiles = new List<HexCell>();
        foreach (HexCell cell in gridGenerator.cells)
        {
            if (cell.currentTile != null)
                cellsWithTiles.Add(cell);
        }

        // Sortujemy komórki – te bardziej wysuniête w kierunku ruchu bêd¹ przetwarzane jako pierwsze
        cellsWithTiles.Sort((a, b) =>
        {
            int aPriority = a.q * direction.x + a.r * direction.y;
            int bPriority = b.q * direction.x + b.r * direction.y;
            return bPriority.CompareTo(aPriority);
        });

        // Przechodzimy po ka¿dej komórce, symuluj¹c ruch i natychmiast aktualizuj¹c stan zajêtoœci
        foreach (HexCell cell in cellsWithTiles)
        {
            if (cell.currentTile == null || mergedCells.Contains(cell))
                continue;

            GameObject movingTile = cell.currentTile;
            int tileValue = movingTile.GetComponent<Tile>().value;
            HexCell currentCell = cell;
            List<Vector3> pathPositions = new List<Vector3>();
            // Dodajemy pozycjê pocz¹tkow¹ z ustalonym Z = -1
            Vector3 startPos = new Vector3(currentCell.transform.position.x, currentCell.transform.position.y, -1);
            pathPositions.Add(startPos);
            HexCell mergeTarget = null;

            // Przesuwamy siê "krok po kroku" w zadanym kierunku, obliczaj¹c œcie¿kê
            while (true)
            {
                int nextQ = currentCell.q + direction.x;
                int nextR = currentCell.r + direction.y;
                HexCell nextCell = gridGenerator.GetCellAt(nextQ, nextR);
                if (nextCell == null)
                    break;

                if (nextCell.currentTile != null)
                {
                    int nextTileValue = nextCell.currentTile.GetComponent<Tile>().value;
                    // Jeœli trafimy na kafelek o tej samej wartoœci i nie by³ jeszcze ³¹czony, ustawiamy merge
                    if (nextTileValue == tileValue && !mergedCells.Contains(nextCell))
                    {
                        mergeTarget = nextCell;
                    }
                    break;
                }
                else
                {
                    Vector3 nextPos = new Vector3(nextCell.transform.position.x, nextCell.transform.position.y, -1);
                    pathPositions.Add(nextPos);
                    currentCell = nextCell;
                }
            }

            // Jeœli mo¿liwe jest ³¹czenie
            if (mergeTarget != null)
            {
                TileMoveCommand cmd = new TileMoveCommand();
                cmd.tile = movingTile;
                cmd.pathPositions = new List<Vector3>(pathPositions);
                cmd.originCell = cell;
                cmd.targetCell = mergeTarget;
                cmd.isMerge = true;
                cmd.mergeTarget = mergeTarget;
                cmd.tileValue = tileValue;
                moveCommands.Add(cmd);

                mergedCells.Add(mergeTarget);
                // Aktualizujemy stan: komórka pocz¹tkowa staje siê pusta
                cell.currentTile = null;
            }
            // Jeœli kafelek mo¿e siê przesun¹æ, ale nie nastêpuje merge
            else if (currentCell != cell)
            {
                TileMoveCommand cmd = new TileMoveCommand();
                cmd.tile = movingTile;
                cmd.pathPositions = new List<Vector3>(pathPositions);
                cmd.originCell = cell;
                cmd.targetCell = currentCell;
                cmd.isMerge = false;
                cmd.tileValue = tileValue;
                moveCommands.Add(cmd);

                // Aktualizujemy stan – zwalniamy komórkê pocz¹tkow¹ i zajmujemy docelow¹
                cell.currentTile = null;
                currentCell.currentTile = movingTile;
            }
        }

        // Uruchamiamy animacje dla wszystkich ruchów jednoczeœnie
        int animationsRunning = moveCommands.Count;
        foreach (TileMoveCommand cmd in moveCommands)
        {
            StartCoroutine(AnimateTile(cmd.tile, cmd.pathPositions, segmentDuration, () => { animationsRunning--; }));
        }
        yield return new WaitUntil(() => animationsRunning == 0);

        // Po zakoñczeniu animacji aktualizujemy ostateczny stan planszy
        foreach (TileMoveCommand cmd in moveCommands)
        {
            if (cmd.originCell != null)
                cmd.originCell.currentTile = null;

            if (cmd.isMerge)
            {
                if (cmd.tile != null)
                    Destroy(cmd.tile);
                if (cmd.mergeTarget != null && cmd.mergeTarget.currentTile != null)
                    Destroy(cmd.mergeTarget.currentTile);

                GameObject newTilePrefab = GetNextTilePrefab(cmd.tileValue);
                if (newTilePrefab != null && cmd.mergeTarget != null)
                {
                    GameObject newTile = Instantiate(newTilePrefab, cmd.mergeTarget.transform.position, Quaternion.identity, cmd.mergeTarget.transform);
                    newTile.transform.position = new Vector3(newTile.transform.position.x, newTile.transform.position.y, -1);
                    cmd.mergeTarget.currentTile = newTile;
                    gameUI.AddScore(cmd.tileValue * 2);
                }
            }
            else
            {
                if (cmd.targetCell != null)
                {
                    cmd.targetCell.currentTile = cmd.tile;
                    cmd.tile.transform.position = new Vector3(cmd.targetCell.transform.position.x, cmd.targetCell.transform.position.y, -1);
                }
            }
        }


        tileSpawner.SpawnTile2();
        tileSpawner.SpawnTile2();
        gameOverChecker.CheckGameOver();
        isMoving = false;
    }

    // Coroutine animuj¹ca przesuwanie pojedynczego kafelka po wyliczonej œcie¿ce
    IEnumerator AnimateTile(GameObject tile, List<Vector3> path, float segmentDuration, System.Action onComplete)
    {
        if (path.Count < 2)
        {
            onComplete?.Invoke();
            yield break;
        }

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 start = path[i - 1];
            Vector3 end = path[i];
            float elapsed = 0f;
            while (elapsed < segmentDuration)
            {
                tile.transform.position = Vector3.Lerp(start, end, elapsed / segmentDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            tile.transform.position = end;
        }
        onComplete?.Invoke();
    }

    GameObject GetNextTilePrefab(int currentValue)
    {
        for (int i = 0; i < tilePrefabs.Count - 1; i++)
        {
            Tile tileComponent = tilePrefabs[i].GetComponent<Tile>();
            if (tileComponent != null && tileComponent.value == currentValue)
            {
                return tilePrefabs[i + 1];
            }
        }
        return null;
    }
}
