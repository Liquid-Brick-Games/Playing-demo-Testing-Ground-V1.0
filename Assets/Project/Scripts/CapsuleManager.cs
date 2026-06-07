using System;
using Unity.VisualScripting;
using UnityEngine;

public class CapsuleManager : MonoBehaviour {
    public static event Action OnCapsuleMaterialChanged;

    private Renderer capsuleRenderer;
    private MaterialSwitcher materialSwitcher;

    private bool IsCapsuleMaterialChanged = false;

    private void Awake() {
        materialSwitcher = GetComponent<MaterialSwitcher>();

        if (TryGetComponent<Renderer>(out var renderer)) { capsuleRenderer = renderer; }
    }

    //*---- Публичные методы класса ----*//
    public Renderer GetCapsuleRender() {
        return capsuleRenderer;
    }

    public void ChangeCapsuleMaterial(string materialName) {
        if (materialSwitcher != null) materialSwitcher.SwitchMaterial(materialName);

        if (materialName != "Gray_Material" && !IsCapsuleMaterialChanged) {
            IsCapsuleMaterialChanged = true;

            OnCapsuleMaterialChanged?.Invoke();
        }
    }

    public void ResetCapsule() {
        IsCapsuleMaterialChanged = false;
        if (materialSwitcher != null) materialSwitcher.SwitchMaterial("Gray_Material");
    }
}
