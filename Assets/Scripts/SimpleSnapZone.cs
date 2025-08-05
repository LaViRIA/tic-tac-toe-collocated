using UnityEngine;

public class SimpleSnapZone : MonoBehaviour
{
    public string allowedTag = "Snappable";

    private void OnTriggerEnter(Collider other)
    {
        // Solo hace snap si el objeto tiene el tag adecuado y NO est� siendo agarrado
        if (other.CompareTag(allowedTag))
        {
            // �El objeto est� siendo agarrado? No hagas nada si s�
            var rb = other.attachedRigidbody;
            if (rb != null && rb.linearVelocity.magnitude > 0.1f) return;

            // Haz snap (alinea el objeto al Snap Zone)
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;
        }
    }
}
