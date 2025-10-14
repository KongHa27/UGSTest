using UnityEngine;

public class EffectRotate : MonoBehaviour
{
    public float speed = 180f;          //회전 속도

    public Vector3 axis = Vector3.up;   //회전 축

    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime);
    }
}
