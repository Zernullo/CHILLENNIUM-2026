using UnityEngine;

public enum BuffType
{
    None,
    SlowNotes,
    DamageBoost,
    Heal,
    Cleanse,
    MissImmunity
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public BuffType buffType; 
}