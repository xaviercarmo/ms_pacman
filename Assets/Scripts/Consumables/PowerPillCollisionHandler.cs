using UnityEngine;
using System.Collections;

public class PowerPillCollisionHandler : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!LevelManager.Instance.ScrollingMode)
            {
                GhostManager.Instance.ChangeGhostMode(GhostMode.Frightened);
            }

            Destroy(gameObject);
        }
    }
}
