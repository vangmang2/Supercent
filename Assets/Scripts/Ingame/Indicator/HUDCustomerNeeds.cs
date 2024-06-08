using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HUDCustomerNeeds : Indicator
{
    [SerializeField] Sprite croassant, pos, table;
    [SerializeField] Image imgNeeds;
    [SerializeField] TMP_Text txtValue;

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public HUDCustomerNeeds SetActiveValue(bool enable)
    {
        txtValue.gameObject.SetActive(enable);
        return this;
    }

    public HUDCustomerNeeds SetNeedsSprite(Needs needs)
    {
        switch (needs)
        {
            case Needs.croassant:
                imgNeeds.sprite = croassant;
                break;
            case Needs.pos:
                imgNeeds.sprite = pos;
                break;
            case Needs.table:
                imgNeeds.sprite = table;
                break;
        }
        return this;
    }

    public HUDCustomerNeeds SetValueText(string text)
    { 
        txtValue.SetText(text);
        return this;
    }

    public override void IndicateTarget(Camera mainCamera, Camera uiCamera, Vector2 canvasSize)
    {
        var indicatedPos = uiCamera.IndicateTarget(mainCamera, target, canvasSize, out var isOutOfBorder);
        rtTransform.anchoredPosition = indicatedPos;
    }
}
