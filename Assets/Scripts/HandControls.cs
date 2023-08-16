using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BalaceState
{
    OK,
    Left,
    Right,
}
public class HandControls : MonoBehaviour
{

   private List<Figure> _figures = new List<Figure>();
   private int _handWidth = 2;

   public void AddFigures(List<Figure> figures)
    {
        _figures = figures;
    }

    public float CheckBalance()
    {

        if (_figures.Count > 0)
        {
            int leftBalanceX = Mathf.RoundToInt(transform.localPosition.x) - _handWidth;
            int rightBalanceX = Mathf.RoundToInt(transform.localPosition.x) + _handWidth;

            float leftSideWeight = 0, rightSideWeight = 0;

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

            if (Mathf.Abs(leftSideWeight - rightSideWeight) > 5f)
                return leftSideWeight - rightSideWeight;
        }
        
        return 0f;
    }
}
