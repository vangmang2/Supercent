using UnityEngine;

public static class ModuleTargetIndicator
{
    static readonly Vector2 indicatorBorder = new Vector2(0.45f, 0.45f);
    static Vector3 outOfBorderPos => new Vector3(1280f * indicatorBorder.x, 720f * indicatorBorder.y);

    /// <summary> 3d 좌표의 타겟을 2d UI 좌표계로 변환 해준다. 인디케이터 경계는 원형 경계로 처리한다. </summary>
    public static Vector2 IndicateTarget(this Camera uiCamera, Camera mainCamera, Transform target, Vector2 canvasSize, out bool isOutOfBorder)
    {
        var targetPos = target.transform.position;
        var targetMainCameraPos = mainCamera.WorldToScreenPoint(targetPos);
        var targetUICameraPos = uiCamera.ScreenToViewportPoint(targetMainCameraPos) - new Vector3(0.5f, 0.5f);

        bool isXposOutOfBorder = Mathf.Abs(targetUICameraPos.x) > indicatorBorder.x;
        bool isYposOutOfBorder = Mathf.Abs(targetUICameraPos.y) > indicatorBorder.y;
        bool isTargetBehindCamera = targetMainCameraPos.z < 0f;
        isOutOfBorder = isXposOutOfBorder || isYposOutOfBorder || isTargetBehindCamera;

        if (isOutOfBorder)
        {
            var dir = targetUICameraPos;
            if (isTargetBehindCamera)
                dir *= -1f;
            var deg = Mathf.Atan2(dir.y, dir.x);

            targetUICameraPos = new Vector2(Mathf.Cos(deg) * outOfBorderPos.x, Mathf.Sin(deg) * outOfBorderPos.y);
        }
        else
        {
            targetUICameraPos.x *= canvasSize.x;
            targetUICameraPos.y *= canvasSize.y;
        }
        return targetUICameraPos;
    }
}