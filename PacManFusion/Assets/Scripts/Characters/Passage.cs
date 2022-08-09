using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    public Transform connection;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 pos = collision.transform.position;
        pos.x = connection.position.x;
        pos.y = connection.position.y;

        collision.transform.position = pos;
    }
}
