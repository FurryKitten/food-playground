using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class HandPlacer : MonoBehaviour, IService
{
    [SerializeField] private HandControls _hand;
    [SerializeField] private Camera _camera;

    private Grid _places4Hand; 
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = new PlayerController();
        //_playerController.Tetris.PlaceHand.started += PlaceHand; // TO DO: decide about this feature
    }

    void Start()
    {
        _places4Hand.width = 6;
        _places4Hand.height = 1;
        _places4Hand.cellsStatus = new bool[_places4Hand.width, _places4Hand.height];
        for(int i = 0; i < _places4Hand.width; i++)
            for (int j = 0; j < _places4Hand.height; ++j)
                _places4Hand.cellsStatus[i, j] = false;
        ControlsOnEnable();
    }

    void Update()
    {
        if(_hand != null)
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;

            float x = Mathf.Clamp(mousePos.x, -(_places4Hand.width * 0.5f), (_places4Hand.width * 0.5f) + 1);
            _hand.transform.localPosition = new Vector2(x, _hand.transform.localPosition.y);
        }
    }

    private void PlaceHand(InputAction.CallbackContext context)
    {
        _hand = null;
    }

    public void SetGridWidth(int trayWidth) // TO DO: use Unity Event
    {
        _places4Hand.width = trayWidth - (2 * _hand.GetHandWidth());
    }

    private void ControlsOnEnable()
    {
        _playerController.Enable();
    }

    private void ControlsOnDisable()
    {
        _playerController.Disable();
    }
}
