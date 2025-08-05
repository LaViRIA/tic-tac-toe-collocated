using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject turno1Indicator; 
    public GameObject turno2Indicator; 

    public GameObject botonReadyA; 
  //  public GameObject botonReadyB;

    public TextMeshPro scoreAText;
    public TextMeshPro scoreBText;

    public GameObject banderaA;         // Arrastra aquí la bandera de Jugador A
    public GameObject banderaB;         // Arrastra aquí la bandera de Jugador B
    public GameObject empateObject; // Arrastra aquí tu objeto visual de empate

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScore(int scoreA, int scoreB)
    {
        if (scoreAText != null)
            scoreAText.text = $"{scoreA}";
        if (scoreBText != null)
            scoreBText.text = $"{scoreB}";
    }

    public void ShowTurn(Player turno)
    {
        if (turno1Indicator != null)
            turno1Indicator.SetActive(turno == Player.A);
        if (turno2Indicator != null)
            turno2Indicator.SetActive(turno == Player.B);

        Debug.Log($"[UI] Turno visual cambiado a: {turno}");
    }

   
    public void ResetBoardVisual()
    {
        if (banderaA != null) banderaA.SetActive(false);
        if (banderaB != null) banderaB.SetActive(false);
        if (empateObject != null) empateObject.SetActive(false);

        Debug.Log("[UI] Tablero visual reiniciado.");
        foreach (var cell in FindObjectsByType<TicTacToeCell>(FindObjectsSortMode.None))
        {
            cell.SetVisual(Player.None);
        }

    }
    public void OnRestartButtonPressed()
    {
        if (NetworkManager.Singleton.IsHost)
            GameManager.Instance.RestartGame_ServerRpc();
    }


    public void ShowRestartButtons()
    {
        if (NetworkManager.Singleton.IsHost && botonReadyA != null)
            botonReadyA.SetActive(true);
        // Si tienes un botón para el cliente, déjalo desactivado
    }


    public void HideRestartButtons()
    {
        if (botonReadyA != null)
            botonReadyA.SetActive(false);
    }



    public void ShowOutcome(Player winner)
    {
        // Oculta todo primero
        if (banderaA != null) banderaA.SetActive(false);
        if (banderaB != null) banderaB.SetActive(false);
        if (empateObject != null) empateObject.SetActive(false);

        // Activa la bandera del ganador
        if (winner == Player.A && banderaA != null)
            banderaA.SetActive(true);
        else if (winner == Player.B && banderaB != null)
            banderaB.SetActive(true);

        Debug.Log($"[UI] Bandera mostrada para: {winner}");
    }

    public void ShowDraw()
    {
        // Oculta banderas y muestra solo el objeto visual de empate
        if (banderaA != null) banderaA.SetActive(false);
        if (banderaB != null) banderaB.SetActive(false);

        if (empateObject != null)
            empateObject.SetActive(true);

        Debug.Log("[UI] Mostrando objeto visual de empate");
    }

}
