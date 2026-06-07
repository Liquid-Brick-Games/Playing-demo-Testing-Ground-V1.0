using UnityEngine;

public class FollowTarget : MonoBehaviour {
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 1f);

    private void Awake() {
        if (targetPosition != null) transform.position = targetPosition.position + offset;
    }
    private void LateUpdate() {
        if (targetPosition != null) transform.position = targetPosition.position + offset;
    }
}
