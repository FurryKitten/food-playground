using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BalaceState
{
    OK,
    Left,
    Right,
}
public class HandControls : MonoBehaviour, IService
{
    [SerializeField] Sprite _tentacleSprite;

    private List<Figure> _figures = new List<Figure>();
    private int _handWidth = 2;
    private int _trayLehgth = 10;

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

        if (Mathf.Abs(leftSideWeight - rightSideWeight) > 5f)
            return leftSideWeight - rightSideWeight;

        return 0f;
    }

    public void SetTentacle() //TO DO: use Unity Event
    {
        GetComponentInChildren<SpriteRenderer>().sprite = _tentacleSprite;
        _handWidth *= 2;
    }

    public int GetHandWidth()
    {
        return _handWidth;
    }



    #region TEST VARIANT

    [SerializeField] private float _handLength = 0.4f;

    private float _centerMass;

    public float FindCenterMass()
    {
        if (_figures.Count == 0)
        {
            return 0f;
        }

        float centerMass = 0f;
        int partBlocksCount = 0;

        foreach (Figure f in _figures)
        {
            Vector3 pos = f.GetWorldPosition();
            Vector2Int[] form = f.GetForm();
            foreach (Vector2Int piece in form)
            {
                float pieceWorldX = piece.x + pos.x + 0.5f;
                centerMass += pieceWorldX;

                partBlocksCount++;
            }
        }

        _centerMass = centerMass;

        return centerMass / partBlocksCount;
    }

    public float DistanceCenterMassToHand()
    {
        float centerMass = FindCenterMass();
        float handPos = transform.localPosition.x;
        float handPosLeft = transform.localPosition.x - _handLength;
        float handPosRight = transform.localPosition.x + _handLength;

        if (centerMass > handPosLeft && centerMass < handPosRight)
        {
            return 0f;
        }

        return centerMass <= handPosLeft
            ? centerMass - handPosLeft
            : centerMass >= handPosRight
            ? centerMass - handPosRight
            : 0f;
    }

    private void OnDrawGizmos()
    {
        // CENTER MASS
        Gizmos.color = Color.yellow;
        float center = FindCenterMass();
        Vector3 centerMassPos = transform.parent.position;
        centerMassPos.x += center; //_centerMass;
        Gizmos.DrawWireSphere(centerMassPos, 0.5f);

        // HAND BORDERS
        Gizmos.color = Color.magenta;
        Vector3 left = transform.position;
        Vector3 right = transform.position;
        left.x -= _handLength;
        right.x += _handLength;
        Gizmos.DrawLine(left - new Vector3(0, 1f, 0), left + new Vector3(0, 1f, 0));
        Gizmos.DrawLine(right - new Vector3(0, 1f, 0), right + new Vector3(0, 1f, 0));

        //CENTER MASS TO HAND
        Gizmos.color = Color.red;
        if (centerMassPos.x <= left.x)
        {
            Gizmos.DrawLine(left + new Vector3(0, 0.7f, 0),
                left + new Vector3(DistanceCenterMassToHand(), 0.7f, 0));
        }
        else if (centerMassPos.x >= right.x)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(right + new Vector3(0, 0.7f, 0),
                right + new Vector3(DistanceCenterMassToHand(), 0.7f, 0));
        }
    }

    #endregion
}
