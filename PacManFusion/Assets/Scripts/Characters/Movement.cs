using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    Transform _transform;
    public float speed = 8f;
    public float speedMultiplier = 1;

    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public Rigidbody2D rb { get; private set; }

    public Vector2 Direction { get; private set; }
    public Vector2 NextDirection { get; private set; }

    public Vector3 StartPosition { get; private set; }

    private void Start()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody2D>();
        StartPosition = _transform.position;
        Direction = initialDirection;
    }

    private void Update()
    {
        if (NextDirection != Vector2.zero)
        {
            SetDirection(NextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = rb.position;
        Vector2 translation = Direction * speed * speedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(pos + translation);

    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !IsColliding(direction) )
        {
            Direction = direction;
            NextDirection = Vector2.zero;
        }
        else
        {
            NextDirection = direction;
        }
    }

    public bool IsColliding(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, obstacleLayer);
        return hit.collider!=null;
    }

    public void ResetState()
    {
        speedMultiplier = 1;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        _transform.position = StartPosition;
        rb.isKinematic = false;
        enabled = true;
    }
}
