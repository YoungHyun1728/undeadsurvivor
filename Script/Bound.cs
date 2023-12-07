using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Camera mainCamera;

    public float speed = 5f;
    public float rotationSpeed = 180f; // 회전 속도

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

    }

    void Update()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            BounceOffScreenEdge();
        }
    }

    void BounceOffScreenEdge()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 dir = Vector3.zero;

        if (viewPos.x < 0 || viewPos.x > 1)
        {
            dir.x = -Mathf.Sign(viewPos.x - 0.5f);
        }

        if (viewPos.y < 0 || viewPos.y > 1)
        {
            dir.y = -Mathf.Sign(viewPos.y - 0.5f);
        }

        Vector2 bounceDirection = new Vector2(dir.x, dir.y).normalized;
        rigid.velocity = bounceDirection * speed;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, bounceDirection);
    }
}

