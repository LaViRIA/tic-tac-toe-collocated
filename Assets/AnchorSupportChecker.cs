using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;

public class AnchorSupportChecker : MonoBehaviour
{
    [SerializeField] ARAnchorManager anchorManager;

    void Start()
    {
        var metaSubsystem = (MetaOpenXRAnchorSubsystem)anchorManager.subsystem;
        bool supported = metaSubsystem.isSharedAnchorsSupported == UnityEngine.XR.ARSubsystems.Supported.Supported;
        Debug.Log($"🌐 Shared Anchors supported: {supported}");
    }
}
