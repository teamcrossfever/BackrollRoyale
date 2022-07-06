using System.Collections;
using UnityEngine;
using Fusion;

public class PacComponent : NetworkBehaviour
{
    public PacEntity Pac { get; private set; }

    public virtual void Init(PacEntity pac)
    {
        Pac = pac;
    }

    public virtual void OnBattleStart() { }
}
