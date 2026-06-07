using UnityEngine;

public class UI_Interact : MonoBehaviour {
    [SerializeField] private GameObject interactAction;

    private void OnEnable() {
        P_ColorManager.OnZoneEntered += ShowInteractAction;
        P_ColorManager.OnZoneExited += HideInteractAction;
    }

    private void OnDisable() {
        P_ColorManager.OnZoneEntered -= ShowInteractAction;
        P_ColorManager.OnZoneExited -= HideInteractAction;
    }

    private void ShowInteractAction() {
        interactAction.SetActive(true);
    }

    private void HideInteractAction() {
        interactAction.SetActive(false);
    }
}
