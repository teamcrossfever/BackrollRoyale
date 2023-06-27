using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Pacman : MonoBehaviour
{
    Transform _transform;
    public int playerNum;
    public float inputThreshold = 0.5f;
    [SerializeField]
    Transform spinner;

    //layers
    [SerializeField]
    LayerMask collectablesMask;

    Player cInput;
    Vector3 inputDirection;
    Movement3D movement;

    Collider[] collisions = new Collider[4];

    GameManager gm;

    //Reverse hit
    float reverseTime = 1;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

        _transform = transform;

        cInput = Rewired.ReInput.players.GetPlayer(playerNum);
        movement = GetComponent<Movement3D>();
    }
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleCollisions();
    }

    public void UpdateMovement(Vector3 direction)
    {
        inputDirection = direction;

        if (reverseTime >= 1)
        {
            //Debug.Log($"MAG: {inputDirection.magnitude}");
            if (inputDirection.magnitude > inputThreshold)
            {
                if (inputDirection.y > 0)
                {
                    movement.SetDirection(Vector3.forward);
                }
                else if (inputDirection.y < 0)
                {
                    movement.SetDirection(Vector3.back);
                }
                else if (inputDirection.x > 0)
                {
                    movement.SetDirection(Vector3.right);
                }
                else if (inputDirection.x < 0)
                {
                    movement.SetDirection(Vector3.left);
                }
            }
        }
        else
        {
            //When reverse time is negative count to 1 to stop it
            reverseTime += Time.deltaTime;
            spinner.rotation = Quaternion.LookRotation(-movement.Direction, Vector3.up);

            if (movement.IsColliding(movement.Direction))
            {
                Vector3 dir = Quaternion.Euler(0, -90, 0) * movement.Direction;

                if (!movement.IsColliding(dir))
                {
                    movement.SetDirection(dir);
                }
                else
                {
                    dir = Quaternion.Euler(0, 90, 0) * movement.Direction;
                    movement.SetDirection(dir);
                }

            }

            if (reverseTime >= -0.5f)
            {
                reverseTime = 1;
                movement.SetDirection(-movement.Direction);
            }

            return;
        }

        /*
        float angle = Mathf.Atan2(movement.Direction.y, movement.Direction.x);
        _transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        */

        spinner.rotation = Quaternion.LookRotation(movement.Direction, Vector3.up);
    }

    private void HandleCollisions()
    {
        collisions = new Collider[4];
        int hitCount = Physics.OverlapSphereNonAlloc(_transform.position, 0.5f, collisions,collectablesMask);
        if (hitCount > 0)
        {
            for(int i=0; i<collisions.Length; i++)
            {
                var col = collisions[i];
                if (!col)
                    continue;

                if (col.CompareTag(Tags.Player))
                {
                    var otherPlayer = col.GetComponent<Pacman>();

                    if (otherPlayer == this)
                        continue;

                    if (reverseTime >= 1)
                    {
                        KnockedBack();
                        otherPlayer.KnockedBack();
                        continue;
                    }
                    
                }

                if (col.CompareTag(Tags.Pellet))
                {
                    var pellet = col.GetComponent<Pellet>();
                    gm.PelletEaten(pellet);
                }
                
                if (col.CompareTag(Tags.Ghost))
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

    public void KnockedBack()
    {
        if (reverseTime < 1)
            return;

        reverseTime = -1;
        movement.SetDirection(-movement.Direction);

    }

    public void ResetState()
    {
        this.movement.ResetState();
        this.gameObject.SetActive(true);
    }
}
