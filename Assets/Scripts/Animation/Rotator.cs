using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationPerSecond;

    private void FixedUpdate()
    {
        transform.Rotate(rotationPerSecond * Time.deltaTime);
    }
}
