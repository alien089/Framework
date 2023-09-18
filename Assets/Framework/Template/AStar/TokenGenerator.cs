namespace Framework.Template.AStarTemplate
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TokenGenerator 
    {
        [SerializeField] private GameObject startPrefab;
        [SerializeField] private GameObject endPrefab;
        private GridManager gridManager;
        private List<Vector2> tokens = new List<Vector2>();

        public List<Vector2> Tokens { get => tokens; }
        public GameObject StartPrefab { get => startPrefab; }
        public GameObject EndPrefab { get => endPrefab; }


        //private void Start()
        //{
        //    TryGetComponent(out gridManager);
        //}

        public void SpawnToken(Vector2Int coordinate)
        {
            tokens.Add(coordinate);
        }

        public void ClearTokens()
        {
            while (tokens.Count > 0)
            {
                var item = tokens[0];
                tokens.RemoveAt(0);
            }
        }
    }
}
