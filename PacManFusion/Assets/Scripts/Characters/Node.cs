using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private Transform _transform;
    public List<Vector2> availableDirections { get; private set; }
    private void Start()
    {
        _transform = transform;
        this.availableDirections = new List<Vector2>();

        CheckAvailableDirection(Vector3.up);
        CheckAvailableDirection(Vector3.down);
        CheckAvailableDirection(Vector3.right);
        CheckAvailableDirection(Vector3.left);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, obstacleLayer);
        if (!hit.collider)
        {
            this.availableDirections.Add(direction);
        }
    }
}
