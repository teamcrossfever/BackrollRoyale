using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostBehavior initalBehavior;

    public Transform target;

    public int points = 200;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();

        target = FindObjectOfType<Pacman>().transform;
    }

    private void Start()
    {
        if (!target)
            target = FindObjectOfType<Transform>();

        ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        chase.Disable();
        frightened.Disable();
        scatter.Enable();
        if(home!= initalBehavior)
        {
            home.Disable();
        }

        if (initalBehavior)
            initalBehavior.Enable();
    }
}
