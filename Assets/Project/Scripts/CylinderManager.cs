using UnityEngine;

public class CylinderManager : MonoBehaviour {
    private Renderer cylinderRenderer;
    private MaterialSwitcher materialSwitcher;

    private void Awake() {
        materialSwitcher = GetComponent<MaterialSwitcher>();

        if (TryGetComponent<Renderer>(out var renderer)) cylinderRenderer = renderer;
    }

    /*---- Публичные методы ----*/
    public Renderer GetCylinderRenderer() {
        return cylinderRenderer;
    }

    public void ChangeCylinderMaterial(string materialName) { if (materialSwitcher != null) materialSwitcher.SwitchMaterial(materialName); }

    public void ResetCylinderMaterial(string materialName) {
        if (materialSwitcher != null) materialSwitcher.SwitchMaterial(materialName);
    }
}
