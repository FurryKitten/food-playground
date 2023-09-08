using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    [SerializeField] Material _figureMaterial;
    
    private Transform _transform;
    private Vector2Int _centerPos; // TODO: pivot to top left corner
    private Vector2Int[] _form;
    private int _cost;
    private bool _doubleCost = false;
    private int _index = 0;
    private Material _material;
    private bool _spoiled = false;
    private float _height = 0.2f; // 0.2f - размер одной клетки, такой формат необходим для шейдера

    public void Init(FigureSO figure)
    {
        _transform = GetComponent<Transform>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.sprite;
        _material = new Material(_figureMaterial);
        _material.SetVector("_Tilling", new Vector2(figure.widthTex, figure.heightTex));
        _index = figure.indexTex;
        _material.SetFloat("_Index", figure.indexTex);
        gameObject.GetComponent<SpriteRenderer>().material = _material;
        _form = figure.form;
        _cost = figure.cost;
        _height = figure.heightTex;
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
    }

    public void FlyAway()
    {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 200);
    }

    public int GetProfit() 
    {
        return (_spoiled) ? 0 :((_doubleCost) ? 2 * _cost : _cost);
    }

    public int GetFine() 
    {
        return (_doubleCost) ? -2 * _cost : -_cost;
    }

    public void SetDoubleCost()
    {
        _doubleCost = true;
        GetComponent<SpriteRenderer>().material.SetFloat("_Index", _index+25);
    }

    public void ChangeSpoiledStatus(bool status)
    {
        if (_spoiled != status)
        {
            _spoiled = status;

            if (_spoiled)
            {
                //анимация порченья
                gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_StepTimer", 0.1f);
            }
            else
            {
                //анимация восстановления
                gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_StepTimer", 1f);
            }
        }
    }

    public void DestroySpoiler()
    {
        // TO DO: Start animation
    }

    public int Index
    { get { return _index; } }

    public bool IsSpoiled
    { get { return _spoiled; } }

    public float Height
    { get { return _height; } }
}
