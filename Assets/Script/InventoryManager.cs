using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {
    public List<ItemData> itemPool; 
    public Transform[] slots;    
    public GameObject itemPrefab;  
    public Button spawnButton;     

    public void TrySpawnItem() {
        foreach (Transform slot in slots) {
            
            if (slot.childCount == 0) {
                SpawnItem(slot);
                CheckIfFull();
                return; 
            }
        }
    }

    void SpawnItem(Transform parentSlot) {
    ItemData randomData = itemPool[Random.Range(0, itemPool.Count)];
    

    GameObject newItem = Instantiate(itemPrefab, parentSlot);
    
    RectTransform rt = newItem.GetComponent<RectTransform>();
    rt.anchoredPosition = Vector2.zero; 
    rt.localScale = Vector3.one;       
 
    newItem.GetComponent<Image>().sprite = randomData.itemIcon;
    
    }
    void CheckIfFull() {
        bool isFull = true;
        foreach (Transform slot in slots) {
            if (slot.childCount == 0) isFull = false;
        }
     
        if (isFull) spawnButton.interactable = false;
    }
}

