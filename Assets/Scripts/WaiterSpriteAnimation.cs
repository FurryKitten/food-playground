using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class WaiterSpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite _defaultFace;
    [SerializeField] private Sprite _worriedFace;

    private Image _image;
    private bool _isWorried = false;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void ChangeFace()
    {
        if (!_isWorried)
        {
            _isWorried = true;
            StartCoroutine(ChangeFaceCoroutine());
        }
    }

    private IEnumerator ChangeFaceCoroutine()
    {
        _image.sprite = _worriedFace;

        yield return new WaitForSeconds(1f);

        _isWorried = false;
        _image.sprite = _defaultFace;
    }
}
