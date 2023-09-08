namespace Framework.Template.AStarTemplate
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Linq;

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private int maxRow;
        [SerializeField] private int maxColumn;
        [SerializeField] private Dictionary<Vector2Int, TileData> mapTiles = new Dictionary<Vector2Int, TileData>();
        private Grid gridData;
        public delegate void GridGenerated(Dictionary<Vector2Int, TileData> mapTiles);
        public GridGenerated onGridGenerated;
        [SerializeField] private Vector3 offsetGrid;

        public int MaxRow { get => maxRow; }
        public int MaxColumn { get => maxColumn; }
        public Dictionary<Vector2Int, TileData> MapTiles { get => mapTiles; }

        private void Awake()
        {
            gridData = GetComponent<Grid>();
        }

        private void Start()
        {
            GenerateGrid();
        }

        #region GenerationGrid
        /// <summary>
        /// Generate the grid and instantiates the tile prefabs
        /// </summary>
        private void GenerateGrid()
        {
            for (int row = 0; row < maxRow; row++)
            {
                for (int column = 0; column < maxColumn; column++)
                {
                    var tile = Instantiate(tilePrefab, GetWorld3DPosition(new Vector2Int(row, column)), Quaternion.identity, transform);
                    tile.transform.localScale = gridData.cellSize;
                    tile.Initialize(this, row, column, true, tile);
                    tile.name = "Tile - (" + row.ToString() + " - " + column.ToString() + ")";
                    mapTiles[new Vector2Int(row, column)] = tile.data;
                }
            }
            if (onGridGenerated != null) onGridGenerated(mapTiles);
            CenterCamera();
        }

        #region GetWorld2DPosition
        /// <summary>
        /// Get the world position in 2D with vector
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 GetWorld2DPosition(Vector2Int position)
        {
            return new Vector2(position.x * (gridData.cellSize.x + gridData.cellGap.x), position.y * (gridData.cellSize.z + gridData.cellGap.z));
        }

        /// <summary>
        /// Get the world position in 2D with x and y positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 GetWorld2DPosition(int x, int y)
        {
            return new Vector2(x * (gridData.cellSize.x + gridData.cellGap.x), y * (gridData.cellSize.z + gridData.cellGap.z));
        }
        #endregion

        #region GetWorld3DPosition
        /// <summary>
        /// Get the world position in 3D with vector
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 GetWorld3DPosition(Vector2Int position)
        {
            return new Vector3(position.x * (gridData.cellSize.x + gridData.cellGap.x), 0, position.y * (gridData.cellSize.z + gridData.cellGap.z));
        }

        /// <summary>
        /// Get the world position in 3D with x and y positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetWorld3DPosition(int x, int y)
        {
            return new Vector3(x * (gridData.cellSize.x + gridData.cellGap.x), 0, y * (gridData.cellSize.z + gridData.cellGap.z));
        }
        #endregion

        /// <summary>
        /// Check if the coordinate is within the grid (false) or in the border of it (true)
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool CheckGridBounds(Vector2Int coordinates)
        {
            return (coordinates.x < 0 || coordinates.x >= maxRow || coordinates.y < 0 || coordinates.y >= maxColumn);
        }

        /// <summary>
        /// Check if the tile is walkable or not
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool CheckWalkable(Vector2Int coordinates)
        {
            return mapTiles[coordinates].walkable;
        }

        /// <summary>
        /// Center the camera
        /// </summary>
        private void CenterCamera()
        {
            Vector3 startGrid = GetWorld3DPosition(0, 0);
            Vector3 endGrid = GetWorld3DPosition(maxRow - 1, maxColumn - 1);

            Camera.main.transform.position = new Vector3((startGrid.x + endGrid.x) / 2, HeightCamera(), (startGrid.z + endGrid.z) / 2);
        }

        /// <summary>
        /// Set the camera at the right height
        /// </summary>
        /// <returns></returns>
        private float HeightCamera()
        {
            return maxColumn * (gridData.cellGap.z + gridData.cellSize.z) + (gridData.cellGap.y + gridData.cellSize.y) + 1;
        }
        #endregion

        #region GenerateRandomRowAndColumn
        /// <summary>
        /// Returns a random position within the grid (in Vector2)
        /// </summary>
        /// <param name="position"></param>
        public void GenerateRowAndColumnRandom(out Vector2Int position)
        {
            int randomRow = Random.Range(0, maxRow);
            int randomColumn = Random.Range(0, maxColumn);
            position = new Vector2Int(randomRow, randomColumn);
        }

        /// <summary>
        /// Returns a random position within the grid (splitted in rows and columns)
        /// </summary>
        /// <param name="randomRow"></param>
        /// <param name="randomColumn"></param>
        public void GenerateRowAndColumnRandom(out int randomRow, out int randomColumn)
        {
            randomRow = Random.Range(0, maxRow);
            randomColumn = Random.Range(0, maxColumn);
        }
        #endregion

    }
}