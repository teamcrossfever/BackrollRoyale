using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer { get; private set; }

    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int CurrentFrame { get; private set; }
    public bool loop = true;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }

    private void Advance()
    {
        CurrentFrame++;
        if(CurrentFrame >= sprites.Length && loop)
        {
            CurrentFrame = 0;
        }

        SpriteRenderer.sprite = sprites[CurrentFrame];
    }

    public void Reset()
    {
        CurrentFrame = -1;
        Advance();
    }
}
