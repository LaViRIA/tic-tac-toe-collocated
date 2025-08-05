using Unity.Netcode;
using UnityEngine;

public class OwnershipDebugger : NetworkBehaviour
{
    Renderer _renderer;
    void Start() { _renderer = GetComponent<Renderer>(); }

    void Update()
    {
        if (!IsSpawned) return;
        if (_renderer != null)
            _renderer.material.color = IsOwner ? Color.green : Color.red;

        if (IsOwner && Input.GetKeyDown(KeyCode.Space))
            Debug.Log($"[Ownership] Yo ({NetworkManager.Singleton.LocalClientId}) SOY el dueño del objeto {NetworkObjectId}");
    }
}
