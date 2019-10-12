using UnityEngine;
using System.Collections;

public class FruitMovement : MonoBehaviour
{
    Vector3Int PreviousCellPos;
    Vector3Int CurrentCellPos;
    Vector3Int TargetCellPos;

    float timeToTravelGridSize = 0.32f;
    Tweener tweener;

    void Awake()
    {
        tweener = GetComponent<Tweener>();
    }

    // Use this for initialization
    void Start()
    {
        CurrentCellPos = GhostManager.Instance.LevelGrid.WorldToCell(transform.position);
        TargetCellPos = CurrentCellPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (OriginalLevelManager.Instance.GameSuspended) { return; }


    }
}
