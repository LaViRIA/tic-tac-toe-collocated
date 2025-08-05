using UnityEngine;
using Unity.Netcode;

public class DebugOwnershipColor : NetworkBehaviour
{
    private Renderer _rend;

    private void Start()
    {
        _rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!IsSpawned) return;

        _rend.material.color = IsOwner ? Color.green : Color.red;
    }
}
