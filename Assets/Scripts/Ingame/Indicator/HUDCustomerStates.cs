using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HUDCustomerStates : Indicator
{
    [SerializeField] Sprite croassant, pos, table;
    [SerializeField] Image imgNeeds;
    [SerializeField] TMP_Text txtValue;
    [SerializeField] float revision;

    [SerializeField] GameObject goNeeds, goHappyFace;

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public HUDCustomerStates SetActiveNeeds(bool enable)
    {
        goNeeds.SetActive(enable);
        return this;
    }

    public HUDCustomerStates SetActiveHappyFace(bool enable)
    {
        goHappyFace.SetActive(enable);
        return this;
    }

    public HUDCustomerStates SetActiveValue(bool enable)
    {
        txtValue.gameObject.SetActive(enable);
        return this;
    }

    public HUDCustomerStates SetNeedsSprite(Needs needs)
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

    public HUDCustomerStates SetValueText(string text)
    { 
        txtValue.SetText(text);
        return this;
    }

    public override void IndicateTarget(Camera mainCamera, Camera uiCamera, Vector2 canvasSize)
    {
        var indicatedPos = uiCamera.IndicateTarget(mainCamera, target, canvasSize, out var isOutOfBorder);
        indicatedPos.y += revision;
        rtTransform.anchoredPosition = indicatedPos;
    }
}
