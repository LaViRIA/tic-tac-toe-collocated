using Unity.Netcode;
using UnityEngine;
using System;

public class CubeDespawnNotifier : NetworkBehaviour
{
    public Action OnDespawned;

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (OnDespawned != null) OnDespawned.Invoke();
    }
}
