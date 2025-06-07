using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public int maxStack = 1;
    public GameObject prefab; // ��� ����������� �������������
    public float throwForce = 10f; // ��������� ���� ������
    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
    }
}