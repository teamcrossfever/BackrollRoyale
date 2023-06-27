using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Movement3D : NetworkTransform
{
    Transform _transform;

    bool hasInitialized = false;

    public float speed = 8f;
    public float speedMultiplier = 1;

    float detectRadius = 0.5f;
    [SerializeField]
    float detectWidth = 0.5f;

    Collider col;
    bool isColliding = false;

    Vector3 CenterPos { get => _transform.position + Vector3.up * 0.5f; }

    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    [Networked]
    public Vector3 Direction { get; private set; }
    [Networked]
    public Vector3 NextDirection { get; private set; }
    [Networked]
    public Vector3 StartPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        //CacheController();
    }

    public override void Spawned()
    {
        base.Spawned();
        Initialize();

    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (hasInitialized)
            return;

        _transform = transform;
        StartPosition = _transform.position;
        Direction = initialDirection;
        col = GetComponent<Collider>();
        hasInitialized = true;
    }

    private void Update()
    {

        isColliding = IsColliding(Direction);
    }
    private void FixedUpdate()
    {
        //HandleMovement();
    }

    public void HandleMovement()
    {
        if (!isColliding)
        {
            Vector3 pos = _transform.position;
            Vector3 translation = Direction * speed * speedMultiplier * Time.fixedDeltaTime;
            _transform.Translate(translation);
        }
        else
        {
            Vector3 newPos = _transform.position;
            newPos.x = Mathf.Round(newPos.x);
            newPos.z = Mathf.Round(newPos.z);
            _transform.position = newPos;
        }

        Depenatrate();
    }

    void Depenatrate()
    {
        _transform.position+= CollisionDetectionAPI.Depenatrate(_transform, col, CenterPos, detectRadius, obstacleLayer);
    }


    public void SetDirection(Vector3 direction, bool forced = false)
    {
        if (forced || !IsColliding(direction))
        {
            Direction = direction;
            NextDirection = Vector3.zero;
        }
        else
        {
            NextDirection = direction;
        }
    }

    public bool IsColliding(Vector3 direction)
    {
        Vector3 sideOffset = Quaternion.Euler(0, -90, 0) * direction*detectWidth;

        Ray ray = new Ray(CenterPos + sideOffset, direction);
        Ray ray2 = new Ray(CenterPos- sideOffset, direction);

#if UNITY_EDITOR
   
        Debug.DrawRay(ray.origin, ray.direction*detectRadius, Color.red);
        Debug.DrawRay(ray2.origin, ray.direction*detectRadius, Color.green);
#endif

        if (Physics.Raycast(ray, detectRadius,obstacleLayer))
        {
            return true;
        }

        if (Physics.Raycast(ray2, detectRadius, obstacleLayer))
        {
            return true;
        }

        return false;
    }

    public void ResetState()
    {
        speedMultiplier = 1;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        _transform.position = StartPosition;
        enabled = true;
    }
}
