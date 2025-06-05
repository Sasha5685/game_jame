using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private float mass = 1f;
    [SerializeField] private bool canBePushed = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = mass;
    }

    public bool CanBePushed()
    {
        return canBePushed && rb != null && !rb.isKinematic;
    }

    // Можно добавить дополнительные методы для управления толканием
    public void SetPushable(bool state)
    {
        canBePushed = state;
    }
}