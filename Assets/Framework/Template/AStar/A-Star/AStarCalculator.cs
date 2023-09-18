namespace Framework.Template.AStarTemplate
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(TokenGenerator))]
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(GridManager))]
    public class AStarCalculator 
    {
        private Directions m_Directions = new Directions();

        [Header("Lists")]
        private List<TileData> m_OpenList = new List<TileData>();
        private List<TileData> m_ClosedList = new List<TileData>();
        private List<Vector3> m_PathPositions = new List<Vector3>();

        [Header("KeyCodes")]
        [SerializeField] private KeyCode m_StartPathCalculationKeycode;
        [SerializeField] private KeyCode m_StartMovePlayerKeycode;
        [SerializeField] private KeyCode m_StartPrefabPositionKeycode;

        [Header("Vector2Int")]
        [SerializeField] private Vector2Int m_StartCoordinates;
        [SerializeField] private Vector2Int m_EndCoordinates;
        [SerializeField] private Vector2Int m_LastCoordinates;

        private GridManager m_GridManager;
        private TokenGenerator m_TokenGenerator;

        //void Start()
        //{
        //    TryGetComponent(out m_GridManager);
        //    TryGetComponent(out m_TokenGenerator);
        //}

        /// <summary>
        /// Calculate the path and traces with line renderer at the end
        /// </summary>
        /// <param name="actualData"></param>
        private void SearchNextStepWhile(TileData actualData)
        {
            while (actualData.ToVector() != m_EndCoordinates)
            {
                foreach (Vector2Int direction in m_Directions.directions)
                {
                    Vector2Int neighbourCell = new Vector2Int(actualData.row + direction.x, actualData.column + direction.y);
                    if (IsClosed(m_GridManager.MapTiles[neighbourCell])) 
                        continue;
                    if (m_GridManager.CheckGridBounds(neighbourCell)) 
                        continue;
                    if (!m_GridManager.CheckWalkable(neighbourCell)) 
                        continue;

                    float g = actualData.aStarData.g + Vector2.Distance(m_GridManager.GetWorld2DPosition(actualData.ToVector()), m_GridManager.GetWorld2DPosition(neighbourCell));
                    float h = Vector2.Distance(m_GridManager.GetWorld2DPosition(neighbourCell), m_GridManager.GetWorld2DPosition(m_EndCoordinates));
                    float f = g + h;

                    if (!UpdateTileData(m_GridManager.MapTiles[neighbourCell], g, h, f, actualData))
                    {
                        AddToOpenList(neighbourCell);
                        m_GridManager.MapTiles[neighbourCell].aStarData.SetupAStarData(g, h, f, actualData);
                    }
                }

                m_OpenList = m_OpenList.OrderBy(tileData => tileData.aStarData.f).ThenBy(tileData => tileData.aStarData.h).ToList();

                //se open list è vuota termina, vicolo cieco
                if (m_OpenList.Count == 0)
                {
                    Debug.Log("Dead end");
                    return;
                }
                TileData firstData = m_OpenList.ElementAt(0);
                AddToClosedList(firstData.ToVector());
                m_OpenList.RemoveAt(0);
                actualData = firstData;
            }
            TracePath();
        }

        /// <summary>
        /// Updates tile's informations
        /// </summary>
        /// <param name="data"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="f"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private bool UpdateTileData(TileData data, float g, float h, float f, TileData parent)
        {
            foreach (TileData tileData in m_OpenList)
            {
                if (tileData == data)
                {
                    tileData.aStarData.g = g;
                    tileData.aStarData.h = h;
                    tileData.aStarData.f = f;
                    tileData.aStarData.parent = parent;
                    return true;
                }
            }
            return false;
        }

        private bool IsClosed(TileData data)
        {
            return m_ClosedList.Contains(data);
        }

        /// <summary>
        /// Adds to open list and changes material
        /// </summary>
        /// <param name="coordinates"></param>
        private void AddToOpenList(Vector2Int coordinates)
        {
            m_OpenList.Add(m_GridManager.MapTiles[coordinates]);
        }

        /// <summary>
        /// Adds to closed list and changes material
        /// </summary>
        /// <param name="coordinates"></param>
        private void AddToClosedList(Vector2Int coordinates)
        {
            m_ClosedList.Add(m_GridManager.MapTiles[coordinates]);
        }

        /// <summary>
        /// Reset of tiles to normal
        /// </summary>
        /// <param name="coordinates"></param>
        public void ReturnToNormalTiles(Vector2Int coordinates)
        {
            if (m_OpenList.Contains(m_GridManager.MapTiles[coordinates]))
            {
                m_OpenList.Remove(m_GridManager.MapTiles[coordinates]);
            }

            if (m_ClosedList.Contains(m_GridManager.MapTiles[coordinates]))
            {
                m_ClosedList.Remove(m_GridManager.MapTiles[coordinates]);
            }
        }

        /// <summary>
        /// Line renderer of the path
        /// </summary>
        private void TracePath()
        {
            m_PathPositions.Clear();
            var actualPosition = m_EndCoordinates;
            while (actualPosition != m_StartCoordinates)
            {
                m_PathPositions.Add(m_GridManager.GetWorld3DPosition(m_GridManager.MapTiles[actualPosition].ToVector()));
                actualPosition = m_GridManager.MapTiles[actualPosition].aStarData.parent.ToVector();
            }
            m_PathPositions.Reverse();
        }


        /// <summary>
        /// Interpolate player's position to end token position 
        /// </summary>
        /// <returns></returns>
        private IEnumerator MovePlayer()
        {
            int index = 0;
            GameObject player = new();
            player.transform.position = m_TokenGenerator.Tokens[0];
            while (index < m_PathPositions.Count)
            {
                player.transform.Translate(GetDirection(player.transform.position, m_PathPositions[index]).normalized / 10f);
                if (Vector3.Distance(player.transform.position, m_PathPositions[index]) < 0.1) index++;
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Reached end");
        }

        private Vector3 GetDirection(Vector3 from, Vector3 to)
        {
            return to - from;
        }

        /// <summary>
        /// Finds random position of start and end and spawns token 
        /// </summary>
        private void SetStartEndPosition()
        {
            FindRandomPosition(ref m_StartCoordinates);
            m_TokenGenerator.SpawnToken(m_StartCoordinates);
            SetupStartCoordinates();
            do
            {
                FindRandomPosition(ref m_EndCoordinates);
            } while (m_EndCoordinates == m_StartCoordinates);

            m_TokenGenerator.SpawnToken(m_EndCoordinates);
        }

        /// <summary>
        /// Setups the start and last coordinates
        /// </summary>
        private void SetupStartCoordinates()
        {
            m_LastCoordinates = m_StartCoordinates;
            AddToClosedList(m_StartCoordinates);
        }

        /// <summary>
        /// Finds random position to assign to a specific tile and for  
        /// </summary>
        /// <param name="tilePositionSelected"></param>
        private void FindRandomPosition(ref Vector2Int tilePositionSelected)
        {
            bool foundPosition = false;
            int numberOfTiles = m_GridManager.MaxColumn * m_GridManager.MaxRow;
            List<Vector2Int> coordinatesExamined = new List<Vector2Int>();

            while (numberOfTiles > 0 && !foundPosition)
            {
                m_GridManager.GenerateRowAndColumnRandom(out Vector2Int positionOnGrid);
                //GenerateRowAndColumnRandom(out int rowGrid, out int columnGrid);
                numberOfTiles--;
                if (coordinatesExamined.Contains(positionOnGrid)) continue;
                coordinatesExamined.Add(positionOnGrid);
                if (m_GridManager.CheckWalkable(positionOnGrid))
                {
                    foundPosition = true;
                    tilePositionSelected = positionOnGrid;
                }
            }
        }
    }
}

