using NUnit.Framework;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour {
    private Renderer objectRenderer;
    private void Awake() {
        if (TryGetComponent<Renderer>(out var renderer)) {
            objectRenderer = renderer;
            return;
        }

        objectRenderer = GetComponentInChildren<Renderer>();
    }

    public void SwitchMaterial(string material) {
        Material loadMaterial = Resources.Load<Material>("Materials/" + material);

        if (loadMaterial != null) objectRenderer.material = loadMaterial;
    }
}
