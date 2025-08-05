using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public float spacing = 0.3f;

    void Start()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("cellPrefab NO asignado");
            return;
        }

        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3 pos = transform.position + new Vector3(x * spacing, 0f, y * spacing);
                var c = Instantiate(cellPrefab, pos, rot, transform);
                c.name = $"Cell_{x}_{y}";

                // Asigna coordenadas a la celda
                var cellComponent = c.GetComponentInChildren<TicTacToeCell>();
                if (cellComponent != null)
                {
                    cellComponent.x = x;
                    cellComponent.y = y;
                   // Debug.Log($"🚩 Asignado: {c.name} tiene x={cellComponent.x}, y={cellComponent.y}");
                }
                else
                {
                    //Debug.LogWarning("CellPrefab no tiene TicTacToeCell");
                }
            }
        }
    }
}
