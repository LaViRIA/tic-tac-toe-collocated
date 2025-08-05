using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkBehaviour
{
    [Header("Prefab debe tener NetworkObject y estar registrado en NetworkManager.")]
    public GameObject prefabToSpawn;
    public Transform spawnParent; // Desde dónde se genera (usualmente frente a la cámara)

    // Llama este método desde el UI o desde un evento
    public void OnSpawnButtonClicked()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("❌ No hay prefab asignado.");
            return;
        }

        // Opcional: Spawnea a 1 metro frente al spawnParent
        Vector3 pos = spawnParent != null
            ? spawnParent.position + spawnParent.forward * 1.0f
            : Vector3.zero;
        Quaternion rot = spawnParent != null
            ? spawnParent.rotation
            : Quaternion.identity;

        // Llama al servidor para crear el objeto
        CreateObjectServerRpc(pos, rot);
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateObjectServerRpc(Vector3 pos, Quaternion rot, ServerRpcParams rpcParams = default)
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("❌ [SERVER] Prefab no asignado.");
            return;
        }

        var go = Instantiate(prefabToSpawn, pos, rot);
        var netObj = go.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("❌ [SERVER] Prefab no tiene NetworkObject.");
            Destroy(go);
            return;

        }

        // Importante: Spawnear con ownership del cliente que lo pidió
        netObj.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

        Debug.Log($"✅ [SERVER] Spawned by client {rpcParams.Receive.SenderClientId} at {pos}");
    }
}
