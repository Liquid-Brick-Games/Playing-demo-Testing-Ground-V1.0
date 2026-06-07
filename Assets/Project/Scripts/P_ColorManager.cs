using System;
using UnityEngine;

public class P_ColorManager : MonoBehaviour {
    public static event Action OnZoneEntered;
    public static event Action OnZoneExited;

    private Renderer playerRenderer;
    private InputActions inputActions;

    private CylinderManager cylinderRenderer;
    private CapsuleManager capsuleRenderer;

    private MaterialSwitcher materialSwitcher;

    private void Awake() {
        inputActions = new InputActions();
        playerRenderer = GetComponentInChildren<Renderer>();
        materialSwitcher = GetComponent<MaterialSwitcher>();
    }

    private void OnEnable() {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable() {
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        GameManager.OnGameStateChanged -= HandleGameStateChanged;

        inputActions.Player.Disable();
    }

    private void HandleGameStateChanged(GameManager.GameState gameState) {
        if (gameState == GameManager.GameState.Playing) { inputActions.Player.Enable(); }
        else { inputActions.Player.Disable(); }
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        string playerMaterialName = playerRenderer.sharedMaterial.name;

        if (cylinderRenderer != null) {

            if (playerMaterialName != "White_Material") return;

            if (cylinderRenderer.GetCylinderRenderer().sharedMaterial.name != "White_Material") {
                materialSwitcher.SwitchMaterial(cylinderRenderer.GetCylinderRenderer().sharedMaterial.name);
                cylinderRenderer.ChangeCylinderMaterial("White_Material");
            }

            return;
        }

        if (capsuleRenderer != null) {
            string capsuleMaterialName = capsuleRenderer.GetCapsuleRender().sharedMaterial.name;

            if (capsuleMaterialName == "Gray_Material" && playerMaterialName != "White_Material") {

                capsuleRenderer.ChangeCapsuleMaterial(playerMaterialName);
                materialSwitcher.SwitchMaterial("White_Material");
            }

            return;
        }

    }

    private void OnCollisionEnter(Collision collision) {
        GameObject hitObject = collision.gameObject;

        if (hitObject.TryGetComponent<CylinderManager>(out var cylinder)) {
            cylinderRenderer = cylinder;
            OnZoneEntered?.Invoke();
        }

        if (hitObject.TryGetComponent<CapsuleManager>(out var capsule)) {
            capsuleRenderer = capsule;
            OnZoneEntered?.Invoke();
        }

    }

    private void OnCollisionExit(Collision collision) {
        GameObject hitObject = collision.gameObject;

        if (hitObject.TryGetComponent<CylinderManager>(out var cylinder)) {
            cylinderRenderer = null;
            OnZoneExited?.Invoke();
        }

        if (hitObject.TryGetComponent<CapsuleManager>(out var capsule)) {
            capsuleRenderer = null;
            OnZoneExited?.Invoke();
        }

    }


    //*---- Публичные методы класса ----*//
    public Renderer GetPlayerRenderer() {
        return playerRenderer;
    }

    public void ResetPlayerMaterial() {
        if (materialSwitcher != null) materialSwitcher.SwitchMaterial("White_Material");
    }
}
