using System.Collections;
using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class PacEntity : PacComponent
{
    public static event Action<PacEntity> OnPacSpawned;
    public static event Action<PacEntity> OnPacDespawned;

    public PacAnimator Animator;
    public PacController Controller;
}
