using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderScript : MonoBehaviour
{
    [SerializeField] Sprite[] _figuresIcons;
    [SerializeField] Image _SandTimer;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }
    public void ChangeIcon(int index)
    {
        if (index < 25)
        {
            _SandTimer.enabled = false;
            _image.enabled = true;
            _image.overrideSprite = _figuresIcons[index];
            _image.SetNativeSize();
        }
        else
        {
            _image.enabled = false;
            _SandTimer.enabled = true;
        }
    }
}
