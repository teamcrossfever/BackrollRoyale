using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
public class Stage : NetworkBehaviour
{
    public string stageName = "TEST 3D STAGE";
    [SerializeField]
    Transform[] spawnPoints;

    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer(NetworkRunner runner, RoomPlayer player)
    {
        var index = RoomPlayer.Players.IndexOf(player);
        var point = spawnPoints[0];

        var prefabId = player.id;
        var prefab = _playerPrefab;

        //Spawn Players
        var entity = runner.Spawn(
            prefab,
            point.position,
            point.rotation,
            player.Object.InputAuthority
        );

        Debug.Log($"Spawning Yokai for {player.Username} as {entity.name}");
        entity.transform.name = $"{player.Username}";
    }
}
