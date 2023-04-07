using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChase : GhostBehavior
{
    Collider2D[] cols = new Collider2D[4];
    private void FixedUpdate()
    {
        HandleCollisions();
    }

    private void OnDisable()
    {
        ghost.chase.Enable();
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
                var node = cols[i].GetComponent<Node>();
                if (node && enabled && !ghost.frightened.enabled)
                {
                    Vector2 dir = Vector2.zero;
                    float minDist = float.MaxValue;

                    foreach (Vector2 availableDirection in node.availableDirections)
                    {
                        Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0);
                        float distance = (this.ghost.target.position - newPosition).sqrMagnitude;

                        if (distance < minDist)
                        {
                            dir = availableDirection;
                            minDist = distance;
                        }
                    }

                    this.ghost.movement.SetDirection(dir);
                }
            }
        }
    }
}
