using UnityEngine;
using Unity.Netcode;
using TMPro;

public enum Player { None = 0, A = 1, B = 2 }


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkVariable<Player> currentPlayer = new NetworkVariable<Player>(Player.A);
    private Player[,] board = new Player[3, 3];

    public Player CurrentPlayer => currentPlayer.Value;

    
    private bool gameEnded = false;


    public NetworkVariable<int> scoreA = new NetworkVariable<int>(0);
    public NetworkVariable<int> scoreB = new NetworkVariable<int>(0);


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            ResetBoardInternal();
            ResetBoardClientRpc();
        }

        ShowTurnClientRpc(currentPlayer.Value);

    }

    // ----- Jugadas -----

    [ServerRpc(RequireOwnership = false)]
    public void TryMarkServerRpc(int x, int y, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"🟧 [TryMarkServerRpc] Petición de marcar celda ({x},{y}) por jugador {currentPlayer.Value}");

        if (board[x, y] != Player.None)
        {
            return;
        }

        Player jugadorActual = currentPlayer.Value;
        board[x, y] = jugadorActual;
        Debug.Log($"✅ Marcado exitoso en ({x},{y}) por {jugadorActual}");
        UpdateCellClientRpc(x, y, jugadorActual);

        if (CheckWinner(jugadorActual))
        {
            Debug.Log($"🏆 ¡Jugador {jugadorActual} gana!");
            if (jugadorActual == Player.A) scoreA.Value++;
            else if (jugadorActual == Player.B) scoreB.Value++;

            ShowOutcomeClientRpc(jugadorActual); // <--- ¡ESTO ya informa a todos inmediatamente!
            return; // ¡NO dejes seguir la jugada!
        }
        else if (IsDraw())
        {
            Debug.Log("🤝 Empate detectado");
            ShowDrawClientRpc();
            return;
        }
        else
        {
            currentPlayer.Value = (jugadorActual == Player.A) ? Player.B : Player.A;
            Debug.Log($"➡️ Turno siguiente: {currentPlayer.Value}");
            ShowTurnClientRpc(currentPlayer.Value);
        }
    }

    [ClientRpc]
    void UpdateCellClientRpc(int x, int y, Player player)
    {
        board[x, y] = player;
        // Aquí actualiza la UI o modelo visual de la celda
        // Ejemplo: BoardUI.Instance?.MarkCell(x, y, player);
    }



    // ----- Reset -----

    public void OnResetButton()
    {
        if (!IsServer) return;
        ResetBoardInternal();
        ResetBoardClientRpc();
    }

    private void ResetBoardInternal()
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                board[x, y] = Player.None;

        currentPlayer.Value = Player.A;
    }

    [ClientRpc]
    void ResetBoardClientRpc()
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                board[x, y] = Player.None;

        // Limpia tu UI aquí (visual del tablero)
        // UIManager.Instance?.ResetBoardVisual();
        ShowTurnClientRpc(Player.A); // Reinicia indicador de turno
    }

    // ----- UI: Turnos, Victoria, Empate -----

    [ClientRpc]
    void ShowOutcomeClientRpc(Player winner)
    {
        UIManager.Instance?.ShowOutcome(winner);
        UIManager.Instance?.UpdateScore(scoreA.Value, scoreB.Value);
        UIManager.Instance?.ShowRestartButtons(); // <--- ¡Agrega esto!
    }

    [ClientRpc]
    void ShowDrawClientRpc()
    {
        UIManager.Instance?.ShowDraw();
        UIManager.Instance?.ShowRestartButtons(); // <--- ¡Agrega esto!
    }


    [ClientRpc]
    void ShowTurnClientRpc(Player turno)
    {
        UIManager.Instance?.ShowTurn(turno);
        Debug.Log($"[CLIENT] Turno de: {turno}");
    }

    // ----- Utilidades -----

    public Player GetCell(int x, int y) => board[x, y];

    public bool CheckWinner(Player p)
    {
        for (int i = 0; i < 3; i++)
        {
            if ((board[i, 0] == p && board[i, 1] == p && board[i, 2] == p))
                return true;
            if ((board[0, i] == p && board[1, i] == p && board[2, i] == p))
                return true;
        }
        if (board[0, 0] == p && board[1, 1] == p && board[2, 2] == p)
            return true;
        if (board[2, 0] == p && board[1, 1] == p && board[0, 2] == p)
            return true;
        return false;
    }

    public bool IsDraw()
    {
        foreach (var cell in board)
            if (cell == Player.None) return false;
        return true;
    }

    

    [ServerRpc(RequireOwnership = false)]
    public void RestartGame_ServerRpc(ServerRpcParams rpcParams = default)
    {
        ResetBoardAndClearPiecesClientRpc();
    }



    [ClientRpc]
    void ResetBoardAndClearPiecesClientRpc()
    {
        // Borra piezas físicas (ahora usando CubeSpawner)
        var spawner = FindAnyObjectByType<CubeSpawner>();
        if (spawner != null)
            spawner.ResetAllCubes();

        // Limpia celdas visuales:
        UIManager.Instance?.ResetBoardVisual();

        // Oculta los botones de reinicio:
        UIManager.Instance?.HideRestartButtons();

        // Reinicia indicadores de turno:
        ShowTurnClientRpc(Player.A);

        Debug.Log("[CLIENT] Reinicio de tablero y piezas (puntaje conservado)");
    }







}
