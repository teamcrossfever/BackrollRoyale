using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Pacman : MonoBehaviour
{
    Transform _transform;
    public float inputThreshold = 0.5f;
    Player cInput;
    Vector2 inputDirection;
    Movement movement;

    private void Start()
    {
        _transform = transform;

        cInput = Rewired.ReInput.players.GetPlayer(0);
        movement = GetComponent<Movement>();
    }
    private void Update()
    {
        inputDirection = cInput.GetAxis2DRaw("Horizontal", "Vertical");
        //Debug.Log($"MAG: {inputDirection.magnitude}");
        if (inputDirection.magnitude > inputThreshold)
        {
            if (inputDirection.y > 0)
            {
                movement.SetDirection(Vector2.up);
            }else if (inputDirection.y < 0)
            {
                movement.SetDirection(Vector2.down);
            }
            else if (inputDirection.x > 0)
            {
                movement.SetDirection(Vector2.right);
            }
            else if (inputDirection.x < 0)
            {
                movement.SetDirection(Vector2.left);
            }
        }

        float angle = Mathf.Atan2(movement.Direction.y, movement.Direction.x);
        _transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }
}
