using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    [SerializeField] Material _doubleCostMaterial;
    
    private Transform _transform;
    private Vector2Int _centerPos; // TODO: pivot to top left corner
    private Vector2Int[] _form;
    private int _width;
    private int _height;
    private int _cost;
    private bool _doubleCost = false;

    public void Init(FigureSO figure)
    {
        // задаем коллайдер из данных фигуры
        _transform = GetComponent<Transform>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.sprite;
        _form = figure.form;
        _width = figure.width;
        _height = figure.height;
        _cost = figure.cost;
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
        GetComponent<Rigidbody2D>().AddForce(new Vector2(dir * 20, 2));

        //ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());
    }

    public int GetProfit() // TO DO: Use Unity Event
    {
        return (_doubleCost) ? 2 * _cost : _cost;
    }

    public int GetFine() // TO DO: Use Unity Event
    {
        return -_cost;
    }

    public void SetDoubleCost()
    {
        _doubleCost = true;
        GetComponent<SpriteRenderer>().material = _doubleCostMaterial;
    }
}
