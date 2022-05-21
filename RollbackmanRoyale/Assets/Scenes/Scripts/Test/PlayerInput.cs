using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public DirectionTesting controller;

    Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<DirectionTesting>();
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = -Input.GetAxis("Vertical");

        controller.Track(-input.y*20*Time.deltaTime);
        controller.SetZVelocity(input.x * 10);
    }
}
