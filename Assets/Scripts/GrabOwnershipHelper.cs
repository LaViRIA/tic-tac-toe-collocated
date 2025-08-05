using Oculus.Interaction.HandGrab;
using Unity.Netcode;
using UnityEngine;

public class GrabOwnershipHelper : NetworkBehaviour
{
    public HandGrabInteractable[] handGrabInteractables;

    void Start()
    {
        UpdateGrabPermission();
    }

    public override void OnGainedOwnership()
    {
        UpdateGrabPermission();
    }
    public override void OnLostOwnership()
    {
        UpdateGrabPermission();
    }
    private void UpdateGrabPermission()
    {
        foreach (var hgi in handGrabInteractables)
            if (hgi != null)
                hgi.enabled = IsOwner;
    }
}
