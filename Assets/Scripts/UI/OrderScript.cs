using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderScript : MonoBehaviour
{
    [SerializeField] Sprite[] _figuresIcons;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }
    public void ChangeIcon(int index)
    {
        _image.overrideSprite = _figuresIcons[index];
        _image.SetNativeSize();
    }
}
