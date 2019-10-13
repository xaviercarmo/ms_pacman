using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Camera LevelCamera;
    public GameObject GridGroup;
    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap GhostUpBlockersTilemap;
    public TileBase GhostUpBlocker;
    public Tilemap GhostDownBlockersTilemap;
    public Tilemap DotsTilemap;
    public Tilemap HorizontalPortalsTilemap;
    public Tilemap DownBlockersTilemap;
    public Tilemap GhostSlowersTilemap;

    float baseScrollSpeed = 0.5f;
    float scrollSpeedMultiplier = 1;

    int rowDeletionOffset = 2;
    Vector3Int rowDeletionOffsetVector;

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
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (!LevelManager.Instance.GameSuspended)
        {
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
                    GhostUpBlockersTilemap.SetTile(cellPos + new Vector3Int(0, 1, 0), null);
                    GhostUpBlockersTilemap.SetTile(cellPos, GhostUpBlocker);
                    WallsTilemap.SetTile(cellPos, null);
                    DotsTilemap.SetTile(cellPos, null);

                }

                rowDeletionOffset++;
            }
        }
    }
}
