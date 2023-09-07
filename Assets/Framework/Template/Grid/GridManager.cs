namespace Framework.Template.GridTemplate
{
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Grid))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Tile m_TilePrefab;
        [SerializeField] private int m_MaxRow;
        [SerializeField] private int m_MaxColumn;
        [SerializeField] private Dictionary<Vector2Int, TileData> m_MapTiles = new Dictionary<Vector2Int, TileData>();
        private Grid m_GridData;
        public delegate void GridGenerated(Dictionary<Vector2Int, TileData> mapTiles);
        public GridGenerated onGridGenerated;
        [SerializeField] private Vector3 m_OffsetGrid;

        public int MaxRow { get => m_MaxRow; }
        public int MaxColumn { get => m_MaxColumn; }
        public Dictionary<Vector2Int, TileData> MapTiles { get => m_MapTiles; }

        private void Awake()
        {
            m_GridData = GetComponent<Grid>();
        }

        private void Start()
        {
            GenerateGrid();
        }

        /// <summary>
        /// Generate the grid and instantiates the tile prefabs
        /// </summary>
        #region GenerationGrid
        private void GenerateGrid()
        {
            for (int row = 0; row < m_MaxRow; row++)
            {
                for (int column = 0; column < m_MaxColumn; column++)
                {
                    var tile = Instantiate(m_TilePrefab, GetWorld3DPosition(new Vector2Int(row, column)), Quaternion.identity, transform);
                    tile.transform.localScale = m_GridData.cellSize;
                    tile.Initialize(this, row, column, true, tile);
                    tile.name = "Tile - (" + row.ToString() + " - " + column.ToString() + ")";
                    m_MapTiles[new Vector2Int(row, column)] = tile.Data;
                }
            }
            if (onGridGenerated != null) onGridGenerated(m_MapTiles);
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
            return new Vector2(position.x * (m_GridData.cellSize.x + m_GridData.cellGap.x), position.y * (m_GridData.cellSize.z + m_GridData.cellGap.z));
        }

        /// <summary>
        /// Get the world position in 2D with x and y positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 GetWorld2DPosition(int x, int y)
        {
            return new Vector2(x * (m_GridData.cellSize.x + m_GridData.cellGap.x), y * (m_GridData.cellSize.z + m_GridData.cellGap.z));
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
            return new Vector3(position.x * (m_GridData.cellSize.x + m_GridData.cellGap.x), 0, position.y * (m_GridData.cellSize.z + m_GridData.cellGap.z));
        }

        /// <summary>
        /// Get the world position in 3D with x and y positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetWorld3DPosition(int x, int y)
        {
            return new Vector3(x * (m_GridData.cellSize.x + m_GridData.cellGap.x), 0, y * (m_GridData.cellSize.z + m_GridData.cellGap.z));
        }
        #endregion

        /// <summary>
        /// Check if the coordinate is within the grid (false) or in the border of it (true)
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool CheckGridBounds(Vector2Int coordinates)
        {
            return (coordinates.x < 0 || coordinates.x >= m_MaxRow || coordinates.y < 0 || coordinates.y >= m_MaxColumn);
        }

        /// <summary>
        /// Check if the tile is walkable or not
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool CheckWalkable(Vector2Int coordinates)
        {
            return m_MapTiles[coordinates].Walkable;
        }

        /// <summary>
        /// Center the camera
        /// </summary>
        private void CenterCamera()
        {
            Vector3 startGrid = GetWorld3DPosition(0, 0);
            Vector3 endGrid = GetWorld3DPosition(m_MaxRow - 1, m_MaxColumn - 1);

            Camera.main.transform.position = new Vector3((startGrid.x + endGrid.x) / 2, HeightCamera(), (startGrid.z + endGrid.z) / 2);
        }

        /// <summary>
        /// Set the camera at the right height
        /// </summary>
        /// <returns></returns>
        private float HeightCamera()
        {
            return m_MaxColumn * (m_GridData.cellGap.z + m_GridData.cellSize.z) + (m_GridData.cellGap.y + m_GridData.cellSize.y) + 1;
        }
        #endregion

        #region GenerateRandomRowAndColumn
        /// <summary>
        /// Returns a random position within the grid (in Vector2)
        /// </summary>
        /// <param name="position"></param>
        public void GenerateRowAndColumnRandom(out Vector2Int position)
        {
            int randomRow = Random.Range(0, m_MaxRow);
            int randomColumn = Random.Range(0, m_MaxColumn);
            position = new Vector2Int(randomRow, randomColumn);
        }

        /// <summary>
        /// Returns a random position within the grid (splitted in rows and columns)
        /// </summary>
        /// <param name="randomRow"></param>
        /// <param name="randomColumn"></param>
        public void GenerateRowAndColumnRandom(out int randomRow, out int randomColumn)
        {
            randomRow = Random.Range(0, m_MaxRow);
            randomColumn = Random.Range(0, m_MaxColumn);
        }
        #endregion

    }
}
