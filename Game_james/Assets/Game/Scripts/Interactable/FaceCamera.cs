using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform parentObject;
    public Vector3 localOffset;

    void Update()
    {
        if (parentObject != null)
        {
            // Позиция текста следует за объектом, но не вращается
            transform.position = parentObject.position + parentObject.TransformDirection(localOffset);

            // Текст всегда смотрит на камеру (как в Billboard)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}