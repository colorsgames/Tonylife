using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "New Item", order = 51)]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        guns,
        item
    }

    [SerializeField] private int id;

    public int Id { get { return id; } }

    public ItemType type = ItemType.item;
}
