using UnityEngine;
using System;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;

public class ColocationManager : MonoBehaviour
{
    public static ColocationManager Instance { get; private set; }
    public event Action<int, int> OnMoveReceived;

    [Tooltip("Marcar como host para pruebas en simulador o establecer vía UI en Quest.")]
    public bool isHost = false; // Selecciona en Inspector o asigna por UI

    private ColocationDiscoveryFeature colocationFeature;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    async void Start()
    {
        var feature = OpenXRSettings.Instance.GetFeature<ColocationDiscoveryFeature>();
        if (feature == null)
        {
            Debug.LogError("[ColocationManager] ColocationDiscoveryFeature no habilitado");
            return;
        }
        colocationFeature = feature;

        // Suscribirse al evento de mensajes
        colocationFeature.messageDiscovered += OnMessageDiscovered;

#if UNITY_EDITOR
        Debug.Log($"[ColocationManager] (Simulador) isHost = {isHost}");
#else
        Debug.Log($"[ColocationManager] (Quest) isHost = {isHost}");
#endif

        // SOLO UNO de los modos según el rol seleccionado
        if (isHost)
        {
            await colocationFeature.TryStartAdvertisementAsync(new byte[0]);
            Debug.Log($"[Host] Estado advertisement: {colocationFeature.advertisementState}");
        }
        else
        {
            await colocationFeature.TryStartDiscoveryAsync();
            Debug.Log($"[Cliente] Estado discovery: {colocationFeature.discoveryState}");
        }
    }

    void OnMessageDiscovered(object sender, ColocationDiscoveryMessage msg)
    {
        var data = msg.data.ToArray();
        if (data.Length >= 2)
        {
            int x = data[0], y = data[1];
            Debug.Log($"[ColocationManager] MOVE recibido -> ({x},{y})");
            OnMoveReceived?.Invoke(x, y);
        }
        else
        {
            Debug.Log($"[ColocationManager] Mensaje recibido de longitud inesperada: {data.Length}");
        }
    }

    public async void SendMove(int x, int y)
    {
        var msg = new byte[] { (byte)x, (byte)y };
        Debug.Log($"[ColocationManager] Enviando MOVE -> ({x},{y})");
        await colocationFeature.TryStartAdvertisementAsync(msg);
    }

    // Método para cambiar rol en runtime (para UI o pruebas)
    public void SetHostRole(bool value)
    {
        isHost = value;
        Debug.Log($"[ColocationManager] Rol cambiado en runtime. isHost = {isHost}");
        // Puedes reiniciar colocalización aquí si lo deseas.
    }

    public async void SendReset()
    {
        var msg = new byte[] { 99, 99 }; // Un valor especial que nunca será jugada real
        Debug.Log("[ColocationManager] Enviando RESET global");
        await colocationFeature.TryStartAdvertisementAsync(msg);
    }
}
