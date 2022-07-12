using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private InteractivitiIndicatorController indicatorController;

    private void Awake()
    {
        instance = this;
    }

    public void IndicatorWakeUp(Transform target)
    {
        indicatorController.target = target;
        indicatorController.gameObject.SetActive(true);
    }
    
    public void IndicatorSleep()
    {
        indicatorController.target = null;
        indicatorController.gameObject.SetActive(false);
    }
}
