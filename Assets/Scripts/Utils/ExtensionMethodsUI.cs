using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethodsUI
{
    public delegate float EasingDelegate(float start, float end, float time);
    
    public static IEnumerator RescaleGraphicsRoutine(this RectTransform rt, Vector3 targetScale, float speed, EasingDelegate easing)
    {
        float timer = 0;
        Vector3 startScale = rt.localScale;
        while (timer < 1)
        {
            Vector3 scale = Vector3.Lerp(startScale, targetScale, easing(0,1,timer));
            rt.localScale = scale;
            timer += Time.deltaTime / speed;
            yield return new WaitForEndOfFrame();
        }
        
        rt.localScale = targetScale;
    }
    
    public static Vector3 GetPositionInCanvas(this Vector3 position, bool heightBased = false)
    {
        Vector3 pos = position;
        pos.x = pos.x / (heightBased ? Screen.height : Screen.width) * (heightBased ? 1920 : 1080);
        pos.y = pos.y / Screen.height * 1920;
        return pos;
    }
    
    public static void SetAlpha(this Image image, float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
    
    public static void SetAlpha(this TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
