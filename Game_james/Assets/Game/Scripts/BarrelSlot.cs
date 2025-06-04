using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BarrelSlot : MonoBehaviour, IPointerDownHandler
{
    public enum SlotType { Hay, Beer }
    public SlotType slotType;
    public int beerSlotIndex;

    public BarrelController barrelController;
    public Image icon;
    public Text countText;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            switch (slotType)
            {
                case SlotType.Hay:
                    //barrelController.OnHaySlotClick();
                    break;
                case SlotType.Beer:
                    //barrelController.OnBeerSlotClick(beerSlotIndex);
                    break;
            }
        }
    }

    public void UpdateSlot(Sprite sprite, int count)
    {
        icon.sprite = sprite;
        icon.enabled = sprite != null;
        countText.text = count > 1 ? count.ToString() : "";
    }
}