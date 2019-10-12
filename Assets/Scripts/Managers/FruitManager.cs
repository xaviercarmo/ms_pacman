using System.Linq;
using System.Collections;
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

    bool spawnedCherries = false;
    List<(GameObject FruitGameObject, int pointsValue, float SpawnTime, float Duration)> spawnedFruits;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            spawnedFruits = new List<(GameObject, int, float, float)>();
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (PlayerManager.Instance.DotsEaten >= 70)
        {
            if (!spawnedCherries)
            {
                var cherryDuration = 20;
                var cherryPointsValue = 200;
                var fruitGameObject = Instantiate(CherriesPrefab, new Vector3(11, 6), Quaternion.identity);

                spawnedFruits.Add((fruitGameObject, cherryPointsValue, Time.time, cherryDuration));
                StartCoroutine(MakeFruitExitAfterDuration(fruitGameObject, cherryDuration));

                spawnedCherries = true;
            }
        }
    }

    IEnumerator MakeFruitExitAfterDuration(GameObject fruitGameObject, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            if (!OriginalLevelManager.Instance.GameSuspended)
            {
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        if (fruitGameObject != null) { fruitGameObject.GetComponent<FruitMovement>().Mode = FruitMode.Exit; }
    }

    public void ConsumeFruit(GameObject fruitGameObject)
    {
        var fruitToConsume = spawnedFruits.Find(tuple => tuple.FruitGameObject == fruitGameObject);
        PlayerManager.Instance.Points += fruitToConsume.pointsValue;
        Destroy(fruitToConsume.FruitGameObject);
    }
}
