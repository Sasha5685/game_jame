using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarrelController : MonoBehaviour
{
    [Header("Defould")]
    public GameObject UIBarrel;
    private int Barrel;
    public PlayerController PlayerController;

    //[Header("Setings")]
    //public Slot Slot; 
    public void OpenUIBarrel(int barrel)
    {
        Barrel = barrel;
        UIBarrel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerController.UIOpen = true;
    }
    public void ClouseUIBarrel()
    {
        UIBarrel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.UIOpen = false;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){ ClouseUIBarrel(); }
    }
}