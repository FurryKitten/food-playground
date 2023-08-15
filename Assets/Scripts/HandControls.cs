using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControls : MonoBehaviour
{
    
   private List<Figure> _figures;

   public void AddFigures(List<Figure> figures)
    {
        _figures = figures;
    }

    public bool CheckBalance()
    {
        int balanceX = Mathf.RoundToInt(transform.position.x);

        float leftSideWeight = 0, rightSideWeight = 0;

        foreach(Figure f in _figures)
        {
            Vector3 pos = f.GetWorldPosition();
            Vector2Int[] form = f.GetForm();

            foreach(Vector2Int piece in form)
            {
                float pieceWorldX = piece.x + pos.x;
                if (pieceWorldX != balanceX)
                { 
                    if (pieceWorldX > balanceX)
                    {
                        rightSideWeight += (pieceWorldX - balanceX); //TO DO: use density instead of mass
                    }
                    else
                        leftSideWeight += (balanceX - pieceWorldX); //TO DO: use density instead of mass
                }
            }
        }

        if (Mathf.Abs(leftSideWeight - rightSideWeight) > 5f)
            return false; 
        
        return true;
    }
}
