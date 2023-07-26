using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Oni : MonoBehaviour
{
    public enum OniStates
    {
        None,
        Chase,
        Fear,
        Wonder,
        Return
    }

    /// <summary>
    /// Match Manager
    /// </summary>
    MatchManager mm;

    [SerializeField]
    public OniStates OniState { private set; get; }
    [SerializeField]
    float speed = 3;
    [SerializeField]
    float speedReturn = 8;
    float speedMultiplier = 1;
    [SerializeField]
    float maxChaseDuration = 10;
    float chaseTimer;
    [SerializeField]
    float lookAheadDistance = 0;
    public int points;

    [SerializeField]
    GameObject returnEffect;

    Renderer rdr;

    Vector3 initialPosition;

    private Transform target;
    private PacEntity pacTarget;

    Animator animator;

    NavMeshAgent agent;

    [SerializeField]
    NavMeshPath path;

    float fearTime = 0;

    public void Initialize()
    {
        mm = FindObjectOfType<MatchManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        animator.enabled = false;

        rdr = GetComponentInChildren<Renderer>();
        returnEffect.SetActive(false);

        initialPosition = transform.position;

        ChangeSpeedMultiplier(speedMultiplier);
        Chase();
        
    }

    public void ChangeSpeedMultiplier(float multiplier)
    {
        speedMultiplier *= multiplier;
        agent.speed = speed * speedMultiplier;
    }
    public void EntityUpdate(float delta)
    {
        if (animator)
        {
            animator.Update(delta);
        }

        if (!target)
            return;

        if (delta <= 0)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        switch (OniState)
        {
            case OniStates.Chase:
                chaseTimer += delta;
                if (pacTarget && !pacTarget.IsAlive)
                {
                    ChangeTarget(mm.RandomPlayer(true), OniStates.Chase);
                }

                agent.SetDestination(CalculateLookAhead());

                if(chaseTimer>= maxChaseDuration)
                {
                    chaseTimer = 0;
                    Chase();
                }
                break;

            case OniStates.Wonder:
                //agent.SetPath()
                break;

            case OniStates.Fear:
                fearTime -= delta;
                if(Vector3.Distance(transform.position, target.position) < 2)
                {
                    ChangeTarget(mm.FindFarEscapePoint(transform), OniStates.Fear);
                }

                agent.SetDestination(target.position);
                if (fearTime <= 0)
                {
                    fearTime = 0;
                    Chase(true);
                }
                break;

            case OniStates.Return:
                fearTime = 0;
                agent.SetDestination(initialPosition);
                if (Vector3.Distance(transform.position, initialPosition) < 1)
                {
                    agent.speed = speed * speedMultiplier;
                    rdr.enabled = true;
                    returnEffect.SetActive(false);
                    Chase(true);
                }
                break;
        }
    }

    public void ChangeTarget(Transform target, OniStates state)
    {
        this.target = target;

        if(target)
            this.pacTarget = target.GetComponent<PacEntity>();

        this.OniState = state;
    }

    Vector3 CalculateLookAhead()
    {
        if (!target)
            return initialPosition;

        if (!pacTarget || (pacTarget && lookAheadDistance==0))
            return target.position;



        //Close in when near
        if(Vector3.Distance(target.position, transform.position) < lookAheadDistance)
        {
            return target.position;
        }

        RaycastHit hit;
        if(Physics.Raycast(pacTarget.transform.position, pacTarget.movement.Direction, out hit, lookAheadDistance, LayerMask.GetMask(Layers.Obstacles)))
        {
            return hit.point;
        }

        return pacTarget.transform.position + pacTarget.movement.Direction * lookAheadDistance;
    }
    public void ResetState()
    {
        speedMultiplier = 1;
        transform.position = initialPosition;
        gameObject.SetActive(true);
        rdr.enabled = true;
        returnEffect.gameObject.SetActive(false);
        ChangeTarget(mm.RandomPlayer(true), OniStates.Chase);
    }

    public void Chase(bool ignore=false)
    {
        if (!ignore)
        {
            if (OniState == OniStates.Fear || OniState == OniStates.Return)
                return;
        }

        ChangeTarget(mm.RandomPlayer(true), OniStates.Chase);
    }
    public void Frightened(float duration)
    {
        ChangeTarget(mm.FindFarEscapePoint(transform), OniStates.Fear);
        fearTime = duration;
    }

    public void Eaten()
    {
        if (OniState == OniStates.Return)
            return;

        agent.speed = 8*speedMultiplier;
        rdr.enabled = false;
        returnEffect.SetActive(true);
        ChangeTarget(this.transform, Oni.OniStates.Return);
    }

}
