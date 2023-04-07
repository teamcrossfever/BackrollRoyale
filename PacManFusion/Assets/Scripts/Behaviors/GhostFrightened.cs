using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    Collider2D[] cols = new Collider2D[4];

    public bool eaten { get; private set; }
    public void Eaten()
    {
        this.eaten = true;
        ToggleCollision(false);
        this.ghost.movement.speedMultiplier = 2f;
    }

    public override void Enable(float duration)
    {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;

        Invoke(nameof(Flash), duration / 2);
    }

    public override void Disable()
    {
        base.Disable();

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void OnEnable()
    {
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable()
    {
        this.ghost.movement.speedMultiplier = 1f;
        this.eaten = false;
        ToggleCollision(true);
    }

    private void FixedUpdate()
    {
        if (eaten)
            HandleCollisions();
    }

    void HandleCollisions()
    {
        float minDist = float.MaxValue;

        var hits = Physics2D.OverlapCircleNonAlloc(transform.position, 0.5f, cols, layerInteraction);
        if (hits > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (!cols[i])
                    continue;

                

                var node = cols[i].GetComponent<Node>();
                if (node && enabled)
                {
                    Vector2 dir = Vector2.zero;
                    

                    foreach (Vector2 availableDirection in node.availableDirections)
                    {
                        Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0);
                        float distance = (this.ghost.home.outside.position - newPosition).sqrMagnitude;

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
        Debug.Log(minDist);
        if (minDist <= 1.05f)
        {
            enterHomeRoutine = StartCoroutine(EnterHome());
        }
    }
    private void Flash()
    {
        if (!this.eaten)
        {
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Reset();
        }
    }

    Coroutine enterHomeRoutine = null;

    /// <summary>
    /// Very much like leaving home but that oppisite
    /// </summary>
    /// <returns></returns>
    IEnumerator EnterHome()
    {
        var inside = ghost.home.inside;
        var outside = ghost.home.outside;
        var startPosition = ghost.movement.StartPosition;

        this.ghost.movement.rb.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 pos = this.transform.position;
        float direction = pos.x < outside.position.x ? 1 : -1;

        //Move towards the outside point
        while (transform.position.x < outside.position.x)
        {
            this.transform.Translate(Vector2.right * direction * this.ghost.movement.speed * Time.deltaTime);
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

        //Now move down into the home
        while (transform.position.y > inside.position.y)
        {
            this.transform.Translate(Vector2.down * this.ghost.movement.speed * Time.deltaTime);
            yield return null;
        }

        pos = transform.position;
        pos.y = inside.position.y;
        this.transform.position = pos;


        //Now time to move ghost back into their room spot
        //Move towards the outside point

        direction = pos.x < startPosition.x ? 1 : -1;

        while (transform.position.x < startPosition.x)
        {
            this.transform.Translate(Vector2.right * direction * this.ghost.movement.speed * Time.deltaTime);
            yield return null;
        }

        if (direction > 0)
        {
            pos = transform.position;
            pos.x = inside.position.x;
            this.transform.position = pos;
        }

        while (transform.position.x > startPosition.x)
        {
            this.transform.Translate(Vector2.right * direction * this.ghost.movement.speed * Time.deltaTime);
            yield return null;
        }

        if (direction < 0)
        {
            pos = transform.position;
            pos.x = startPosition.x;
            this.transform.position = pos;
        }


        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rb.isKinematic = false;
        this.ghost.movement.enabled = true;

        this.ghost.home.Enable(this.duration);
        this.Disable();
        yield return null;

    }
}
