using UnityEngine;

public class TicTacToeCell : MonoBehaviour
{
    public int x, y;

    // Llama esto cuando el usuario quiera marcar la celda (por ejemplo, con SnapInteractable o click)
    public void OnCellSelected()
    {
        // Solo manda el request, el host valida
        if (GameManager.Instance != null && GameManager.Instance.IsSpawned)
        {
            GameManager.Instance.TryMarkServerRpc(x, y);
        }
    }

    // Si quieres mostrar el estado visual de la celda, puedes usar un método
    public void SetVisual(Player player)
    {
        // Cambia sprite, color, texto, etc., según el valor de 'player'
    }

    public void OnSnapped()
    {
        Debug.Log($" [OnSnapped] Pieza snappeada en celda ({x},{y})");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryMarkServerRpc(x, y);
        }
        else
        {
            Debug.LogWarning("[OnSnapped] GameManager.Instance es null");
        }
    }

}
