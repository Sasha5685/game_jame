using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionText = "Press [E] to open";
    public KeyCode interactionKey = KeyCode.E;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);

    [Header("Script References")]
    public int interactionIndex = 0;
    public int BarrelInt;
    public BarrelController BarrelController;

    private GameObject interactionTextInstance;
    private TextMeshPro textMesh;
    private bool isHighlighted;

    private void Start()
    {
        CreateInteractionText();
    }

    private void CreateInteractionText()
    {
        interactionTextInstance = new GameObject("InteractionText");
        interactionTextInstance.transform.SetParent(transform);
        interactionTextInstance.transform.localPosition = textOffset;

        textMesh = interactionTextInstance.AddComponent<TextMeshPro>();
        textMesh.text = interactionText.Replace("[E]", $"[{interactionKey}]");
        textMesh.fontSize = 2;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.enabled = false;
    }

    private void Update()
    {
        if (isHighlighted && interactionTextInstance != null)
        {
            // ѕосто€нное обновление позиции и поворота
            interactionTextInstance.transform.position = transform.position + textOffset;
            interactionTextInstance.transform.LookAt(Camera.main.transform);
            interactionTextInstance.transform.Rotate(0, 180, 0);
        }
    }

    public void SetHighlight(bool state)
    {
        isHighlighted = state;
        if (textMesh != null)
        {
            textMesh.enabled = state;
        }
    }

    public void Interact()
    {
        if (interactionIndex == 0 && BarrelController != null)
        {
            BarrelController.OpenUIBarrel(BarrelInt);
        }
    }
}