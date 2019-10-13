using UnityEngine;
using System.Collections;

public class SpikesCollisionHandling : MonoBehaviour
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
            PlayerManager.Instance.Health = 0;
        }

        switch (collision.gameObject.tag)
        {
            case "Player":
                PlayerManager.Instance.Health = 0;
                break;
            case "Ghost":
                AudioManager.Instance.GhostEatenAudioSource.Play();
                collision.gameObject.SetActive(false);
                break;
        }
    }
}
