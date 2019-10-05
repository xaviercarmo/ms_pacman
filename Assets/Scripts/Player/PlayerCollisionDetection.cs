using UnityEngine;
using System.Collections;

public class PlayerCollisionDetection : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered Bitch");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided Bitch");
    }
}
