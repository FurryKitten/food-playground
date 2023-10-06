using System.Collections;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    [SerializeField] Material _figureMaterial;
    [SerializeField] GameObject _spoiledParticleSystem;
    [SerializeField] GameObject _goldParticleSystem;
    [SerializeField] GameObject _mainTexture;

    private Transform _transform;
    private Vector2Int _centerPos; // TODO: pivot to top left corner
    private Vector2Int[] _form;
    private int _cost;
    private bool _doubleCost = false;
    private int _index = 0;
    private Material _material;
    private bool _spoiled = false;
    private float _height = 0.2f; // 0.2f - размер одной клетки, такой формат необходим для шейдера
    private float _width = 0.2f;
    private bool _shaderAnimation = false;
    private float _step = 0.7f;
    private float _targetStep = 0f;
    private bool _shaderStateGold2Spoiled = false;
    private bool _figureDestroyed = false;
    private bool _timedDoubleCost = false;

    public void Init(FigureSO figure)
    {
        _transform = GetComponent<Transform>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.spriteO;
        _mainTexture.GetComponent<SpriteRenderer>().sprite = figure.sprite;
        _material = new Material(_figureMaterial);
        _material.SetVector("_Tilling", new Vector2(figure.widthTex, figure.heightTex));
        _index = figure.indexTex;
        if(_index == 18)
            _mainTexture.layer = 0;
        _material.SetFloat("_Index", figure.indexTex);
        _mainTexture.GetComponent<SpriteRenderer>().material = _material;
        _form = figure.form;
        _cost = figure.cost;
        _height = figure.heightTex;
        _width = figure.widthTex;
    }
    public void SetPosition(int x, int y)
    {
        _centerPos = new Vector2Int(x, y);
    }

    public void SetWorldPosition(Vector2 pos)
    {
        _transform.localPosition = pos;
    }

    public void Fall()
    {
        _centerPos.y -= 1;
        Vector3 pos = _transform.localPosition;
        pos.y -= 1;
        _transform.localPosition = pos;
    }

    public Vector2Int GetPosition()
    {
        return _centerPos;
    }

    public Vector3 GetWorldPosition()
    {
        return _transform.localPosition;
    }
    public Vector2Int[] GetForm()
    {
        return _form;
    }

    public void HorizontalMove(int dir)
    {
        _centerPos.x += dir;
        Vector3 pos = _transform.localPosition;
        pos.x += dir;
        _transform.localPosition = pos;
    }

    public void FlyAway(int dir)
    {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(dir * 200, 2));

        ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());

        if (_index != 18)
            ServiceLocator.Current.Get<GameState>().ChangeHealth(-1);
    }

    public void FlyAway()
    {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 200);
    }

    public int GetProfit() 
    {
        return (_doubleCost) ? 2 * _cost : _cost;
    }

    public int GetFine() 
    {
        return (_doubleCost) ? -2 * _cost : -_cost;
    }

    public void SetDoubleCost()
    {
        _doubleCost = true;
        _material.SetInt("_Gold2Normal", 1);
        _targetStep = 0;
        if (!_shaderAnimation)
            StartCoroutine(ShaderAnimation());
    }

    public void ResetDoubleCost()
    {
        if (!_spoiled)
        {
            _doubleCost = false;
            if (_shaderStateGold2Spoiled)
            {
                _shaderStateGold2Spoiled = false;
                _material.SetInt("_Gold2Spoiled", 0);
            }
            ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());
            _targetStep = 0.7f;
            if (!_shaderAnimation)
                StartCoroutine(ShaderAnimation());
        }
        else
        {
            _doubleCost = false;
            if (_shaderStateGold2Spoiled)
            {
                _shaderStateGold2Spoiled = false;
                _material.SetInt("_Gold2Spoiled", 0);
            }
        }
    }

    public void ChangeGoldenStatus(bool status)
    {
        if(!_doubleCost && status)
        {
            _timedDoubleCost = true;
            ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetProfit());
            SetDoubleCost();
            return;
        }

        if(_doubleCost && _timedDoubleCost && !status)
        {
            ResetDoubleCost();
        }

    }

    public void ChangeSpoiledStatus(bool status)
    {
        if (_spoiled != status)
        {
            _spoiled = status;

            if (_doubleCost && !_shaderStateGold2Spoiled)
            {
                _shaderStateGold2Spoiled = true;
                _material.SetInt("_Gold2Spoiled", 1);
                _step = 0.7f;
                _material.SetFloat("_StepTimer", _step);
            }

            if(!_doubleCost && _timedDoubleCost)
            {
                if (_shaderStateGold2Spoiled)
                {
                    _shaderStateGold2Spoiled = false;
                    _material.SetInt("_Gold2Spoiled", 0);
                }
                _material.SetInt("_Gold2Normal", 0);

                if (status)
                    _step = 0.7f;
                else
                    _step = 0f;
                _material.SetFloat("_StepTimer", _step);
            }

            if (_spoiled)
            {
                //анимация порченья
                _targetStep = 0;
                ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());
                if (!_shaderAnimation)
                    StartCoroutine(ShaderAnimation());
            }
            else
            {
                //анимация восстановления
                _targetStep = 0.7f;
                ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetProfit());
                if (!_shaderAnimation)
                    StartCoroutine(ShaderAnimation());
            }
        }
    }

    private IEnumerator ShaderAnimation()
    {
        float t = 0;
        float animationSpeed = (_index != 18) ? 0.8f : 1.6f;
        _shaderAnimation = true;
        while (t < 1)
        {
            _step = Mathf.Lerp(_step, _targetStep, t*t*t);
            t += Time.deltaTime * animationSpeed;
            _material.SetFloat("_StepTimer", _step);
            yield return null; 
        }
        _shaderAnimation = false;
        if (_figureDestroyed)
            Destroy(this.gameObject);
    }


    public void DestroySpoiler()
    {
        // TO DO: Start animation of destroy spoiler
        transform.parent = null;
        ChangeSpoiledStatus(true);
        Instantiate(_spoiledParticleSystem, _transform.position, Quaternion.identity);
    }

    public void CreateTriplet()
    {
        ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetProfit());

        SetDoubleCost();
        _timedDoubleCost = false;
        Vector3 position = _transform.position;
        position.x += _width * 2.5f;
        Instantiate(_goldParticleSystem, position, Quaternion.identity);
    }

    public void DoGoldSpoiler()
    {
        SetDoubleCost();
    }
    public void CombineIntoTriplet()
    {
        _material.SetInt("_Normal2Disappear", 1);
        _targetStep = 0;
        _figureDestroyed = true;
        ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());
        if (!_shaderAnimation)
            StartCoroutine(ShaderAnimation());
    }

    public void SetMaterial()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public int Index
    { get { return _index; } }

    public bool IsSpoiled
    { get { return _spoiled; } }

    public float Height
    { get { return _height; } }

    public float Width
    { get { return _width; } }

    public bool IsGold
    { get { return _doubleCost; } }

    public bool IsTimedGold
    { get { return _doubleCost & _timedDoubleCost; } }
}
