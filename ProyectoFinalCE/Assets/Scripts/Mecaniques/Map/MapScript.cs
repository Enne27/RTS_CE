using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    #region VARIABLES
    [Header("Map/Grid Structure")]
    [Tooltip("Matriz de dos dimensiones que contiene el estado de la celda.")]
    public CELL_STATE[,] gridCells; // Dos dimensiones

    [Tooltip("Tamaño del grid.")]
    public Vector2Int gridSize = new Vector2Int(25,25);

    [Tooltip("Diferentes estados de las celdas.")]
    public enum CELL_STATE
    {
        EMPTY, // Celda vacia
        OCUPPIED, // Celda ocupada por un edificio
        IMPOSSIBLE // Celda no edificable ni posible de caminar por ella
    }

    [Header("Resources and parameters")]
    [Tooltip("Script que contiene los datos iniciales del mapa.")]
    [SerializeField] GenerateMapResources mapResources;

    private float cellSize = 1f;

    List<Vector2Int> selectedCells = new List<Vector2Int>();
    List<Vector2Int> invalidCells = new List<Vector2Int>();
    List<Vector2Int> antHillsPositions = new List<Vector2Int>();
    #endregion

    private void Start()
    {
        cellSize = FlowField_Manager.Instance.cellRadius;

        GenerateMap();
    }

    #region RANDOM_CELLS
    /// <summary>
    /// Encontrar una celda válida.
    /// </summary>
    /// <returns></returns>
    private Vector2Int GetRandomCell(int minDistance)
    {
        Vector2Int randomCell;
        int attempts = 0;
        do
        {
            randomCell = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));

            attempts++;

            if (attempts > 1000) // evitemos bucles infinitos
            {
                break;
            }

        } while (selectedCells.Contains(randomCell) || 
            invalidCells.Contains(randomCell) || 
            !IsValidCellDistance(randomCell, selectedCells, minDistance));

        return randomCell;
    }

    /// <summary>
    /// Encontrar una celda aleatoria, pero dentro de un rango.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="minRadius">Distancia mínima entre recursos.</param>
    /// <param name="maxRadius">Límite del mapa o rango.</param>
    /// <returns></returns>
    private Vector2Int GetRandomCellAround(Vector2Int center, int minRadius, int maxRadius)
    {
        int minX = Mathf.Max(0, center.x - minRadius);
        int maxX = Mathf.Min(gridSize.x - 1, center.x + maxRadius);

        int minY = Mathf.Max(0, center.y - minRadius);
        int maxY = Mathf.Min(gridSize.y - 1, center.y + maxRadius);

        return new Vector2Int(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1));
    }
    #endregion

    /// <summary>
    /// Pasar el tamaño de las celdas a tamaño(Vector3 position) de mundo.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private Vector3 CellToWorldPosition(Vector2Int cell)
    {
        return new Vector3((cell.x -gridSize.x /2) * cellSize * 2, 0, (cell.y - gridSize.y / 2) * cellSize * 2);
    }

    /// <summary>
    /// Asegurar una distancia mínima entre recursos para que todos los jugadores tengan las mismas posibilidades.
    /// </summary>
    private bool IsValidCellDistance(Vector2Int candidate, List<Vector2Int> selectedCells, int minDistance)
    {
        foreach(var cell in selectedCells)
        {
            float distance = Vector2Int.Distance(candidate, cell);
            if (distance < minDistance)
                return false;
        }
        return true;
    }

    #region GENERATION
    /// <summary>
    /// Método para generar el grid del mapa.
    /// </summary>
    private void GenerateMap()
    {
        gridCells = new CELL_STATE[gridSize.x, gridSize.y];

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++) 
            {
                gridCells[i,j] = CELL_STATE.EMPTY;
            }
        }

        GenerateAntHills();
        GenerateResourcesZones();
    }

    /// <summary>
    /// Buscar una celda valida, ocuparla y generar el hormiguero.
    /// </summary>
    private void GenerateAntHills()
    {
        for (int i = 0; i < mapResources.antHillQuantity; i++)
        {
            Vector2Int pos = GetRandomCell(mapResources.minDistanceAntHill);

            gridCells[pos.x, pos.y] = CELL_STATE.OCUPPIED;

            selectedCells.Add(pos);
            antHillsPositions.Add(pos);

            mapResources.InstantiateAntHill(CellToWorldPosition(pos));
            //Debug.Log("Hills: " + pos.x + ", " + pos.y);
        }
    }

    /// <summary>
    /// Generar todas las zonas de recursos dividiendo más o menos entre todos los hormigueros.
    /// </summary>
    private void GenerateResourcesZones()
    {
        int hillsCount = mapResources.antHillQuantity;
        int baseResourcesEachAntHill = mapResources.resourcesQuantity / hillsCount;
        int extraResource = mapResources.resourcesQuantity % hillsCount;

        for (int i = 0; i < hillsCount; i++)
        {
            int amount = baseResourcesEachAntHill + (i < extraResource ? 1 : 0);
            GenerateResourcesZonesAroundHill(antHillsPositions[i], amount);
        }
    }

    /// <summary>
    /// Generar la zona de recursos alrededor del hormiguero.
    /// </summary>
    /// <param name="hillCell"></param>
    /// <param name="amount"></param>
    private void GenerateResourcesZonesAroundHill(Vector2Int hillCell, int amount)
    {
        int minRadius = mapResources.minDistanceResources;
        int maxRadius = gridSize.x; // Cualquier lugar dentro del mapa.

        int generated = 0;
        int attempts = 0; // Evitar bucles infinitos.

        while (generated < amount && attempts < 5000)
        {
            attempts++;

            Vector2Int cellPos = GetRandomCellAround(hillCell, minRadius, maxRadius);
            float distanceToCenter = Vector2Int.Distance(cellPos, hillCell);

            if (distanceToCenter < minRadius || distanceToCenter > maxRadius)
                continue;

            // Esta celda ya está ocupada.
            if (gridCells[cellPos.x, cellPos.y] != CELL_STATE.EMPTY)
                continue;

            // Se intentan instanciar demasiado cerca.
            if (!IsValidCellDistance(cellPos, selectedCells, mapResources.minDistanceResources))
                continue;

            gridCells[cellPos.x, cellPos.y] = CELL_STATE.OCUPPIED;
            selectedCells.Add(cellPos);
            mapResources.InstantiateResourcesZone(CellToWorldPosition(cellPos));

            generated++;
        }
    }
    #endregion
}