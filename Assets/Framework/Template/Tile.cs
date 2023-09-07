namespace Framework.Template.GridTemplate
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class Tile : MonoBehaviour, IPointerClickHandler
    {
        public TileData Data;

        public void Initialize(GridManager gridManager, int rowInit, int columnInit, bool walkable, Tile tile)
        {
            Data = new TileData(gridManager, rowInit, columnInit, walkable, tile);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
           
        }
    }
}