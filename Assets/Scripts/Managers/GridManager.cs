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

    //Tiles
    public TileBase LeftOuterWall;
    public TileBase RightOuterWall;
    public TileBase LeftEndInnerWall;
    public TileBase RightEndInnerWall;
    public TileBase MiddleInnerWallHorizontal;
    public TileBase Dot;

    public GameObjectGrid ObjectGrid;

    Tilemap[] tileMapsToClear;

    float baseScrollSpeed = 0.15f;
    float scrollSpeedMultiplier = 1;

    int rowDeletionOffset = 2;
    Vector3Int rowDeletionOffsetVector;

    int rowCreationOffset = 0;
    Vector3Int rowCreationOffsetVector;

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
            rowCreationOffsetVector = new Vector3Int(WallsTilemap.cellBounds.xMin, WallsTilemap.cellBounds.yMin - rowCreationOffset, 0);

            tileMapsToClear = new Tilemap[]
            {
                WallsTilemap,
                UpBlockersTilemap,
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
        PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        if (!LevelManager.Instance.GameSuspended && LevelManager.Instance.ScrollingMode)
        {
            //Adjusts the speed of the map's scrolling based on the player's proximity the bottom of the viewport
            //Works so that as the player gets close to the bottom it gets drastically exponentially faster
            scrollSpeedMultiplier = Mathf.Pow(1 + (1 - LevelCamera.WorldToViewportPoint(PlayerManager.Instance.gameObject.transform.position).y), 6);

            var translationAmount = new Vector3(0, baseScrollSpeed * scrollSpeedMultiplier * Time.deltaTime, 0);
            GridGroup.transform.Translate(translationAmount);

            DeleteNonVisibleRows();
            GenerateRows();
        }
    }

    public void DeleteBottomRow()
    {
        var cellPos = rowDeletionOffsetVector;
        cellPos.y = WallsTilemap.cellBounds.yMin;
        for (var i = WallsTilemap.cellBounds.xMin; i < WallsTilemap.cellBounds.xMax; i++)
        {
            cellPos.x = i;
            Array.ForEach(tileMapsToClear, tileMap => tileMap.SetTile(cellPos, null));
        }
    }

    void DeleteNonVisibleRows()
    {
        //Adjusts the deletion offset to keep track of where the scrolling map is up to, then uses this value to
        //find the world co-ordinates of the highest cell-pos
        rowDeletionOffsetVector.y = WallsTilemap.cellBounds.yMax - rowDeletionOffset;
        var highestCellWorldPos = LevelGrid.CellToWorld(rowDeletionOffsetVector);

        //If the highest point is above the viewport
        if (LevelCamera.WorldToViewportPoint(highestCellWorldPos).y > 1)
        {
            //Iterates the row of the grid that is off the screen for every tilemap, and clears the tiles from those tilemaps
            var cellPos = rowDeletionOffsetVector;
            for (var i = WallsTilemap.cellBounds.xMin; i < WallsTilemap.cellBounds.xMax; i++)
            {
                cellPos.x = i;
                Array.ForEach(tileMapsToClear, tileMap => tileMap.SetTile(cellPos, null));
            }

            //Shifts the object grid so that it stays in sync with the level grid
            ObjectGrid.ShiftDown(1);

            rowDeletionOffset++;
        }
    }

    //Procedurally generates rows (only horizontally right now) and spawns ghosts
    void GenerateRows()
    {
        rowCreationOffsetVector.y = WallsTilemap.cellBounds.yMin - rowCreationOffset;
        var lowestCellWorldPos = LevelGrid.CellToWorld(rowCreationOffsetVector);

        //if the lowest point is close to being visible
        if (LevelCamera.WorldToViewportPoint(lowestCellWorldPos).y > -0.05)
        {
            var cellPos = rowCreationOffsetVector;

            AddOuterWalls(cellPos);
            cellPos.x++;

            DotsTilemap.SetTile(cellPos, Dot);
            DotsTilemap.SetTile(new Vector3Int(20 - cellPos.x, cellPos.y, 0), Dot);
            cellPos.x++;

            var totalWidth = 0;
            while (totalWidth < 9 - 2)
            {
                var width = UnityEngine.Random.Range(2, Mathf.Min(7, 9 - totalWidth));
                AddPlatformOfSize(width, cellPos, true);
                cellPos.x += width;

                DotsTilemap.SetTile(cellPos, Dot);
                DotsTilemap.SetTile(new Vector3Int(20 - cellPos.x, cellPos.y, 0), Dot);
                cellPos.x++;

                totalWidth += width + 1;
            }

            var remainder = ((10 - totalWidth) - 1) * 2 + 1;

            if (remainder >= 5)
            {
                //bridge the gap
                AddPlatformOfSize(remainder - 2, new Vector3Int(totalWidth + 2, cellPos.y, 0), false);
            }
            else if (remainder == 3)
            {
                //conjoin
                AddBridgeOfSize(5, new Vector3Int(totalWidth, cellPos.y, 0));
            }
            else
            {
                //roll to conjoin
                if (UnityEngine.Random.value < 0.5f)
                {
                    AddBridgeOfSize(3, new Vector3Int(totalWidth, cellPos.y, 0));
                }
            }

            cellPos.x = 0;
            cellPos.y--;
            AddOuterWalls(cellPos);
            AddDotRow(cellPos);

            rowCreationOffset = 1;
        }
    }

    void AddDotRow(Vector3Int startPos)
    {
        var currPos = startPos;
        for (var i = 1; i < 20; i++)
        {
            currPos.x = i;
            DotsTilemap.SetTile(currPos, Dot);
            if (UnityEngine.Random.value < 0.05f)
            {
                var ghostType = UnityEngine.Random.Range(0, 3);
                var ghostPos = LevelGrid.CellToWorld(currPos);
                GhostBehaviour behaviour = null;

                switch (ghostType)
                {
                    case 0:
                        behaviour = new RedGhostBehaviour();
                        GhostManager.Instance.SpawnGhost(GhostManager.Instance.RedGhostPrefab, ghostPos, behaviour);
                        break;
                    case 1:
                        behaviour = new OrangeGhostBehaviour();
                        GhostManager.Instance.SpawnGhost(GhostManager.Instance.OrangeGhostPrefab, ghostPos, behaviour);
                        break;
                    case 2:
                        behaviour = new PinkGhostBehaviour();
                        GhostManager.Instance.SpawnGhost(GhostManager.Instance.PinkGhostPrefab, ghostPos, behaviour);
                        break;
                    case 3:
                        behaviour = new RedGhostBehaviour();
                        GhostManager.Instance.SpawnGhost(GhostManager.Instance.RedGhostPrefab, ghostPos, behaviour);
                        GhostManager.Instance.SpawnGhost(GhostManager.Instance.BlueGhostPrefab, ghostPos, new BlueGhostBehaviour(behaviour.MovementHandler));
                        break;
                }

                behaviour?.SetMode(GhostMode.Chasing);
            }
        }
    }

    void AddOuterWalls(Vector3Int startPos)
    {
        WallsTilemap.SetTile(startPos, LeftOuterWall);
        WallsTilemap.SetTile(new Vector3Int(20 - startPos.x, startPos.y, 0), RightOuterWall);
    }

    void AddPlatformOfSize(int size, Vector3Int startPos, bool mirror)
    {
        var currPos = startPos;
        WallsTilemap.SetTile(currPos, LeftEndInnerWall);
        currPos.x++;

        for (var i = 1; i < size - 1; i++)
        {
            WallsTilemap.SetTile(currPos, MiddleInnerWallHorizontal);
            currPos.x++;
        }

        WallsTilemap.SetTile(currPos, RightEndInnerWall);
        
        if (mirror)
        {
            var mirrPos = new Vector3Int(20 - startPos.x, startPos.y, 0);
            WallsTilemap.SetTile(mirrPos, RightEndInnerWall);
            mirrPos.x--;

            for (var i = 1; i < size - 1; i++)
            {
                WallsTilemap.SetTile(mirrPos, MiddleInnerWallHorizontal);
                mirrPos.x--;
            }

            WallsTilemap.SetTile(mirrPos, LeftEndInnerWall);
        }
    }

    void AddBridgeOfSize(int size, Vector3Int startPos)
    {
        var currPos = startPos;
        for (var i = 0; i < size; i++)
        {
            WallsTilemap.SetTile(currPos, MiddleInnerWallHorizontal);
            DotsTilemap.SetTile(currPos, null);
            currPos.x++;
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
