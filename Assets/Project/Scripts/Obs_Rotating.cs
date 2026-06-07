using UnityEngine;

public class Obs_Rotating : MonoBehaviour {
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float speed = 2f;

    private Quaternion startRotation;

    private void Start() {
        startRotation = transform.rotation;
    }

    private void Update() {
        float sineWave = Mathf.Sin(Time.time * speed);
        float currentAngle = sineWave * maxAngle;

        Quaternion rotationOffset = Quaternion.AngleAxis(currentAngle, rotationAxis);
        transform.rotation = startRotation * rotationOffset;
    }
}
