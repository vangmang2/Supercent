using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinShower : MonoBehaviour
{
    [SerializeField] RectTransform rtCoin;
    [SerializeField] TMP_Text txtCoin;

    public CoinShower SetTextCoin(string text)
    {
        txtCoin.SetText(text);
        return this;
    }

    public CoinShower PlayCoinAnim()
    {
        rtCoin.DOKill();
        rtCoin.localScale = Vector3.one * 1.25f;
        rtCoin.DOScale(1f, 0.5f).SetEase(Ease.OutQuad);
        return this;
    }
}
