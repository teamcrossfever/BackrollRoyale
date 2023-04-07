using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    Collider2D[] cols = new Collider2D[4];

    public Transform inside;
    public Transform outside;
    public Vector3 initialPosition;

    Coroutine exitTransitionRoutin = null;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (this.gameObject.activeSelf)
        {
            exitTransitionRoutin = StartCoroutine(ExitTransition());
        }
    }
    private void FixedUpdate()
    {
        HandleCollisions();
    }
    void HandleCollisions()
    {
        var hits = Physics2D.OverlapCircleNonAlloc(transform.position, 0.5f, cols, layerInteraction);
        if (hits > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (!cols[i])
                    continue;

                if (enabled && !ghost.frightened.enabled)
                {
                    this.ghost.movement.SetDirection(-this.ghost.movement.Direction);
                }
            }
        }
    }

    IEnumerator ExitTransition()
    {
        //this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rb.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 pos = this.transform.position;
        float direction = pos.x < inside.position.x ? 1 : -1;

        //this.ghost.movement.SetDirection(Vector2.right*direction, true);
        while (transform.position.x < inside.position.x)
        {
            this.transform.Translate(Vector2.right * direction * this.ghost.movement.speed*Time.deltaTime);
            yield return null;
        }

        if (direction > 0)
        {
            pos = transform.position;
            pos.x = inside.position.x;
            this.transform.position = pos;
        }

        while (transform.position.x > inside.position.x)
        {
            this.transform.Translate(Vector2.right * direction * this.ghost.movement.speed * Time.deltaTime);
            yield return null;
        }

        if (direction < 0)
        {
            pos = transform.position;
            pos.x = inside.position.x;
            this.transform.position = pos;
        }

        while (transform.position.y < outside.position.y)
        {
            this.transform.Translate(Vector2.up * this.ghost.movement.speed * Time.deltaTime);
            yield return null;
        }

        pos = transform.position;
        pos.y = outside.position.y;
        this.transform.position = pos;

        this.ghost.movement.SetDirection(new Vector2(Random.value <0.5f ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rb.isKinematic = false;
        this.ghost.movement.enabled = true;
        yield return null;

    }
}
