namespace Framework.Template.GridTemplate
{
    using UnityEngine;

    [System.Serializable]
    public class TileData
    {
        public int Row;
        public int Column;
        public GridManager GridManager;
        public bool Walkable;
        public Tile Tile;

        public TileData(GridManager gridManager, int newRow, int newColumn, bool walkable, Tile tile)
        {
            Row = newRow;
            Column = newColumn;
            GridManager = gridManager;
            Walkable = walkable;
            Tile = tile;
        }

        public Vector2Int ToVector()
        {
            return new Vector2Int(Row, Column);
        }

    }

}