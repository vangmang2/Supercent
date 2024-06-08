using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerNeedsManager : MonoBehaviour
{
    public static CustomerNeedsManager instance { get; private set; }


    [SerializeField] HUDCustomerNeeds indicatorPrefab;
    [SerializeField] Transform indicatorParent;
    [SerializeField] Camera mainCamera, uiCamera;
    [SerializeField] RectTransform rtCanvas;

    List<HUDCustomerNeeds> indicatorList = new List<HUDCustomerNeeds>();

    private void Awake()
    {
        instance = this;
    }

    public HUDCustomerNeeds Spawn(Transform target)
    {
        var indicator = Instantiate(indicatorPrefab, indicatorParent).GetComponent<HUDCustomerNeeds>();
        indicator.SetTarget(target);
        indicatorList.Add(indicator);
        return indicator;
    }

    public void Despawn(HUDCustomerNeeds indicator)
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
