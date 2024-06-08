using UnityEngine;

public abstract class Indicator : MonoBehaviour
{
    [SerializeField] protected RectTransform rtTransform;
    protected Transform target;

    public Indicator SetTarget(Transform target)
    {
        this.target = target;
        return this;
    }

    public virtual void IndicateTarget(Camera mainCamera, Camera uiCamera, Vector2 canvasSize) { }
}
