using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private float stronglyDropItemForce = 20f;
    [SerializeField] private float rotationForce = 5f;

    private AliveCreature aliveCreature;
    private Inventory inventory;

    private void Start()
    {
        aliveCreature = GetComponentInParent<AliveCreature>();
        inventory = GetComponentInParent<Inventory>();
    }

    public void Drop()
    {
        inventory.DropItem(aliveCreature.DropTarget.position, aliveCreature.MyDirection(), aliveCreature.DropTarget.rotation, stronglyDropItemForce, rotationForce);
    }
}
