using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();

    private Dictionary<int, Item> itemLookup;

    private void OnEnable()
    {
        itemLookup = new Dictionary<int, Item>();
        foreach (var item in items)
        {
            if (!itemLookup.ContainsKey(item.ID))
            {
                itemLookup.Add(item.ID, item);
            }
        }
    }

    public Item GetItemByID(int id)
    {
        itemLookup.TryGetValue(id, out Item item);
        return item;
    }
}
