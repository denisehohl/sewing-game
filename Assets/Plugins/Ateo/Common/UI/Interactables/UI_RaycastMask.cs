/***************************************************************************\
The MIT License (MIT)
Copyright (c) 2014 Jonas Schiegl (https://github.com/senritsu)
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
\***************************************************************************/

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UI_RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
    private Image _image;
    private Sprite _sprite;

    void OnEnable ()
    {
        _image = GetComponent<Image>();
        _image.raycastTarget = true;
        _sprite = _image.sprite;
    }
    
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {   
        var rectTransform = (RectTransform)transform;
        Vector2 localPositionPivotRelative;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, sp, eventCamera, out localPositionPivotRelative);

        // convert to bottom-left origin coordinates
        var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x*rectTransform.rect.width,
            localPositionPivotRelative.y + rectTransform.pivot.y*rectTransform.rect.height);
        
        var maskRect = rectTransform.rect;
        var pixelCoord = new Vector2((localPosition.x/maskRect.width)*_sprite.texture.width, (localPosition.y/maskRect.height)*_sprite.texture.height);

        // destroy component if texture import settings are wrong
        try
        {
            return _sprite.texture.GetPixel((int)pixelCoord.x,(int)pixelCoord.y).a > 0;
        }
        catch
        {
            DebugDev.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
            Destroy(this);
            return false;
        }
    }
}
