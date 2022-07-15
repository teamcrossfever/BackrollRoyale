using System.Collections;
using UnityEngine;
using Fusion;
public class PlayerNet : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    [SerializeField]
    private Ball _prefabBall;
    [SerializeField]
    private PhysxBall _prefabPhysxBall;

    private Vector3 _forward;

    [Networked]
    private TickTimer delay { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;
            else
                _forward = transform.forward;

            if (delay.ExpiredOrNotRunning(Runner)) //Check if the timer is done 
            {
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
                    {
                        //Initalize before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
                }
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
                    {
                        o.GetComponent<PhysxBall>().Init(10 * _forward);
                    });
                }
            }

            
        }
    }
}
