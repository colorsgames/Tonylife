using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectDetector : Detector
{
    public List<InteractiveObject> InteractiveObjects { get { return canUseInteractiveObjects; } }

    [SerializeField]private List<InteractiveObject> canUseInteractiveObjects = new List<InteractiveObject>();
    private List<InteractiveObject> allInteractiveObjects = new List<InteractiveObject>();

    private InteractiveObject oldInteract;

    private void Update()
    {
        if (allInteractiveObjects.Count > 0)
        {
            foreach (InteractiveObject item in allInteractiveObjects)
            {
                if (!item) return;
                RaycastHit2D hit = GetRaycastHit2D(item.transform.position);
                if (!hit) return;
                if (hit.collider.GetComponent<InteractiveObject>() == item)
                {
                    if (!canUseInteractiveObjects.Contains(item))
                    {
                        canUseInteractiveObjects.Add(item);
                    }
                }
                else
                {
                    canUseInteractiveObjects.Remove(item);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {
            allInteractiveObjects.Add(collision.gameObject.GetComponent<InteractiveObject>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {
            canUseInteractiveObjects.Remove(collision.gameObject.GetComponent<InteractiveObject>());
            allInteractiveObjects.Remove(collision.gameObject.GetComponent<InteractiveObject>());
        }
    }
}
