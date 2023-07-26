using System.Collections;
using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class PacEntity : PacComponent
{
    public static event Action<PacEntity> OnPacSpawned;
    public static event Action<PacEntity> OnPacDespawned;

    public Transform graphics;
    public PacAnimator Animator;
    public PacController Controller;
    public Pacman pacman;
    public Movement3D movement;

    public bool IsAlive { get; set; }

    private void Awake()
    {
        IsAlive = true;
        movement = GetComponent<Movement3D>();
        var components = GetComponentsInChildren<PacComponent>();
        foreach (var component in components) component.Init(this);
    }

    /*
    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {

        }

        OnPacSpawned?.Invoke(this);
    }
    */
}
