using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FruitMode
{
    Idle,
    Random,
    Exit
}

public class FruitManager : MonoBehaviour
{
    public static FruitManager Instance { get; private set; }

    public Tilemap WallsTilemap;
    public GameObject CherriesPrefab;

    bool SpawnedCherries = false;
    List<GameObject> SpawnedFruits;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            SpawnedFruits = new List<GameObject>();
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (PlayerManager.Instance.DotsEaten >= 70)
        {
            if (!SpawnedCherries)
            {
                SpawnedFruits.Add(Instantiate(CherriesPrefab, new Vector3(11, 6), Quaternion.identity));
                SpawnedCherries = true;
            }
            else
            {

            }
        }
    }
}
