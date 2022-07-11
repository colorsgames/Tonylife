using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivitiIndicatorController : MonoBehaviour
{
    [HideInInspector] public Transform target;

    [SerializeField] private Vector3 offset;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (target)
        {
            Vector3 screenSpacePos = cam.WorldToScreenPoint(target.position + offset);
            transform.position = screenSpacePos;
        }
    }
}
