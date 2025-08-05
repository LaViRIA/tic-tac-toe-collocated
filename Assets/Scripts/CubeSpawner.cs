using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CubeSpawner : NetworkBehaviour
{
    public GameObject[] piecePrefabs; // 0 = X, 1 = O
    public Transform[] spawnZones;    // 0 = Zona X, 1 = Zona O

    // Diccionario: <ownerClientId, Lista de cubos>
    private Dictionary<ulong, List<NetworkObject>> spawnedCubes = new();

    // El jugador pide spawn, pasando el tipo de pieza (ej. 0 para X, 1 para O)
    public void RequestSpawnCube(int pieceType)
    {
        if (!IsServer)
            SpawnCubeServerRpc(pieceType);
        else
            SpawnCubeForClient(OwnerClientId, pieceType);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnCubeServerRpc(int pieceType, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        SpawnCubeForClient(clientId, pieceType);
    }

    void SpawnCubeForClient(ulong ownerClientId, int pieceType)
    {
        // Limitar a 5 cubos por cliente
        if (!spawnedCubes.ContainsKey(ownerClientId))
            spawnedCubes[ownerClientId] = new List<NetworkObject>();

        if (spawnedCubes[ownerClientId].Count >= 5)
        {
            Debug.Log($"Jugador {ownerClientId} ya tiene 5 piezas.");
            return;
        }

        // Seguridad: asegúrate de que el índice es válido
        if (pieceType < 0 || pieceType >= piecePrefabs.Length || pieceType >= spawnZones.Length)
        {
            Debug.LogError("Tipo de pieza o zona fuera de rango");
            return;
        }

        var pos = spawnZones[pieceType].position;
        var rot = spawnZones[pieceType].rotation;
        var cube = Instantiate(piecePrefabs[pieceType], pos, rot);
        var netObj = cube.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(ownerClientId);

        // Agrega el cubo a la lista
        spawnedCubes[ownerClientId].Add(netObj);

        // Se subscribe a la notificación de despawn
        var despawner = cube.GetComponent<CubeDespawnNotifier>();
        if (despawner != null)
        {
            despawner.OnDespawned += () =>
            {
                if (spawnedCubes.ContainsKey(ownerClientId))
                    spawnedCubes[ownerClientId].Remove(netObj);
            };
        }
    }

    // Función para destruir un cubo por botón (debes implementarla donde la necesites)
    public void DestroyCube(NetworkObject netObj, ulong ownerClientId)
    {
        if (netObj != null)
        {
            netObj.Despawn();
            if (spawnedCubes.ContainsKey(ownerClientId))
            {
                spawnedCubes[ownerClientId].Remove(netObj);
            }
        }
    }

    public void ResetAllCubes()
    {
        // Destruye todas las piezas y limpia el diccionario
        foreach (var kvp in spawnedCubes)
        {
            foreach (var netObj in kvp.Value)
            {
                if (netObj != null && netObj.IsSpawned)
                    netObj.Despawn();
            }
        }
        spawnedCubes.Clear();
        Debug.Log("[CubeSpawner] Todas las piezas eliminadas y contador reseteado.");
    }

}
