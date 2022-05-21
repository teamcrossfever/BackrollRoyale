using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTesting : MonoBehaviour
{
    public Transform target;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    public void Track(float input)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        UnityEngine.Debug.DrawRay(transform.position, direction, Color.green);

        float r = Vector3.Distance(target.position, transform.position);
        float angle = (input / (2 * Mathf.PI * r)) * 360;

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = Quaternion.LookRotation(direction);
        rb.MovePosition(target.position + (rot * Vector3.Normalize(transform.position - target.position) * r));


        //transform.RotateAround(target.position, target.up, input);
    }

    public void SetXVelocity(float x)
    {
        Vector3 vel = transform.InverseTransformDirection(rb.velocity);
        vel.x = x;
        rb.velocity = transform.TransformDirection(vel);
    }

    public void SetZVelocity(float z)
    {
        Vector3 vel = transform.InverseTransformDirection(rb.velocity);
        vel.z = z;
        rb.velocity = transform.TransformDirection(vel);
    }
}
