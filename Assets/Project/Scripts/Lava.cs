using UnityEngine;

public class Lava : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        GameObject hitObject = collision.gameObject;

        if (hitObject.TryGetComponent<P_Movement>(out var component)) component.TeleportToStar();
    }
}
