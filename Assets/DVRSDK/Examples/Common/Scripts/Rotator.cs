using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 Speed;

    void Update()
    {
        transform.Rotate(Speed * Time.deltaTime);
    }
}
