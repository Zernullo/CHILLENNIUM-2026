using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceCard : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button selectButton;

    public void Setup(ItemData data, System.Action onSelected) {
        titleText.text = data.itemName;
        descriptionText.text = data.itemDescription;
        iconImage.sprite = data.itemIcon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected());

    }
}
