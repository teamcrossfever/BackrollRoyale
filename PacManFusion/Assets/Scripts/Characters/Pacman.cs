using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using MoreMountains.Feedbacks;
public class Pacman : PacComponent
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

    MatchManager gm;

    Animator animator;

    //Reverse hit
    float reverseTime = 1;
    bool poweredUp = false;

    [SerializeField]
    MMF_Player getBigEffect;

    [SerializeField]
    MMF_Player getSmall;

    UnityEngine.AI.NavMeshObstacle navMeshObstacle; //Keeps the ghost away

    private void Start()
    {
        gm = FindObjectOfType<MatchManager>();
        
        _transform = transform;

        cInput = Rewired.ReInput.players.GetPlayer(playerNum);
        movement = GetComponent<Movement3D>();
        animator = GetComponentInChildren<Animator>();
        navMeshObstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        navMeshObstacle.enabled = false;
        animator.enabled = false;
    }

    public void EntityUpdate(float delta)
    {
        if (!Pac.IsAlive)
            return;

        if (animator)
        {
            animator.Update(delta);
        }
        Pac.movement.HandleCollision();
    }

    public void EntityFixedUpdate(float delta)
    {
        if (!Pac.IsAlive)
            return;

        inputDirection = new Vector3(cInput.GetAxisRaw("Horizontal"), cInput.GetAxisRaw("Vertical"),0);
        UpdateMovement(inputDirection);
        Pac.movement.HandleMovement(delta, poweredUp ? 1.1f:1);
        HandleCollisions();
    }

    /*
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

            UpdateMovement(data.direction);

            if((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
            {
                Debug.Log("MOUSE PRESS FIRED!");
            }
        }

        HandleCollisions();
        Pac.movement.HandleMovement();
    }
    */

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
        int hitCount = Physics.OverlapSphereNonAlloc(_transform.position, 0.5f, collisions,collectablesMask, QueryTriggerInteraction.Collide);
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

                    Debug.Log($"Player HIT: {col.name}");

                    if (poweredUp && otherPlayer.poweredUp == false)
                    {
                        gm.PacmanEaten(otherPlayer);
                        continue;
                    }

                    if (reverseTime >= 1 && poweredUp == otherPlayer.poweredUp)
                    {
                        float dot = Vector3.Dot(Pac.movement.Direction, otherPlayer.Pac.movement.Direction);
                        dot = dot < 1 ? -1 : 1;

                        if (dot == 1)
                        {
                            KnockedBack(-1);
                            otherPlayer.KnockedBack(dot);
                        }
                        else
                        {
                            KnockedBack(dot);
                            otherPlayer.KnockedBack(dot);
                        }
                        
                        continue;
                    } 
                }

                if (col.CompareTag(Tags.Pellet))
                {
                    var pellet = col.GetComponent<Pellet>();
                    gm.PelletEaten(pellet, this);
                }
                
                if (col.CompareTag(Tags.Ghost))
                {
                    var oni = col.GetComponent<Oni>();

                    if (oni.OniState == Oni.OniStates.Fear)
                    {
                        if(poweredUp)
                            gm.OniEaten(oni);
                    }
                    else
                    {
                        if (oni.OniState != Oni.OniStates.Return)
                            gm.PacmanEaten(this);
                    }
                }

                if (col.CompareTag(Tags.Passage))
                {
                    var other = col.GetComponent<Passage>();

                    if (other)
                    {
                        transform.position = other.connection.position;
                    }
                }
            }
        }
    }
    public void PowerUp()
    {
        if (!poweredUp)
        {
            getBigEffect.PlayFeedbacks();
        }
        poweredUp = true;
        navMeshObstacle.enabled = poweredUp;

    }

    public void PowerDown()
    {
        if (poweredUp)
        {
            getSmall.PlayFeedbacks();
        }
        poweredUp = false;
        navMeshObstacle.enabled = poweredUp;
    }

    public void SpeedIncreaseMultiplier(float multiplier)
    {
        Pac.movement.speedMultiplier *= multiplier;
    }

    public void KnockedBack(float dot)
    {
        if (reverseTime < 1)
            return;

        reverseTime = -1;
        movement.SetDirection(movement.Direction*dot);

    }

    public void ResetState()
    {
        this.Pac.movement.ResetState();
        this.gameObject.SetActive(true);
        Pac.movement.speedMultiplier = 1;
        Pac.IsAlive = true;
    }
}
