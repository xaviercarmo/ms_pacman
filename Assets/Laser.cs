using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            lineRenderer.SetPosition(1, Input.mousePosition);

            var hit = Physics2D.Raycast(transform.position, Input.mousePosition - transform.position);
            if (hit)
            {
                lineRenderer.enabled = true;
            }
        }
    }
}
