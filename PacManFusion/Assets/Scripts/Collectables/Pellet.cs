﻿using UnityEditor;
using UnityEngine;


public class Pellet : MonoBehaviour, ICollectable
{
    public bool isPower;
    public int points = 10;

    public bool Collect()
    {
        gameObject.SetActive(false);
        return true;
    }
}