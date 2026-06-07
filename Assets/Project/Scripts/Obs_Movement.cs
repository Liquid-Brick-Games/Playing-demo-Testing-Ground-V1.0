using UnityEngine;

public class Obs_Movement : MonoBehaviour {
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;

    private void Start() {
        startPosition = transform.position;
    }

    private void Update() {
        float sineWave = Mathf.Sin(Time.time * speed);

        Vector3 offset = moveDirection.normalized * (sineWave * distance);
        transform.position = startPosition + offset;
    }
}
