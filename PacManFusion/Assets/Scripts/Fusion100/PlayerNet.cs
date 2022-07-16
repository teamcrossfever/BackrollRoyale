using System.Collections;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
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

    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }


    private Text _message;

    //Mat
    private Material _material;
    public Material Material {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;

            return _material;
        }
    }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    private void Update()
    {
        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("HELLO PEOPLES");
        }
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
                    spawned = !spawned;
                }
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) =>
                    {
                        o.GetComponent<PhysxBall>().Init(10 * _forward);
                    });
                    spawned = !spawned;
                }
            }

            
        }
    }

    public override void Render()
    {
        Material.color = Color.Lerp(Material.color, Color.blue, Time.deltaTime);
    }

    public static void OnBallSpawned(Changed<PlayerNet> changed)
    {
        changed.Behaviour.Material.color = Color.white;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (!_message)
        {
            _message = FindObjectOfType<Text>();
        }

        if (info.IsInvokeLocal)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player: {message}\n";
        }

        _message.text += message;
    }
}
