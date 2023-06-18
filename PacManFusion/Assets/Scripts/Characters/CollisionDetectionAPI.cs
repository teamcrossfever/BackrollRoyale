using UnityEditor;
using UnityEngine;

public static class CollisionDetectionAPI
{
    public static Vector3 Depenatrate(Transform transform, Collider col, Vector3 origin, float detectRadius, LayerMask layers)
    {
        //If colliding move object outside of collision
        Collider[] colliders = Physics.OverlapSphere(origin, detectRadius, layers);
        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];

            Vector3 direction;
            float distance;
            Physics.ComputePenetration(col, origin, transform.rotation, collider, collider.transform.position, collider.transform.rotation, out direction, out distance);

            return (direction) * distance;
        }

        return Vector3.zero;
    }
}
