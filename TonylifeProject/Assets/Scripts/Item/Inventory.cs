using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Weapon[] items;
    [SerializeField] private int takeItemId = -1;

    [SerializeField] private Animator arms;
    [SerializeField] private RuntimeAnimatorController armDefaultController;

    public bool Busy { get { return busy; } }

    private bool busy;

    private void Start()
    {
        TakeItem(takeItemId);
    }

    public void TakeItem(int id)
    {
        if (id < 0)
        {
            arms.runtimeAnimatorController = armDefaultController;
            return;
        }
        busy = true;
        takeItemId = id;
        for (int i = 0; i < items.Length; i++)
        {
            if (i == id)
            {
                items[i].gameObject.SetActive(true);
                arms.runtimeAnimatorController = items[i].Controller;
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }
    }

    public Weapon GetCurretWeapon()
    {
        if(takeItemId < 0) return null;
        return items[takeItemId];
    }

    public void DropItem(Vector3 pos, Vector2 dir, Quaternion startRot, float dropForce, float rotationForce)
    {
        if (takeItemId < 0) return;
        arms.runtimeAnimatorController = armDefaultController;
        busy = false;
        Rigidbody2D rb = Instantiate(items[takeItemId].WeaponPrefab, pos, startRot).GetComponent<Rigidbody2D>();
        rb.AddForce(dir * dropForce, ForceMode2D.Impulse);
        rb.AddTorque(-dir.x * rotationForce, ForceMode2D.Impulse);
        items[takeItemId].gameObject.SetActive(false);
        takeItemId = -1;
    }
}
