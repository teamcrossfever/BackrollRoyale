using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set; }
    public float duration;
    public LayerMask layerInteraction;

    private void Awake()
    {
        this.ghost = GetComponent<Ghost>();
        this.enabled = false;
    }

    public void Enable()
    {
        Enable(this.duration);
    }

    public virtual void Enable(float duration)
    {
        this.enabled = true;
        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        this.enabled = false;
        CancelInvoke();
    }
}
