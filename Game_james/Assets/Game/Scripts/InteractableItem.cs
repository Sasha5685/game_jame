using TMPro;
using UnityEngine;


public class InteractableItem : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;      // ���� �������
    public float defaultOutlineWidth = 3f;        // ������ ������� ��� ���������
    public float hoverOutlineWidth = 8f;          // ������ ������� ��� ���������
    public float pulseSpeed = 2.5f;               // �������� ���������
    [Range(0f, 1f)] public float pulseIntensity = 0.85f; // ��������� ������ ���������� (0 = ��� ���������, 1 = �������)

    public Item ItemData;
    public int ItemIntAdd;

    private bool isHighlighted;
    private Outline outline;
    private float currentPulseValue;

    private void Start()
    {
        InitializeOutline();
    }

    private void InitializeOutline()
    {
        outline = gameObject.GetComponent<Outline>();
    }



    private void Update()
    {
        if (isHighlighted)
        {


            // ������� ��������� ������� (���������� ��������� ��� ���������)
            currentPulseValue = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            float pulseWidth = hoverOutlineWidth * (1f + currentPulseValue);
            outline.OutlineWidth = pulseWidth;
        }
    }

    public void SetHighlight(bool state)
    {
        isHighlighted = state;


        if (state)
        {
            // ��� ��������� �������� ��������� (��� �������� � Update)
            outline.OutlineWidth = hoverOutlineWidth;
        }
        else
        {
            // ��� ��������� � ������ ��������� �������
            outline.OutlineWidth = defaultOutlineWidth;
        }
    }

    public void Interact()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null && ItemData != null && playerInventory.FoundFreeSlot() > -1)
        {
            playerInventory.AddItemToSlot(playerInventory.FoundFreeSlot(), ItemData, ItemIntAdd);

            Destroy(gameObject);
        }
    }


}