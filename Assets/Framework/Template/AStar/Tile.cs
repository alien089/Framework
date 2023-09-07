namespace Framework.Template.AStarTemplate
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class Tile : MonoBehaviour, IPointerClickHandler
    {
        public TileData data;
        private TileUIManager uiManager;

        public TileUIManager UiManager { get => uiManager; }

        private void Awake()
        {
            uiManager = GetComponent<TileUIManager>();
        }

        public void Initialize(GridManager gridM, int rowInit, int columnInit, bool walkable, Tile tile)
        {
            data = new TileData(gridM, rowInit, columnInit, walkable, tile);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

    }
}
