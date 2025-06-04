using UnityEngine;
using UnityEngine.UI;

public class InteractableItem : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public Item itemData;

    [Header("Visuals")]
    public Color outlineColor = Color.white;
    public float outlineWidth = 0.02f;
    public float glowIntensity = 1.5f;

    [Header("UI")]
    public GameObject takeTextPrefab;

    private GameObject takeTextInstance;
    private Camera mainCamera;
    private bool isHighlighted = false;
    private Material[] originalMaterials;
    private Material outlineMaterial;

    private void Start()
    {
        mainCamera = Camera.main;

        // Создаем текст "Take" как дочерний объект
        if (takeTextPrefab != null)
        {
            takeTextInstance = Instantiate(takeTextPrefab, transform);
            takeTextInstance.transform.localPosition = Vector3.up * 1.5f;
            takeTextInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (mainCamera == null) return;

        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        bool isLooking = IsPlayerLookingAtObject();

        if (distance <= interactionDistance && isLooking)
        {

            // Поворачиваем текст к камере
            if (takeTextInstance != null)
            {
                takeTextInstance.SetActive(true);
                takeTextInstance.transform.LookAt(mainCamera.transform);
                takeTextInstance.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TakeItem();
            }
        }
        else
        {


            if (takeTextInstance != null)
            {
                takeTextInstance.SetActive(false);
            }
        }
    }

    private bool IsPlayerLookingAtObject()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }



    private void TakeItem()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null && itemData != null)
        {
            if (playerInventory.AddItem(itemData))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (takeTextInstance != null)
        {
            Destroy(takeTextInstance);
        }
    }
}