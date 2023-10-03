using System.Collections.Generic;
using UnityEngine;


public class HandControls : MonoBehaviour, IService
{
    [SerializeField] Sprite _handSprite;
    [SerializeField] Sprite _tentacleSprite;
    [SerializeField] Sprite _tripleTentacleSprite;

    private int _handWidth = 1;
    private List<Figure> _figures = new List<Figure>();
    private int _trayLehgth = 10;
    private int _handPower = 0;

   public void AddFigures(List<Figure> figures)
    {
        _figures = figures;
    }

    public float CheckBalance()
    {
        int leftBalanceX = Mathf.RoundToInt(transform.localPosition.x) - _handWidth;
        int rightBalanceX = Mathf.RoundToInt(transform.localPosition.x) + _handWidth;

        float leftSideWeight = 0, rightSideWeight = 0;

        if (_figures.Count > 0)
        {

            foreach (Figure f in _figures)
            {
                Vector3 pos = f.GetWorldPosition();
                Vector2Int[] form = f.GetForm();

                foreach (Vector2Int piece in form)
                {
                    float pieceWorldX = piece.x + pos.x;

                    if (pieceWorldX > rightBalanceX)
                    {
                        rightSideWeight += (pieceWorldX - rightBalanceX); //TO DO: use density instead of mass
                    }
                    else if (pieceWorldX < leftBalanceX)
                        leftSideWeight += (leftBalanceX - pieceWorldX); //TO DO: use density instead of mass
                }
            }

            
        }

        leftSideWeight += (leftBalanceX + _trayLehgth * 0.5f) * (leftBalanceX + _trayLehgth * 0.5f) * 0.1f;
        rightSideWeight += (_trayLehgth * 0.5f - rightBalanceX) * (_trayLehgth * 0.5f - rightBalanceX) * 0.1f;  

        if (Mathf.Abs(leftSideWeight - rightSideWeight) > _handPower)
            return leftSideWeight - rightSideWeight;

        return 0f;
    }
    
    public void SetTentacle() //TO DO: use Unity Event
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _tentacleSprite;
        _handWidth *= 2; 
        _handPower += 3;
    }
    public void SetTripleTentacle() //TO DO: use Unity Event
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _tripleTentacleSprite;
        _handWidth *= 2;
        _handPower += 2;
    }

    public void ResetHand()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _handSprite;
        _handWidth = 1; 
        _trayLehgth = 10;
        _handPower = 0;
}

    public int GetHandWidth()
    {
        return _handWidth;
    }
}
