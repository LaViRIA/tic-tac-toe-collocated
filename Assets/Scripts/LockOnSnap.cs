using UnityEngine;
using Oculus.Interaction;

public class LockOnSnap : MonoBehaviour
{
    private SnapInteractable snapInteractable;

    void Start()
    {
        snapInteractable = GetComponent<SnapInteractable>();
        if (snapInteractable != null)
        {
            snapInteractable.WhenSelectingInteractorViewAdded += OnPieceSnapped;
        }
    }

    private void OnPieceSnapped(IInteractorView interactorView)
    {
        // El SnapInteractable tiene la referencia del objeto snapeado en el último frame, pero NO es público.
        // Solución: buscar en los hijos del snap, el objeto con Grabbable activo (o NetworkObject si es multiplayer).

        foreach (Transform child in transform)
        {
            var grabbable = child.GetComponent<Grabbable>();
            if (grabbable != null)
            {
                grabbable.enabled = false; // Bloquea la pieza
            }
        }
    }
}
