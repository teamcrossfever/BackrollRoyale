using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScatter : GhostBehavior
{
    Collider2D[] cols = new Collider2D[4];
    private void FixedUpdate()
    {
        HandleCollisions();
    }

    void HandleCollisions()
    {
        Debug.Log("Scatter");
        var hits = Physics2D.OverlapCircleNonAlloc(transform.position, 0.5f, cols, layerInteraction);
        if (hits > 0)
        {
            Debug.Log($"COLLISION SCATTER HIT: {hits}");
            for(int i=0; i<cols.Length; i++)
            {
                if (!cols[i])
                    continue;
                var node = cols[i].GetComponent<Node>();
                if(node && enabled && !ghost.frightened.enabled)
                {
                    int index = Random.Range(0, node.availableDirections.Count);

                    if (node.availableDirections[index] == -this.ghost.movement.Direction && node.availableDirections.Count>1)
                    {
                        index++;

                        if(index>= node.availableDirections.Count)
                        {
                            index = 0;
                        }

                        this.ghost.movement.SetDirection(node.availableDirections[index]);
                    }
                }
            }
        }
    }
}
