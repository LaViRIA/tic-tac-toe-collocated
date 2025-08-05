using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;

public class SnapInteractableEx : SnapInteractable
{
    private List<SnapInteractor> _snappedInteractors = new ();

    protected override void SelectingInteractorAdded(SnapInteractor interactor)
    {
        base.SelectingInteractorAdded(interactor);
        Debug.Log($"🔗 SnapInteractableEx: snapped por {interactor.name}");

        var cell = GetComponent<TicTacToeCell>();
        if (cell != null)
        {
            Debug.Log("✅ Llamando a OnSnapped() desde SnapInteractableEx");
            cell.OnSnapped();
        }
        else
        {
            Debug.LogWarning("❌ TicTacToeCell NO encontrado en este GameObject");
        }
    }



    protected override void SelectingInteractorRemoved(SnapInteractor interactor)
    {
        base.SelectingInteractorRemoved(interactor);
        if (_snappedInteractors.Remove(interactor))
        {
            Debug.Log($"Objeto liberado por: {interactor.name}");
        }
    }

    public bool HasAnySnapped()
    {
        return _snappedInteractors.Count > 0;
    }
}
