using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerStatesManager : MonoBehaviour
{
    public static CustomerStatesManager instance { get; private set; }


    [SerializeField] HUDCustomerStates indicatorPrefab;
    [SerializeField] Transform indicatorParent;
    [SerializeField] Camera mainCamera, uiCamera;
    [SerializeField] RectTransform rtCanvas;

    List<HUDCustomerStates> indicatorList = new List<HUDCustomerStates>();

    private void Awake()
    {
        instance = this;
    }

    public HUDCustomerStates Spawn(Transform target)
    {
        var indicator = Instantiate(indicatorPrefab, indicatorParent).GetComponent<HUDCustomerStates>();
        indicator.SetTarget(target);
        indicatorList.Add(indicator);
        return indicator;
    }

    public void Despawn(HUDCustomerStates indicator)
    {
        if (indicator == null) return;
        indicatorList.Remove(indicator);
        try
        {
            DestroyImmediate(indicator.gameObject);
        }
        catch (MissingReferenceException)
        {

        }
    }

    private void LateUpdate()
    {
        foreach (var indicator in indicatorList.ToList())
        {
            indicator.IndicateTarget(mainCamera, uiCamera, rtCanvas.sizeDelta);
        }
    }
}
