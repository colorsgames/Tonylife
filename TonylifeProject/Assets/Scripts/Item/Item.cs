using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : InteractiveObject
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Transform centerOfMass;

    public ItemData ItemInfo { get { return itemData; } }

    private void Start()
    {
        if(centerOfMass)
            GetComponent<Rigidbody2D>().centerOfMass = centerOfMass.localPosition;
    }

    public override void Use()
    {
        Destroy(gameObject);
    }
}
