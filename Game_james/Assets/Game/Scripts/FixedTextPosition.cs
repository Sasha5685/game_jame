using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FixedTextPosition : MonoBehaviour
{
    public Transform target; // Объект, над которым должен быть текст
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Смещение текста

    private void LateUpdate()
    {
        if (target != null)
        {
            // Фиксируем позицию текста над объектом
            transform.position = target.position + offset;

            // Поворачиваем текст к камере (но только по оси Y)
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y + 180, 0);
        }
    }
}