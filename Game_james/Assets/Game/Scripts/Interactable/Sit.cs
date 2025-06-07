using TMPro;
using UnityEngine;

public class Sit : MonoBehaviour, IInteractable
{
    public Transform PosSit;
    public Transform LookTarget; // Добавляем цель для взгляда

    [Header("Interaction Settings")]
    public string interactionText = "Press [E] to sit";
    public KeyCode interactionKey = KeyCode.E;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);

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
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (!playerController.isSit)
        {
            playerController.Sit(PosSit, LookTarget); // Передаем цель для взгляда
        }
    }
}