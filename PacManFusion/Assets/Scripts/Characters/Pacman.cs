using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Pacman : MonoBehaviour
{
    Transform _transform;
    public float inputThreshold = 0.5f;

    //layers
    [SerializeField]
    LayerMask collectablesMask;

    Player cInput;
    Vector2 inputDirection;
    Movement movement;

    Collider2D[] collisions = new Collider2D[4];

    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

        _transform = transform;

        cInput = Rewired.ReInput.players.GetPlayer(0);
        movement = GetComponent<Movement>();
    }
    private void Update()
    {
        inputDirection = cInput.GetAxis2DRaw("Horizontal", "Vertical");
        //Debug.Log($"MAG: {inputDirection.magnitude}");
        if (inputDirection.magnitude > inputThreshold)
        {
            if (inputDirection.y > 0)
            {
                movement.SetDirection(Vector2.up);
            }else if (inputDirection.y < 0)
            {
                movement.SetDirection(Vector2.down);
            }
            else if (inputDirection.x > 0)
            {
                movement.SetDirection(Vector2.right);
            }
            else if (inputDirection.x < 0)
            {
                movement.SetDirection(Vector2.left);
            }
        }

        float angle = Mathf.Atan2(movement.Direction.y, movement.Direction.x);
        _transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    private void FixedUpdate()
    {
        HandleCollisions();
    }

    private void HandleCollisions()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(_transform.position, 0.5f, collisions,collectablesMask);
        if (hitCount > 0)
        {
            Debug.Log("COLLISIONS");
            for(int i=0; i<collisions.Length; i++)
            {
                var col = collisions[i];
                if (!col)
                    continue;

                if (col.CompareTag(Tags.Pellet))
                {
                    var pellet = col.GetComponent<Pellet>();
                    gm.PelletEaten(pellet);
                }else if (col.CompareTag(Tags.Ghost))
                {
                    var ghost = col.GetComponent<Ghost>();

                    if (ghost.frightened.enabled)
                        gm.GhostEaten(ghost);
                    else
                        gm.PacmanEaten();
                }
            }
        }
    }

    public void ResetState()
    {
        this.movement.ResetState();
        this.gameObject.SetActive(true);
    }
}
