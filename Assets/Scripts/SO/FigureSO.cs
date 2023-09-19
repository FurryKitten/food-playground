using UnityEngine;

[CreateAssetMenu(fileName = "Figure", menuName = "ScriptableObjects/Figures", order = 1)]
public class FigureSO : ScriptableObject
{
    [SerializeField] float _widthTex;
    [SerializeField] float _heightTex;
    [SerializeField] Vector2Int[] _form;
    [SerializeField] float _mass;
    [SerializeField] int _cost;
    [SerializeField] int _indexTex;
    [SerializeField] Sprite _sprite;
    [SerializeField] Sprite _spriteO;

    
    public float widthTex
    { get { return _widthTex; } }

    public float heightTex
    { get { return _heightTex; } }

    public Vector2Int[] form
    { get { return _form; } }

    public float mass
    { get { return _mass; } }

    public int cost 
    { get { return _cost; } }

    public int indexTex
    { get { return _indexTex; } }

    public Sprite sprite
    { get { return _sprite; } }

    public Sprite spriteO
    { get { return _spriteO; } }
}
