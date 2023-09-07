namespace Framework.Template.AStarTemplate
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(TokenGenerator))]
    [RequireComponent(typeof(LineRenderer))]
    public class AStarCalculator : MonoBehaviour
    {
        private Directions m_Direction = new Directions();
        private List<TileData> m_OpenList = new List<TileData>();
        private List<TileData> m_ClosedList = new List<TileData>();
        private LineRenderer m_LineRenderer;
        private List<Vector3> m_PathPositions = new List<Vector3>();
        private bool m_PathSearching = false;
        private bool m_PathSearched = false;
        [SerializeField] private KeyCode m_StartPathCalculationKeycode;
        [SerializeField] private KeyCode m_StartMovePlayerKeycode;
        [SerializeField] private KeyCode m_StartPrefabPositionKeycode;
        private GridManager m_GridManager;
        [SerializeField] private Vector2Int m_StartCoordinates;
        [SerializeField] private Vector2Int m_EndCoordinates;
        [SerializeField] private Vector2Int m_LastCoordinates;
        private TokenGenerator m_TokenGenerator;

        // Start is called before the first frame update
        void Start()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            TryGetComponent(out m_GridManager);
            TryGetComponent(out m_TokenGenerator);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualData"></param>
        private void SearchNextStepWhile(TileData actualData)
        {
            m_PathSearching = true;
            while (actualData.ToVector() != m_EndCoordinates)
            {
                foreach (Vector2Int direction in m_Direction.directions)
                {
                    Vector2Int neighbourCell = new Vector2Int(actualData.row + direction.x, actualData.column + direction.y);
                    if (IsClosed(m_GridManager.MapTiles[neighbourCell])) continue;
                    if (m_GridManager.CheckGridBounds(neighbourCell)) continue;
                    if (!m_GridManager.CheckWalkable(neighbourCell)) continue;

                    float g = actualData.aStarData.g + Vector2.Distance(m_GridManager.GetWorld2DPosition(actualData.ToVector()), m_GridManager.GetWorld2DPosition(neighbourCell));
                    float h = Vector2.Distance(m_GridManager.GetWorld2DPosition(neighbourCell), m_GridManager.GetWorld2DPosition(m_EndCoordinates));
                    float f = g + h;

                    //WIP
                    if (!UpdateTileData(m_GridManager.MapTiles[neighbourCell], g, h, f, actualData))
                    {
                        AddToOpenList(neighbourCell);
                        //openList.Add(mapTiles[neighbourCell]);
                        m_GridManager.MapTiles[neighbourCell].aStarData.SetupAStarData(g, h, f, actualData);
                        //mapTiles[neighbourCell].ChangeMaterial(mapTiles[neighbourCell].tile.open);
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
        /// 
        /// </summary>
        /// <param name="actualData"></param>
        private void SearchNextStep(TileData actualData)
        {
            if (actualData.ToVector() == m_EndCoordinates)
            {
                TracePath();
                return;
            }

            foreach (Vector2Int direction in m_Direction.directions)
            {
                Vector2Int neighbourCell = new Vector2Int(actualData.row + direction.x, actualData.column + direction.y);
                if (IsClosed(m_GridManager.MapTiles[neighbourCell])) continue;
                if (m_GridManager.CheckGridBounds(neighbourCell)) continue;
                if (!m_GridManager.CheckWalkable(neighbourCell)) continue;

                float g = actualData.aStarData.g + Vector2.Distance(m_GridManager.GetWorld2DPosition(actualData.ToVector()), m_GridManager.GetWorld2DPosition(neighbourCell));
                float h = Vector2.Distance(m_GridManager.GetWorld2DPosition(neighbourCell), m_GridManager.GetWorld2DPosition(m_EndCoordinates));
                float f = g + h;

                //setup TextMeshPro
                m_GridManager.MapTiles[neighbourCell].tile.UiManager.SetFGHValues(f, g, h);

                //WIP
                if (!UpdateTileData(m_GridManager.MapTiles[neighbourCell], g, h, f, actualData))
                {
                    AddToOpenList(neighbourCell);
                    //openList.Add(mapTiles[neighbourCell]);
                    m_GridManager.MapTiles[neighbourCell].aStarData.SetupAStarData(g, h, f, actualData);
                    //mapTiles[neighbourCell].ChangeMaterial(mapTiles[neighbourCell].tile.open);
                }
            }

            m_OpenList = m_OpenList.OrderBy(tileData => tileData.aStarData.f).ThenBy(tileData => tileData.aStarData.h).ToList();
            TileData firstData = m_OpenList.ElementAt(0);
            //closedList.Add(firstData);
            AddToClosedList(firstData.ToVector());
            m_OpenList.RemoveAt(0);
            //firstData.ChangeMaterial(firstData.tile.closed);
            m_LastCoordinates = firstData.ToVector();
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsClosed(TileData data)
        {
            return m_ClosedList.Contains(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        private void AddToOpenList(Vector2Int coordinates)
        {
            m_OpenList.Add(m_GridManager.MapTiles[coordinates]);
            m_GridManager.MapTiles[coordinates].ChangeMaterial(m_GridManager.MapTiles[coordinates].tile.open);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        private void AddToClosedList(Vector2Int coordinates)
        {
            m_ClosedList.Add(m_GridManager.MapTiles[coordinates]);
            m_GridManager.MapTiles[coordinates].ChangeMaterial(m_GridManager.MapTiles[coordinates].tile.closed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        public void ReturnToNormalTiles(Vector2Int coordinates)
        {
            if (m_OpenList.Contains(m_GridManager.MapTiles[coordinates]))
            {
                m_OpenList.Remove(m_GridManager.MapTiles[coordinates]);
                m_GridManager.MapTiles[coordinates].tile.ChangeMaterial(m_GridManager.MapTiles[coordinates].tile.normal);
            }

            if (m_ClosedList.Contains(m_GridManager.MapTiles[coordinates]))
            {
                m_ClosedList.Remove(m_GridManager.MapTiles[coordinates]);
                m_GridManager.MapTiles[coordinates].tile.ChangeMaterial(m_GridManager.MapTiles[coordinates].tile.normal);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void TracePath()
        {
            m_PathPositions.Clear();
            var actualPosition = m_EndCoordinates;
            m_LineRenderer.positionCount = 0;
            while (actualPosition != m_StartCoordinates)
            {
                m_PathPositions.Add(m_GridManager.GetWorld3DPosition(m_GridManager.MapTiles[actualPosition].ToVector()));
                AddVertexToPath(actualPosition);
                actualPosition = m_GridManager.MapTiles[actualPosition].aStarData.parent.ToVector();
            }
            m_PathPositions.Reverse();
            m_LineRenderer.enabled = true;
            m_PathSearched = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualPosition"></param>
        private void AddVertexToPath(Vector2Int actualPosition)
        {
            m_LineRenderer.positionCount++;
            m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, m_GridManager.GetWorld3DPosition(actualPosition) + Vector3.up);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator MovePlayer()
        {
            int index = 0;
            GameObject player = m_TokenGenerator.Tokens[0];
            while (index < m_PathPositions.Count)
            {
                player.transform.Translate(GetDirection(player.transform.position, m_PathPositions[index]).normalized / 10f);
                if (Vector3.Distance(player.transform.position, m_PathPositions[index]) < 0.1) index++;
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Reached end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private Vector3 GetDirection(Vector3 from, Vector3 to)
        {
            return to - from;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetStartEndPosition()
        {
            FindRandomPosition(ref m_StartCoordinates, m_TokenGenerator.StartPrefab);
            m_TokenGenerator.SpawnToken(m_TokenGenerator.StartPrefab, m_StartCoordinates);
            SetupStartCoordinates();
            do
            {
                FindRandomPosition(ref m_EndCoordinates, m_TokenGenerator.EndPrefab);
            } while (m_EndCoordinates == m_StartCoordinates);

            m_TokenGenerator.SpawnToken(m_TokenGenerator.EndPrefab, m_EndCoordinates);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetupStartCoordinates()
        {
            m_LastCoordinates = m_StartCoordinates;
            AddToClosedList(m_StartCoordinates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tilePositionSelected"></param>
        /// <param name="prefab"></param>
        private void FindRandomPosition(ref Vector2Int tilePositionSelected, GameObject prefab)
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
                if (m_GridManager.CheckIfTileIsWalkable(positionOnGrid))
                {
                    foundPosition = true;
                    tilePositionSelected = positionOnGrid;
                }
            }
        }
    }
}
