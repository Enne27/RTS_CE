using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
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
    private int minDistance = 1;
    #endregion

    private void Start()
    {
        cellSize = FlowField_Manager.Instance.cellRadius;
        minDistance = mapResources.minDistance;

        GenerateMap();
    }

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
                gridCells[i,j] = new CELL_STATE();
            }
        }

        List<Vector2Int> selectedCells = new List<Vector2Int>();
        List<Vector2Int> invalidCells = new List<Vector2Int>();


        Vector2Int GetRandomCell()
        {
            Vector2Int randomCell;
            do
            {
                randomCell = new Vector2Int(Random.Range(0, gridSize.x-1), Random.Range(0, gridSize.y-1));

            } while (selectedCells.Contains(randomCell) || invalidCells.Contains(randomCell));

            selectedCells.Add(randomCell);

            return randomCell;
        }

        for (int i = 0; i < mapResources.antHillQuantity; i++)
        {
            Vector2Int pos = GetRandomCell();
            gridCells[pos.x, pos.y] = CELL_STATE.OCUPPIED;

            Vector3 cellPosWorld = CellToWorld(pos);
            //Debug.Log("Hills: " + pos.x + ", " + pos.y);

            mapResources.InstantiateAntHill(cellPosWorld);
        }

        for (int i = 0; i < mapResources.resourcesQuantity; i++)
        {
            Vector2Int pos = GetRandomCell();
            gridCells[pos.x, pos.y] = CELL_STATE.OCUPPIED;

            Vector3 cellPosWorld = CellToWorld(pos);

            //Debug.Log("Resources: " + pos.x + ", " + pos.y);

            mapResources.InstantiateResourcesZone(cellPosWorld);
        }

    }

    /// <summary>
    /// Pasar el tamaño de las celdas a tamaño de mundo.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * cellSize, 0, cell.y * cellSize);
    }

    /// <summary>
    /// Asegurar una distancia mínima entre recursos para que todos los jugadores tengan las mismas posibilidades.
    /// </summary>
    private void DistanceInstantiation()
    {

    }
}
