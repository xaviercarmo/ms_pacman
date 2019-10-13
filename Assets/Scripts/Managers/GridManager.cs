using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Camera LevelCamera;
    public GameObject GridGroup;
    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap UpBlockersTilemap;
    public TileBase UpBlocker;
    public Tilemap DownBlockersTilemap;
    public Tilemap DotsTilemap;
    public Tilemap HorizontalPortalsTilemap;
    public Tilemap GhostSlowersTilemap;
    public GameObject SpikesRow;

    public GameObjectGrid ObjectGrid;

    Tilemap[] tileMapsToClear;

    float baseScrollSpeed = 0.15f;
    float scrollSpeedMultiplier = 1;

    int rowDeletionOffset = 2;
    Vector3Int rowDeletionOffsetVector;

    LevelManagerNew levelManagerNew;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            rowDeletionOffsetVector = new Vector3Int(WallsTilemap.cellBounds.xMin, WallsTilemap.cellBounds.yMax - rowDeletionOffset, 0);

            tileMapsToClear = new Tilemap[]
            {
                WallsTilemap,
                DownBlockersTilemap,
                DotsTilemap,
                HorizontalPortalsTilemap,
                GhostSlowersTilemap
            };

            int numRows = WallsTilemap.cellBounds.yMax - WallsTilemap.cellBounds.yMin;
            int numCols = WallsTilemap.cellBounds.xMax - WallsTilemap.cellBounds.xMin;

            ObjectGrid = new GameObjectGrid(numRows, numCols);
            ObjectGrid.AddGameObject(2, 1, GameObject.Find("power_pill_tl"));
            ObjectGrid.AddGameObject(2, 19, GameObject.Find("power_pill_tr"));
            ObjectGrid.AddGameObject(18, 1, GameObject.Find("power_pill_bl"));
            ObjectGrid.AddGameObject(18, 19, GameObject.Find("power_pill_br"));
        }
    }

    void Start()
    {
        levelManagerNew = LevelManager.Instance as LevelManagerNew;
    }

    void Update()
    {
        if (!LevelManager.Instance.GameSuspended && levelManagerNew.ScrollingMode)
        {
            scrollSpeedMultiplier = Mathf.Pow(1 + (1 - LevelCamera.WorldToViewportPoint(PlayerManager.Instance.gameObject.transform.position).y), 4);

            var translationAmount = new Vector3(0, baseScrollSpeed * scrollSpeedMultiplier * Time.deltaTime, 0);
            GridGroup.transform.Translate(translationAmount);

            rowDeletionOffsetVector.y = WallsTilemap.cellBounds.yMax - rowDeletionOffset;
            var highestCellWorldPos = LevelGrid.CellToWorld(rowDeletionOffsetVector);
            if (LevelCamera.WorldToViewportPoint(highestCellWorldPos).y > 1)
            {
                var cellPos = rowDeletionOffsetVector;
                for (var i = WallsTilemap.cellBounds.xMin; i < WallsTilemap.cellBounds.xMax; i++)
                {
                    cellPos.x = i;
                    UpBlockersTilemap.SetTile(cellPos, null);
                    UpBlockersTilemap.SetTile(cellPos - new Vector3Int(0, 2, 0), UpBlocker);

                    Array.ForEach(tileMapsToClear, tileMap => tileMap.SetTile(cellPos, null));
                }

                ObjectGrid.ShiftDown(1);

                rowDeletionOffset++;
            }
        }
    }

    public class GameObjectGrid
    {
        List<GameObject[]> grid = new List<GameObject[]>();
        int numCols;

        public GameObjectGrid(int numCols, int numRows)
        {
            this.numCols = numCols;

            for (var i = 0; i < numRows; i++)
            {
                grid.Add(new GameObject[numCols]);
            }
        }

        void DeleteRow(int rowNum)
        {
            if (rowNum >= 0 && rowNum < grid.Count)
            {
                Array.ForEach(grid[rowNum], gameObject => Destroy(gameObject));
                grid.RemoveAt(rowNum);
            }
        }

        public void AddGameObject(int rowNum, int colNum, GameObject gameObject)
        {
            if (rowNum >= 0 && rowNum < grid.Count && colNum >= 0 && colNum < numCols)
            {
                grid[rowNum][colNum] = gameObject;
            }
        }

        public void ShiftDown(int numRows)
        {
            for (var i = 0; i < numRows; i++)
            {
                DeleteRow(0);
                grid.Add(new GameObject[numCols]);
            }
        }
    }
}
