using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public struct Grid
{
    public int width, height;
    public Figure[,] figureGrid;
}

public class Tetris : MonoBehaviour, IService
{
    [SerializeField] private Vector2Int _figureStartPos;
    [SerializeField] private FigureSO[] _figureSOPrefabs;
    [SerializeField] private Figure _defaultFigure;
    [SerializeField] private float _movementTime;
    [SerializeField] private int _queueSize;
    [SerializeField] private HandControls _handControls;
    [SerializeField] private float _trayAngle;

    [SerializeField] private UnityEvent _onFigureFall;


    private Queue<int> _figureSOIdQueue;
    private Figure _flyingFigure = null;
    private Grid _gameSpace;
    private float _movementTimer = 0;
    private PlayerController _playerController;
    private List<Figure> _figureList = new List<Figure>();
    private float _dashTime;
    private float _figureListTimer = 0;
    private bool _dashMode = false;
    private bool _dashOffOnSpawning = false;
    private float _spawnTimer = 0;
    private bool _lastMove = false;

    private GameState _gameState;

    private float _balaceValue = 0;

    // boosts flags
    private bool _doubleCost = false;
    private bool _trayBorders = false;
    private bool _triplets = false;
    private bool _stickyTray = false;
    private bool _spoilerTriplets = false;
    private bool _goldenFish = false;
    private bool _goldenTea = false;
    private bool _goldenSmallFood = false;
    private bool _traySlowMo = false;

    // quests flags
    private bool _expressQuest = false;
    private bool _seafoodQuest = false;
    private bool _teaPartyQuest = false;
    private bool _killSpoilersQuest = false;
    private bool _collectSpoilersQuest = false;
    private bool _collectSpoiledFoodsQuest = false;

    private int _leftGridConstrain = 4;
    private int _rightGridConstrain = 14;
    private int _gridXOffsetFromWorld = 0;
    

    private int _stageNumber = 0;
    private int _trayNumber = 0;
    private int[,] _queueSizes = {  { 7, 9, 11, 14 }, 
                                    { 8, 10, 12, 15 }, 
                                    { 9, 11, 14, 17 },
                                    { 10, 12, 15, 18 }};
    private int _currentFigureNumber = 0;
    private int[] _SpawnProbability = { 7, 6, 6, 6, 3, 
                                        6, 7, 6, 6, 6, 
                                        2, 2, 2, 5, 7, 
                                        6, 7, 7, 7, 5, 
                                        7, 3, 5, 5, 3 };
    private int[] _figureSpawnPercent;

    private void Awake()
    {
        _playerController = new PlayerController();
        _playerController.Tetris.Move.started += HorizontalMove;
        _playerController.Enable();

        _playerController.Tetris.Dash.started += DashMode;
        _playerController.Tetris.Dash.canceled += DashMode;

        _playerController.Tetris.Pause.performed += (_) => ServiceLocator.Current.Get<UIService>().OnPressPause();
    }
    private void Start()
    {
        _figureSOIdQueue = new Queue<int>();
        //_figureSOIdQueue.Enqueue(0); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(1); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(0);
        //GenerateQueue();
        //GenerateQueue(_queueSizes[0, 0]);
        _gameSpace.width = 18;
        _gridXOffsetFromWorld = (_gameSpace.width - 16) / 2;
        _gameSpace.height = 15;
        _gameSpace.figureGrid = new Figure[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
            {
                _gameSpace.figureGrid[i, j] = null;
            }
        }

        _dashTime = 0.1f * _movementTime;

        _gameState = ServiceLocator.Current.Get<GameState>();

        _figureSpawnPercent = new int[25];
        _currentFigureNumber = 1;
        PrepareSmartGenerate();
        _figureSOIdQueue.Enqueue(SmartGenerateQueue(18));

        // Обновление UI очереди
        if (_figureSOIdQueue.TryPeek(out int nextFigureSOId))
            _gameState.ChangeFigureInOrder(nextFigureSOId);
        else
            _gameState.ChangeFigureInOrder(25);
    }
    private void Update()
    {
        TetrisUpdate();
    }
    public void TetrisUpdate()
    {
        if (_gameState.State == State.PAUSED)
            return;

        RotateTray();


        _figureListTimer += Time.deltaTime;
        if (_figureListTimer > _movementTime)
        {
            _figureListTimer = 0;
            if (_figureList.Count != 0)
            {
                NewVerticalMoveFigureList();
                if (transform.parent.rotation.eulerAngles.z > 10 && transform.parent.rotation.eulerAngles.z <= 30)
                {
                    NewHorizontalMoveFigureList(-1);
                }
                else if (transform.parent.rotation.eulerAngles.z < 350 && transform.parent.rotation.eulerAngles.z >= 330)
                    NewHorizontalMoveFigureList(1);
                UseFiguresEffects();
            }
        }

        if (_gameState.State == State.TETRIS)
        {
            if (_flyingFigure == null)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer > _movementTime) // Спавн
                {
                    _spawnTimer = 0;
                    if (!FigureSpawn())
                        FinishStage();
                }
            }
            else
            {
                _movementTimer += Time.deltaTime;
                if (_movementTimer >= (_dashMode ? _dashTime : _movementTime))
                {
                    MoveFlyingFigure();
                    _movementTimer = 0;
                }
            }
        }
    }
    private void FinishStage()
    {
        //stop tetris, walk, reset queue
        _gameState.SetState(State.WALK);
        _gameState.AddStage();

        _figureSOIdQueue.Clear();

        if (_stageNumber != 3)
            _stageNumber++;
        else
        {
            _stageNumber = 0;
            PrepareSmartGenerate();
        }
        
        // Обновление UI очереди
        if (_figureSOIdQueue.TryPeek(out int nextFigureSOId))
            _gameState.ChangeFigureInOrder(nextFigureSOId);
        else
            _gameState.ChangeFigureInOrder(25);

        _currentFigureNumber = 1;
        
        _figureSOIdQueue.Enqueue(SmartGenerateQueue(Random.Range(0, _figureSOPrefabs.Length)));

        //GenerateQueue(_queueSizes[_trayNumber, _stageNumber]);
    }
    private void RotateTray()
    {
        _balaceValue = _handControls.CheckBalance();

        int rotationDir = 0;

        if (_balaceValue != 0)
        {
            if (_balaceValue > 0)
            {
                rotationDir = 1;
            }
            else
            {
                rotationDir = -1;
            }
        }
        else
        {
            if (transform.parent.rotation.eulerAngles.z >= 340 && transform.parent.rotation.eulerAngles.z < 360f)
            {
                rotationDir = 1;
            }
            else
            {
                if (transform.parent.rotation.eulerAngles.z > 0.1)
                {
                    rotationDir = -1;
                }
                else
                {
                    transform.parent.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        transform.parent.Rotate(0, 0, rotationDir * _trayAngle * Time.deltaTime);

        transform.parent.rotation = Quaternion.Euler(0, 0, RotationClamp(transform.parent.rotation.eulerAngles.z));
    }
    private void MoveFlyingFigure()
    {
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = _flyingFigurePos.x + _gridXOffsetFromWorld;
        int gridY = _flyingFigurePos.y;

        bool checkGridSpace = false;
        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridY - pos.y - 1) >= 0)
            {
                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != null)
                {
                    checkGridSpace = true;
                    break;
                }
            }
            else
            {
                checkGridSpace = true;
                break;
            }
       
        if (!checkGridSpace)
        { 
            _flyingFigure.Fall(); 
        }
        else // Ставим на доску
        {
            if (_lastMove)
            {
                bool smashSpoiler = true;
                List<int> spoilersX = new List<int>();
                List<int> spoilersY = new List<int>();

                foreach (Vector2Int pos in _flyingFigure.GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = _flyingFigure;

                    if (_dashMode && _flyingFigure.Index != 18)
                        if (gridY - pos.y - 1 >= 0)
                        {
                            if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != null &&
                                _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != _flyingFigure)
                            {
                                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1].Index == 18)
                                {
                                    spoilersX.Add(gridX + pos.x);
                                    spoilersY.Add(gridY - pos.y - 1);
                                }
                                else
                                    smashSpoiler = false;
                            }
                        }
                        else
                            smashSpoiler = false;
                }

                if(smashSpoiler)
                {
                    // Давим спойлеры
                    for (int i = 0; i < spoilersX.Count; i++)
                    {
                        _figureList.Remove(_gameSpace.figureGrid[spoilersX[i], spoilersY[i]]);
                        _gameSpace.figureGrid[spoilersX[i], spoilersY[i]].DestroySpoiler();
                        _gameSpace.figureGrid[spoilersX[i], spoilersY[i]].FlyAway();
                        _gameSpace.figureGrid[spoilersX[i], spoilersY[i]] = null;
                    }
                }

                _figureList.Add(_flyingFigure);
                _handControls.AddFigures(_figureList);
                _gameState.AddTrayMoney(_flyingFigure.GetProfit());

                _flyingFigure.SetMaterial();

                _flyingFigure = null;
                _spawnTimer = 0;//_movementTime;

                ServiceLocator.Current.Get<AudioService>().PlayTetrisLanding();
            }
            else
            {
                _lastMove = true;
                _movementTimer = -_movementTime;
            }
        }
    }
    private bool FigureSpawn()
    {
        if (_figureSOIdQueue.TryPeek(out int figureSOId)) // Фигура заспавнилась
        {
            Vector2 posParent = transform.parent.position;
            _flyingFigure = Instantiate(_defaultFigure, transform.parent);
            _flyingFigure.Init(_figureSOPrefabs[figureSOId]);
            _flyingFigure.name = "FlyingFigure";
            _figureSOIdQueue.Dequeue();

            int spawnOffset = 0;
            if(_flyingFigure.Width > 0.7f)
            {
                posParent.x += 1;
                spawnOffset += 1;
            }

            _flyingFigure.SetPosition(_figureStartPos.x - spawnOffset, _figureStartPos.y);
            _flyingFigure.SetWorldPosition(_figureStartPos - posParent);

            ServiceLocator.Current.Get<AudioService>().PlayTetrisSpawn();
            if (_doubleCost && _flyingFigure.Index != 18)
            {
                if ((Random.Range(0f, 1f) < 0.1f))
                {
                    _flyingFigure.SetDoubleCost();
                    ServiceLocator.Current.Get<AudioService>().PlayTetrisGoldSpawn();
                }
            }
            else
            {
                if (_goldenFish && (_flyingFigure.Index == 1 || _flyingFigure.Index == 5))
                {
                    _flyingFigure.SetDoubleCost();
                    ServiceLocator.Current.Get<AudioService>().PlayTetrisGoldSpawn();
                }
                else
                {
                    if(_goldenTea && (_flyingFigure.Index == 22 || _flyingFigure.Index == 21))
                    {
                        _flyingFigure.SetDoubleCost();
                        ServiceLocator.Current.Get<AudioService>().PlayTetrisGoldSpawn();
                    }
                    else
                    {
                        if(_goldenSmallFood && (_flyingFigure.Index >= 10 && _flyingFigure.Index <= 12))
                        {
                            _flyingFigure.SetDoubleCost();
                            ServiceLocator.Current.Get<AudioService>().PlayTetrisGoldSpawn();
                        }
                    }
                }
            }


            bool placeCheck = true;
            // Если нет места для спавна - удаляем фигуры
            foreach (Vector2Int p in _flyingFigure.GetForm())
            {
                if (_gameSpace.figureGrid[_figureStartPos.x + _gridXOffsetFromWorld - spawnOffset + p.x, _figureStartPos.y - p.y] != null)
                {
                    int index = _figureList.IndexOf(_gameSpace.figureGrid[_figureStartPos.x + _gridXOffsetFromWorld - spawnOffset + p.x, _figureStartPos.y - p.y]);
                    Vector2Int _fPos = _figureList[index].GetPosition();
                    int gridX = (_fPos.x) + _gridXOffsetFromWorld;
                    int gridY = (_fPos.y);

                    foreach (Vector2Int pos in _figureList[index].GetForm())
                    {
                        _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                    }
                    _figureList[index].FlyAway(-1);
                    _onFigureFall?.Invoke();
                    _figureList.RemoveAt(index);
                    placeCheck = false;
                }
            }

            if(!placeCheck)
            {
                _flyingFigure.SetMaterial();
                _flyingFigure.FlyAway();
                _flyingFigure = null;
                _spawnTimer = 0;
                _handControls.AddFigures(_figureList);
                ServiceLocator.Current.Get<AudioService>().PlayTetrisBadSpawn();
            }

            // Отмена _dashModa при спавне
            if (_dashMode)
            {
                _dashMode = false;
                _dashOffOnSpawning = true;
            }

            // Генерация следующей фигуры
            _currentFigureNumber++;
            if(_currentFigureNumber <= _queueSizes[_trayNumber, _stageNumber])
            {
                _figureSOIdQueue.Enqueue(SmartGenerateQueue(figureSOId));
            }

            // Обновление UI очереди
            if (_figureSOIdQueue.TryPeek(out int nextFigureSOId))
                _gameState.ChangeFigureInOrder(nextFigureSOId);
            else
                _gameState.ChangeFigureInOrder(25);

            return true;
        }
        
        return false;
    }
    private float RotationClamp(float angle)
    {
        if(angle > 20 && angle < 30)
        {
            return Mathf.Clamp(angle, 0, 20);
        }
        else
        {
            if(angle > 20)
            {
                return Mathf.Clamp(angle, 340, 360);
            }
        }

        return angle;
    }
    private bool checkHorizontalMove(int dir)
    {
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = (_flyingFigurePos.x) + dir + _gridXOffsetFromWorld;
        int gridY = (_flyingFigurePos.y);

        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridX + pos.x ) >= _leftGridConstrain && (gridX + pos.x ) < _rightGridConstrain && gridY - pos.y >= 0)
            {
                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] != null)
                    return true;
            }
            else
                return true;

        return false;
    }
    private void HorizontalMove(InputAction.CallbackContext context)
    {
        if (_flyingFigure == null)
            return;

        float inpDir = _playerController.Tetris.Move.ReadValue<float>();

        int dir = 0;

        if (inpDir > 0)
            dir = 1;
        else if (inpDir < 0)
            dir = -1;

        if (!checkHorizontalMove(dir))
        {
            _flyingFigure.HorizontalMove(dir);
        }
        
    }
    private void NewHorizontalMoveFigureList(int dir)
    {
        bool[,] gridMask = new bool[_gameSpace.width, _gameSpace.height];

        for (int i = 0; i < _gameSpace.width; i++)
            for (int j = 0; j < _gameSpace.height; j++)
                gridMask[i, j] = false;

        if (_trayBorders)
        {
            gridMask[_leftGridConstrain - 1, 0] = true;
            gridMask[_rightGridConstrain, 0] = true;
        }

        if(_stickyTray)
        {
            for (int j = 0; j < _gameSpace.height; j++)
                gridMask[j, 0] = true;
        }

        if (_flyingFigure != null)
        {
            Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
            int gridXFlying = (_flyingFigurePos.x) + _gridXOffsetFromWorld;
            int gridYFlying = (_flyingFigurePos.y);

            foreach(Vector2Int pos in _flyingFigure.GetForm())
                gridMask[gridXFlying + pos.x, gridYFlying - pos.y] = true;
        }

        
        for(int j = 0; j < _gameSpace.height; j++)
            for (int i = (dir > 0) ? _gameSpace.width - 1 : 0; 
                i != ((dir > 0) ? -1 : _gameSpace.width); i -= dir)
            {
                if (_gameSpace.figureGrid[i, j] != null && !gridMask[i, j])
                {
                    Vector2Int figurePos = _gameSpace.figureGrid[i, j].GetPosition();
                    int gridX = (figurePos.x) + _gridXOffsetFromWorld + dir;
                    int gridY = (figurePos.y);

                    bool bordured = false;
                    
                    foreach (Vector2Int pos in _gameSpace.figureGrid[i, j].GetForm())
                    {
                        if (gridX + pos.x >= 0 && gridX + pos.x < _gameSpace.width)
                            if (gridMask[gridX + pos.x, gridY - pos.y])
                            {
                                bordured = true;
                                break;
                            }
                    }

                    if (bordured)
                    {
                        int newJ = j;
                        foreach (Vector2Int pos in _gameSpace.figureGrid[i, j].GetForm())
                        {
                            gridMask[gridX + pos.x - dir, gridY - pos.y] = true;
                            if (gridY - pos.y < newJ)
                                newJ = gridY - pos.y;
                        }
                        i = (dir > 0) ? _gameSpace.width - 1 : 0; 
                        j = newJ;
                    }
                    
                }
            }

        List<int> lostFigureIndexes = new List<int>();
        List<int> sortedFigureIndexes = new List<int>();
        for (int i = 0; i < _figureList.Count; i++)
            sortedFigureIndexes.Add(i);

        if (dir < 0)
        {
            for (int i = 0; i < sortedFigureIndexes.Count; i++)
                for (int j = 0; j < sortedFigureIndexes.Count - 1; j++)
                {
                    if (_figureList[sortedFigureIndexes[j]].GetPosition().x
                        > _figureList[sortedFigureIndexes[j + 1]].GetPosition().x)
                    {
                        int temp = sortedFigureIndexes[j];
                        sortedFigureIndexes[j] = sortedFigureIndexes[j + 1];
                        sortedFigureIndexes[j + 1] = temp;
                    }
                }

            for (int i = 0; i < sortedFigureIndexes.Count; i++)
                for (int j = 0; j < sortedFigureIndexes.Count - 1; j++)
                {
                    if (_figureList[sortedFigureIndexes[j]].GetPosition().x ==
                        _figureList[sortedFigureIndexes[j + 1]].GetPosition().x)
                    {
                        if (_figureList[sortedFigureIndexes[j]].GetPosition().y - (_figureList[sortedFigureIndexes[j]].Height - 0.2f) * 5 >
                            _figureList[sortedFigureIndexes[j + 1]].GetPosition().y - (_figureList[sortedFigureIndexes[j + 1]].Height - 0.2f) * 5)
                        {
                            int temp = sortedFigureIndexes[j];
                            sortedFigureIndexes[j] = sortedFigureIndexes[j + 1];
                            sortedFigureIndexes[j + 1] = temp;
                        }
                    }
                }
        }
        else
        {
            for (int i = 0; i < sortedFigureIndexes.Count; i++)
                for (int j = 0; j < sortedFigureIndexes.Count - 1; j++)
                {
                    if (_figureList[sortedFigureIndexes[j]].GetPosition().x + _figureList[sortedFigureIndexes[j]].Width * 5
                        < _figureList[sortedFigureIndexes[j + 1]].GetPosition().x + _figureList[sortedFigureIndexes[j + 1]].Width * 5)
                    {
                        int temp = sortedFigureIndexes[j];
                        sortedFigureIndexes[j] = sortedFigureIndexes[j + 1];
                        sortedFigureIndexes[j + 1] = temp;
                    }
                }

            for (int i = 0; i < sortedFigureIndexes.Count; i++)
                for (int j = 0; j < sortedFigureIndexes.Count - 1; j++)
                {
                    if (_figureList[sortedFigureIndexes[j]].GetPosition().x + _figureList[sortedFigureIndexes[j]].Width * 5 ==
                        _figureList[sortedFigureIndexes[j + 1]].GetPosition().x + _figureList[sortedFigureIndexes[j + 1]].Width * 5)
                    {
                        if (_figureList[sortedFigureIndexes[j]].GetPosition().y - (_figureList[sortedFigureIndexes[j]].Height - 0.2f) * 5 >
                            _figureList[sortedFigureIndexes[j + 1]].GetPosition().y - (_figureList[sortedFigureIndexes[j + 1]].Height - 0.2f) * 5)
                        {
                            int temp = sortedFigureIndexes[j];
                            sortedFigureIndexes[j] = sortedFigureIndexes[j + 1];
                            sortedFigureIndexes[j + 1] = temp;
                        }
                    }
                }
        }
        
        for(int i = 0; i < _gameSpace.width; i++)
            for(int j = 0; j < _gameSpace.height; j++)
            {
                _gameSpace.figureGrid[i, j] = null;
            }

        foreach(int i in sortedFigureIndexes)
        {
            Vector2Int figurePos = _figureList[i].GetPosition();
            int gridX = (figurePos.x) + _gridXOffsetFromWorld;
            int gridY = (figurePos.y);

            if(!gridMask[gridX + _figureList[i].GetForm()[0].x, gridY - _figureList[i].GetForm()[0].y])
            {
                _figureList[i].HorizontalMove(dir);

                bool lost = false;

                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    if(gridX + dir + pos.x < _leftGridConstrain || gridX + dir + pos.x == _rightGridConstrain)
                    {
                        lost = true;
                        if (_figureList[i].Index != 0 && _figureList[i].Index != 14 
                            && _figureList[i].Index != 15 && _figureList[i].Index != 21)
                        {
                            break;
                        } 
                        
                    }
                    else
                    {
                        if (_figureList[i].Index != 0 && _figureList[i].Index != 14
                            && _figureList[i].Index != 15 && _figureList[i].Index != 21)
                        {
                            continue;
                        }

                        if (gridX + 2 * dir + pos.x == _leftGridConstrain - 1 || gridX + 2 * dir + pos.x == _rightGridConstrain)
                        {
                            if (gridMask[gridX + 2 * dir + pos.x, gridY - pos.y])
                            {
                                lost = false;
                                break;
                            }
                        }
                    }
                }

                if (lost)
                {
                    lostFigureIndexes.Add(i);
                    _figureList[i].FlyAway(dir);
                    _onFigureFall?.Invoke();
                }

            }
        }

        if (lostFigureIndexes.Count > 0)
        {
            for (int i = 0; i < lostFigureIndexes.Count; i++)
                for (int j = 0; j < lostFigureIndexes.Count - 1; j++)
                    if (lostFigureIndexes[j] > lostFigureIndexes[j + 1])
                    {
                        int tmp = lostFigureIndexes[j];
                        lostFigureIndexes[j] = lostFigureIndexes[j + 1];
                        lostFigureIndexes[j + 1] = tmp;
                    }

            int offset = 0;
            foreach (int i in lostFigureIndexes)
            {
                _figureList.RemoveAt(i - offset);
                offset++;
            }

            _handControls.AddFigures(_figureList);

        }

        foreach (Figure figure in _figureList)
        {
            Vector2Int figurePos = figure.GetPosition();
            int gridX = (figurePos.x) + _gridXOffsetFromWorld;
            int gridY = (figurePos.y);

            foreach (Vector2Int pos in figure.GetForm())
            {
                _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = figure;
            }
        }

    } 
    private void NewVerticalMoveFigureList()
    {
        bool[,] gridMask = new bool[_gameSpace.width, _gameSpace.height];
        
        for (int i = 0; i < _gameSpace.width; i++)
            for (int j = 0; j < _gameSpace.height; j++)
                gridMask[i, j] = false;

        if (_flyingFigure != null)
        {
            Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
            int gridXFlying = (_flyingFigurePos.x) + _gridXOffsetFromWorld;
            int gridYFlying = (_flyingFigurePos.y);

            foreach (Vector2Int pos in _flyingFigure.GetForm())
                gridMask[gridXFlying + pos.x, gridYFlying - pos.y] = true;
        }

        for (int j = 0; j < _gameSpace.height; j++)
            for (int i = _gridXOffsetFromWorld; i < _gameSpace.width; i++)
            {
                if (_gameSpace.figureGrid[i, j] != null && !gridMask[i, j])
                {
                    Vector2Int figurePos = _gameSpace.figureGrid[i, j].GetPosition();
                    int gridX = (figurePos.x) + _gridXOffsetFromWorld;
                    int gridY = (figurePos.y);

                    bool bordured = false;

                    foreach (Vector2Int pos in _gameSpace.figureGrid[i, j].GetForm())
                    {
                        if (gridY - pos.y - 1 >= 0)
                        {
                            if (gridMask[gridX + pos.x, gridY - pos.y - 1])
                            {
                                bordured = true;
                                break;
                            }
                        }
                        else
                        {
                            bordured = true;
                            break;
                        }
                    }

                    if (bordured)
                    {
                        foreach (Vector2Int pos in _gameSpace.figureGrid[i, j].GetForm())
                        {
                            gridMask[gridX + pos.x, gridY - pos.y] = true;
                        }
                    }
                }
            }

        for (int i = 0; i < _figureList.Count; i++)
        {
            Vector2Int figurePos = _figureList[i].GetPosition();
            int gridX = (figurePos.x) + _gridXOffsetFromWorld;
            int gridY = (figurePos.y);

            if (!gridMask[gridX + _figureList[i].GetForm()[0].x, gridY - _figureList[i].GetForm()[0].y])
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                }

                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] = _figureList[i];
                }

                _figureList[i].Fall();
            }
        }

    }
    private void DashMode(InputAction.CallbackContext context)
    {
        if (_dashOffOnSpawning && !_playerController.Tetris.Dash.triggered)
        {
            _dashMode = false;
            _dashOffOnSpawning = false;
        }
        else
            _dashMode = !_dashMode;
    }
    private void UseFiguresEffects()
    {
        List<Figure> deletedFigures = new List<Figure>();
        for(int i = 0; i < _figureList.Count; ++i)
        {
            if(_figureList[i].Index == 18) // Проверка спойлеров
            {
                List<Vector2Int> tripletList = new List<Vector2Int>();
                Vector2Int fPos = _figureList[i].GetPosition();
                int gridX = (fPos.x) + _gridXOffsetFromWorld;
                int gridY = (fPos.y);

                if(gridX + 1 < _rightGridConstrain)
                    if (_gameSpace.figureGrid[gridX + 1, gridY] != null)
                    {
                        if (!deletedFigures.Contains(_gameSpace.figureGrid[gridX + 1, gridY]))
                        {
                            if (_gameSpace.figureGrid[gridX + 1, gridY].Index != 18)
                            {
                                if (_figureList[i].IsGold)
                                    //вызов метода золочения продукт
                                    _gameSpace.figureGrid[gridX + 1, gridY].ChangeGoldenStatus(true);
                                else
                                    //вызов метода порченья продукта
                                    _gameSpace.figureGrid[gridX + 1, gridY].ChangeSpoiledStatus(true);
                            }
                            else
                            {
                                if (_figureList[i].IsGold)
                                {
                                    //Золотим соседний спойлер
                                    if (!_gameSpace.figureGrid[gridX + 1, gridY].IsGold)
                                        _gameSpace.figureGrid[gridX + 1, gridY].DoGoldSpoiler();
                                }
                                else
                                    // триплет спойлеров
                                    if (!_gameSpace.figureGrid[gridX + 1, gridY].IsGold && _spoilerTriplets)
                                    {
                                        Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX + 1, gridY].GetPosition().x
                                        + _gameSpace.figureGrid[gridX + 1, gridY].GetForm()[0].x + _gridXOffsetFromWorld,
                                             _gameSpace.figureGrid[gridX + 1, gridY].GetPosition().y - _gameSpace.figureGrid[gridX + 1, gridY].GetForm()[0].y);
                                        if (!tripletList.Contains(figureAncor))
                                        {
                                            tripletList.Add(figureAncor);
                                        }
                                    }
                            }
                        }
                    }

                if (gridX - 1 >= _leftGridConstrain)
                    if (_gameSpace.figureGrid[gridX - 1, gridY] != null)
                    {
                        if (!deletedFigures.Contains(_gameSpace.figureGrid[gridX - 1, gridY]))
                        {
                            if (_gameSpace.figureGrid[gridX - 1, gridY].Index != 18)
                            {
                                if (_figureList[i].IsGold)
                                    //вызов метода золочения продукт
                                    _gameSpace.figureGrid[gridX - 1, gridY].ChangeGoldenStatus(true);
                                else
                                    //вызов метода порченья продукта
                                    _gameSpace.figureGrid[gridX - 1, gridY].ChangeSpoiledStatus(true);
                            }
                            else
                            {
                                if (_figureList[i].IsGold)
                                {
                                    //Золотим соседний спойлер
                                    if (!_gameSpace.figureGrid[gridX - 1, gridY].IsGold)
                                        _gameSpace.figureGrid[gridX - 1, gridY].DoGoldSpoiler();
                                }
                                else
                                    // триплет спойлеров
                                    if (!_gameSpace.figureGrid[gridX - 1, gridY].IsGold && _spoilerTriplets)
                                    {
                                        Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX - 1, gridY].GetPosition().x
                                        + _gameSpace.figureGrid[gridX - 1, gridY].GetForm()[0].x + _gridXOffsetFromWorld,
                                             _gameSpace.figureGrid[gridX - 1, gridY].GetPosition().y - _gameSpace.figureGrid[gridX - 1, gridY].GetForm()[0].y);
                                        if (!tripletList.Contains(figureAncor))
                                        {
                                            tripletList.Add(figureAncor);
                                        }
                                    }
                            }
                        }
                    }


                if (_gameSpace.figureGrid[gridX, gridY + 1] != null)
                {
                    if (!deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY + 1]))
                    {
                        if (_gameSpace.figureGrid[gridX, gridY + 1].Index != 18)
                        {
                            if (_figureList[i].IsGold)
                                //вызов метода золочения продукт
                                _gameSpace.figureGrid[gridX, gridY + 1].ChangeGoldenStatus(true);
                            else
                                //вызов метода порченья продукта
                                _gameSpace.figureGrid[gridX, gridY + 1].ChangeSpoiledStatus(true);
                        }
                        else
                        {
                            if (_figureList[i].IsGold)
                            {
                                //Золотим соседний спойлер
                                if (!_gameSpace.figureGrid[gridX, gridY + 1].IsGold)
                                    _gameSpace.figureGrid[gridX, gridY + 1].DoGoldSpoiler();
                            }
                            else
                                // триплет спойлеров
                                if (!_gameSpace.figureGrid[gridX, gridY + 1].IsGold && _spoilerTriplets)
                                {
                                    Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX, gridY + 1].GetPosition().x
                                    + _gameSpace.figureGrid[gridX, gridY + 1].GetForm()[0].x + _gridXOffsetFromWorld,
                                         _gameSpace.figureGrid[gridX, gridY + 1].GetPosition().y - _gameSpace.figureGrid[gridX, gridY + 1].GetForm()[0].y);
                                    if (!tripletList.Contains(figureAncor))
                                    {
                                        tripletList.Add(figureAncor);
                                    }
                                }
                        }
                    }
                }

                if (gridY - 1  >= 0)
                    if (_gameSpace.figureGrid[gridX, gridY - 1] != null)
                    {
                        if (!deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY - 1]))
                        {
                            if (_gameSpace.figureGrid[gridX, gridY - 1].Index != 18)
                            {
                                if (_figureList[i].IsGold)
                                    //вызов метода золочения продукт
                                    _gameSpace.figureGrid[gridX, gridY - 1].ChangeGoldenStatus(true);
                                else
                                    //вызов метода порченья продукта
                                    _gameSpace.figureGrid[gridX, gridY - 1].ChangeSpoiledStatus(true);
                            }
                            else
                            {
                                if (_figureList[i].IsGold)
                                {
                                    //Золотим соседний спойлер
                                    if (!_gameSpace.figureGrid[gridX, gridY - 1].IsGold)
                                        _gameSpace.figureGrid[gridX, gridY - 1].DoGoldSpoiler();
                                }
                                else
                                    // триплет спойлеров
                                    if (!_gameSpace.figureGrid[gridX, gridY - 1].IsGold && _spoilerTriplets)
                                    {
                                        Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX, gridY - 1].GetPosition().x
                                        + _gameSpace.figureGrid[gridX, gridY - 1].GetForm()[0].x + _gridXOffsetFromWorld,
                                             _gameSpace.figureGrid[gridX, gridY - 1].GetPosition().y - _gameSpace.figureGrid[gridX, gridY - 1].GetForm()[0].y);
                                        if (!tripletList.Contains(figureAncor))
                                        {
                                            tripletList.Add(figureAncor);
                                        }
                                    }
                            }
                        }
                    }

                if (tripletList.Count > 1 && _spoilerTriplets)
                {
                    _figureList[i].CreateTriplet();

                    for (int j = 0; j < 2; ++j)
                    {
                        Vector2Int fPosition = _gameSpace.figureGrid[tripletList[j].x, tripletList[j].y].GetPosition();
                        int gridXx = (fPosition.x + _gridXOffsetFromWorld);
                        int gridYy = (fPosition.y);

                        deletedFigures.Add(_gameSpace.figureGrid[tripletList[j].x, tripletList[j].y]);

                        foreach (Vector2Int pos in _gameSpace.figureGrid[tripletList[j].x, tripletList[j].y].GetForm())
                        {
                            _gameSpace.figureGrid[gridXx + pos.x, gridYy - pos.y] = null;
                        }
                    }
                }
            }
            else if(_figureList[i].IsSpoiled || _figureList[i].IsTimedGold)
            {
                bool checkSpoiler = false;
                bool checkGoldSpoiler = false;
                Vector2Int fPos = _figureList[i].GetPosition();
                int gridX = (fPos.x) + _gridXOffsetFromWorld;
                int gridY = (fPos.y);

                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    gridX += pos.x;
                    gridY -= pos.y;

                    if (gridX + 1 < _rightGridConstrain)
                        if (_gameSpace.figureGrid[gridX + 1, gridY] != null)
                        {
                            if (_gameSpace.figureGrid[gridX + 1, gridY].Index == 18 
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX + 1, gridY]))
                            {
                                if (_gameSpace.figureGrid[gridX + 1, gridY].IsGold)
                                {
                                    checkGoldSpoiler = true;
                                }
                                else
                                {
                                    checkSpoiler = true;
                                }
                            }
                        }

                    if (gridX - 1 >= _leftGridConstrain)
                        if (_gameSpace.figureGrid[gridX - 1, gridY] != null)
                        {
                            if(_gameSpace.figureGrid[gridX - 1, gridY].Index == 18
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX - 1, gridY]))
                            {
                                if (_gameSpace.figureGrid[gridX - 1, gridY].IsGold)
                                {
                                    checkGoldSpoiler = true;
                                }
                                else
                                {
                                    checkSpoiler = true;
                                }
                            }
                        }

                    if (_gameSpace.figureGrid[gridX, gridY + 1] != null)
                    {
                        if(_gameSpace.figureGrid[gridX, gridY + 1].Index == 18
                            && !deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY + 1]))
                        {
                            if (_gameSpace.figureGrid[gridX, gridY + 1].IsGold)
                            {
                                checkGoldSpoiler = true;
                            }
                            else
                            {
                                checkSpoiler = true;
                            }
                        }
                    }

                    if (gridY - 1 >= 0)
                        if (_gameSpace.figureGrid[gridX, gridY - 1] != null)
                        {
                            if(_gameSpace.figureGrid[gridX, gridY - 1].Index == 18
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY - 1]))
                            {
                                if (_gameSpace.figureGrid[gridX, gridY - 1].IsGold)
                                {
                                    checkGoldSpoiler = true;
                                }
                                else
                                {
                                    checkSpoiler = true;
                                }
                            }
                        }

                    if (checkSpoiler && checkGoldSpoiler)
                        break;

                    gridX -= pos.x;
                    gridY += pos.y;
                }

                if (!checkSpoiler)
                    _figureList[i].ChangeSpoiledStatus(false);

                if (!checkGoldSpoiler)
                    _figureList[i].ChangeGoldenStatus(false);

            }
            else if(!_figureList[i].IsGold && _triplets)
            {
                List<Vector2Int> tripletList = new List<Vector2Int>();
                Vector2Int fPos = _figureList[i].GetPosition();
                int gridX = (fPos.x) + _gridXOffsetFromWorld;
                int gridY = (fPos.y);

                tripletList.Add(new Vector2Int(gridX + _figureList[i].GetForm()[0].x, 
                    gridY - _figureList[i].GetForm()[0].y));

                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    gridX += pos.x;
                    gridY -= pos.y;

                    if (gridX + 1 < _rightGridConstrain)
                        if (_gameSpace.figureGrid[gridX + 1, gridY] != null 
                            && _gameSpace.figureGrid[gridX + 1, gridY] != _figureList[i])
                        {
                            if (_gameSpace.figureGrid[gridX + 1, gridY].Index == _figureList[i].Index 
                                && !_gameSpace.figureGrid[gridX + 1, gridY].IsSpoiled
                                && !_gameSpace.figureGrid[gridX + 1, gridY].IsGold
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX + 1, gridY]))
                            {
                                Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX + 1, gridY].GetPosition().x 
                                    + _gameSpace.figureGrid[gridX + 1, gridY].GetForm()[0].x + _gridXOffsetFromWorld,
                                         _gameSpace.figureGrid[gridX + 1, gridY].GetPosition().y - _gameSpace.figureGrid[gridX + 1, gridY].GetForm()[0].y);
                                if (!tripletList.Contains(figureAncor))
                                {
                                    tripletList.Add(figureAncor);
                                }
                            }
                        }

                    if (gridX - 1 >= _leftGridConstrain)
                        if (_gameSpace.figureGrid[gridX - 1, gridY] != null
                            && _gameSpace.figureGrid[gridX - 1, gridY] != _figureList[i])
                        {
                            if (_gameSpace.figureGrid[gridX - 1, gridY].Index == _figureList[i].Index
                                && !_gameSpace.figureGrid[gridX - 1, gridY].IsSpoiled
                                && !_gameSpace.figureGrid[gridX - 1, gridY].IsGold
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX - 1, gridY]))
                            {
                                Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX - 1, gridY].GetPosition().x 
                                    + _gameSpace.figureGrid[gridX - 1, gridY].GetForm()[0].x + _gridXOffsetFromWorld,
                                         _gameSpace.figureGrid[gridX - 1, gridY].GetPosition().y - _gameSpace.figureGrid[gridX - 1, gridY].GetForm()[0].y);
                                if (!tripletList.Contains(figureAncor))
                                {
                                    tripletList.Add(figureAncor);
                                }
                            }
                        }

                    if (_gameSpace.figureGrid[gridX, gridY + 1] != null
                            && _gameSpace.figureGrid[gridX, gridY + 1] != _figureList[i])
                    {
                        if (_gameSpace.figureGrid[gridX, gridY + 1].Index == _figureList[i].Index
                                && !_gameSpace.figureGrid[gridX, gridY + 1].IsSpoiled
                                && !_gameSpace.figureGrid[gridX, gridY + 1].IsGold
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY + 1]))
                        {
                            Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX, gridY + 1].GetPosition().x 
                                + _gameSpace.figureGrid[gridX, gridY + 1].GetForm()[0].x + _gridXOffsetFromWorld,
                                         _gameSpace.figureGrid[gridX, gridY + 1].GetPosition().y - _gameSpace.figureGrid[gridX, gridY + 1].GetForm()[0].y);
                            if (!tripletList.Contains(figureAncor))
                            {
                                tripletList.Add(figureAncor);
                            }
                        }
                    }

                    if (gridY - 1 >= 0)
                        if (_gameSpace.figureGrid[gridX, gridY - 1] != null
                            && _gameSpace.figureGrid[gridX, gridY - 1] != _figureList[i])
                        {
                            if (_gameSpace.figureGrid[gridX, gridY - 1].Index == _figureList[i].Index
                                && !_gameSpace.figureGrid[gridX, gridY - 1].IsSpoiled
                                && !_gameSpace.figureGrid[gridX, gridY - 1].IsGold
                                && !deletedFigures.Contains(_gameSpace.figureGrid[gridX, gridY - 1]))
                            {
                                Vector2Int figureAncor = new Vector2Int(_gameSpace.figureGrid[gridX, gridY - 1].GetPosition().x 
                                    + _gameSpace.figureGrid[gridX, gridY - 1].GetForm()[0].x + _gridXOffsetFromWorld,
                                        _gameSpace.figureGrid[gridX, gridY - 1].GetPosition().y - _gameSpace.figureGrid[gridX, gridY - 1].GetForm()[0].y);
                                if (!tripletList.Contains(figureAncor))
                                {
                                    tripletList.Add(figureAncor);
                                }
                            }
                        }

                    gridX -= pos.x;
                    gridY += pos.y;
                }

                if(tripletList.Count > 2)
                {
                    int lowerY = tripletList[0].y;
                    int lowerInd = 0;

                    for(int j = 1; j < 3; ++j)
                        if(lowerY > tripletList[j].y)
                        {
                            lowerY = tripletList[j].y;
                            lowerInd = j;
                        }

                    
                    for (int j = 0; j < 3; ++j)
                    {
                        if (j != lowerInd)
                        {
                            Vector2Int fPosition = _gameSpace.figureGrid[tripletList[j].x, tripletList[j].y].GetPosition();
                            int gridXx = (fPosition.x + _gridXOffsetFromWorld);
                            int gridYy = (fPosition.y);

                            deletedFigures.Add(_gameSpace.figureGrid[tripletList[j].x, tripletList[j].y]);

                            foreach (Vector2Int pos in _gameSpace.figureGrid[tripletList[j].x, tripletList[j].y].GetForm())
                            {
                                _gameSpace.figureGrid[gridXx + pos.x, gridYy - pos.y] = null;
                            }
                        }
                        else
                        {
                            _gameSpace.figureGrid[tripletList[j].x, tripletList[j].y].CreateTriplet();
                        }
                    }

                    
                }
            } 
            
        }

        if (deletedFigures.Count > 0)
        {
            foreach (Figure figure in deletedFigures)
            {
                figure.CombineIntoTriplet();
                _figureList.Remove(figure);
            }
            _handControls.AddFigures(_figureList);
        }
    }
    private void GenerateQueue()
    {
        for(int i = 0; i < _queueSize; i++)
            _figureSOIdQueue.Enqueue(Random.Range(0, _figureSOPrefabs.Length));
    }
    private void GenerateQueue(int queueSize)
    {
        for (int i = 0; i < queueSize; i++)
            _figureSOIdQueue.Enqueue(Random.Range(0, _figureSOPrefabs.Length));
    }
    private void PrepareSmartGenerate()
    {
        if(!_expressQuest)
            for (int i = 0; i < _figureSpawnPercent.Length; ++i)
                _figureSpawnPercent[i] = _SpawnProbability[i];
        else
            for (int i = 0; i < _figureSpawnPercent.Length; ++i)
                _figureSpawnPercent[i] = 1;

        if (_spoilerTriplets)
            _figureSpawnPercent[18] *= 10;

        if (_collectSpoiledFoodsQuest)
            _figureSpawnPercent[18] *= 2;
        else
        {
            if ((_collectSpoilersQuest || _killSpoilersQuest))
            {
                if(!_spoilerTriplets)
                    _figureSpawnPercent[18] *= 10;
            }
            else 
            {
                if(_seafoodQuest)
                {
                    for (int i = 0; i < 6; ++i)
                        _figureSpawnPercent[i] = Random.Range(0, 3);
                }
                else
                {
                    if(_teaPartyQuest)
                    {
                        _figureSpawnPercent[21] = Random.Range(0, 3);
                        _figureSpawnPercent[22] = 7;
                    }
                }
            }
        }
    }
    private int SmartGenerateQueue(int lastFigureIndex)
    {
        int[] _timedFigureSpawnPercent = { 0, 0, 0, 0, 0,
                                           0, 0, 0, 0, 0,
                                           0, 0, 0, 0, 0,
                                           0, 0, 0, 0, 0,
                                           0, 0, 0, 0, 0 };
        int cupCounter = 0;

        if(_seafoodQuest)
        {
            for (int i = 0; i < 6; ++i)
                _figureSpawnPercent[i] += Random.Range(_stageNumber, _stageNumber + 2);

            if (_figureList.Count > 0)
                foreach (Figure figure in _figureList)
                    if (figure.Index >= 0 && figure.Index < 6)
                        _figureSpawnPercent[figure.Index] = 2;
        }
        else 
        {
            if(_teaPartyQuest)
            {
                _figureSpawnPercent[21] += Random.Range(_stageNumber + 1, _stageNumber + 2); // Teapot
                _figureSpawnPercent[22] += Random.Range(_stageNumber + 4 - cupCounter, _stageNumber + 6 - cupCounter);

                if (_figureList.Count > 0)
                    foreach (Figure figure in _figureList)
                    {
                        if (figure.Index == 21)
                            _figureSpawnPercent[figure.Index] = 2;
                        else
                        {
                            if(figure.Index == 22)
                                if(cupCounter < 4)
                                {
                                    cupCounter++;
                                }
                                else
                                {
                                    _figureSpawnPercent[figure.Index] = 2;
                                }
                        }
                    }

            }
            else 
            {
                if(_expressQuest)
                {
                    if (_figureList.Count > 0)
                    {
                        for (int i = _leftGridConstrain; i < _rightGridConstrain; i++)
                            for (int j = _figureStartPos.y; j >= 0; j--)
                            {
                                if (j - 1 >= 0)
                                {
                                    if (_gameSpace.figureGrid[i, j - 1] != null)
                                    {
                                        foreach (FigureSO figureSO in _figureSOPrefabs)
                                        {
                                            bool spaceCheck = true;
                                            int offsetJ = Mathf.RoundToInt(figureSO.heightTex * 5) - 1;

                                            foreach (Vector2Int pos in figureSO.form)
                                            {
                                                if (i + pos.x >= _leftGridConstrain && i + pos.x < _rightGridConstrain &&
                                                    j + offsetJ - pos.y >= 0 && j + offsetJ - pos.y <= _figureStartPos.y)
                                                {
                                                    if (_gameSpace.figureGrid[i + pos.x, j + offsetJ - pos.y] != null)
                                                    {
                                                        spaceCheck = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    spaceCheck = false;
                                                    break;
                                                }
                                            }

                                            if (spaceCheck)
                                            {
                                                foreach (Vector2Int pos in figureSO.form)
                                                {
                                                    if (i + pos.x >= _leftGridConstrain && i + pos.x < _rightGridConstrain)
                                                        if (_gameSpace.figureGrid[i + pos.x, j - 1] == null)
                                                        {
                                                            spaceCheck = false;
                                                            break;
                                                        }
                                                }

                                                if (spaceCheck)
                                                    _timedFigureSpawnPercent[figureSO.indexTex] = (_figureStartPos.y - j) * (_figureStartPos.y - j) *
                                                        Mathf.RoundToInt(figureSO.widthTex / figureSO.heightTex + 1);// * figureSO.form.Length;
                                            }
                                        }

                                        break;
                                    }
                                }
                                else 
                                {
                                    if (_gameSpace.figureGrid[i, j] == null)
                                    {
                                        foreach (FigureSO figureSO in _figureSOPrefabs)
                                        {
                                            bool spaceCheck = true;
                                            int offsetJ = Mathf.RoundToInt(figureSO.heightTex * 5);

                                            foreach (Vector2Int pos in figureSO.form)
                                            {
                                                if (i + pos.x >= _leftGridConstrain && i + pos.x < _rightGridConstrain &&
                                                    j + offsetJ - pos.y >= 0 && j + offsetJ - pos.y <= _figureStartPos.y)
                                                {
                                                    if (_gameSpace.figureGrid[i + pos.x, j + offsetJ - pos.y] != null)
                                                    {
                                                        spaceCheck = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    spaceCheck = false;
                                                    break;
                                                }
                                            }

                                            if (spaceCheck)
                                                _timedFigureSpawnPercent[figureSO.indexTex] += (_figureStartPos.y - j) * figureSO.form.Length;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                }
            }
        }

        if (_figureList.Count > 0 && _triplets)
        {
            Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
            Dictionary<Vector2Int, int> ways = new Dictionary<Vector2Int, int>();
            Vector2Int goal = _figureStartPos;
            goal.x += _gridXOffsetFromWorld;
            Vector2Int[] neighborsOffsets = new Vector2Int[3];
            neighborsOffsets[0] = Vector2Int.right;
            neighborsOffsets[1] = Vector2Int.left;
            neighborsOffsets[2] = Vector2Int.up;

            foreach (Figure figure in _figureList)
            {
                if (figure.IsGold || figure.IsSpoiled)
                    continue;

                Vector2Int fPos = figure.GetPosition();
                int gridX = fPos.x + _gridXOffsetFromWorld;
                int gridY = fPos.y;

                Vector2Int leftUpPos = new Vector2Int(Mathf.RoundToInt(gridX - figure.Width * 5),
                    Mathf.RoundToInt(gridY + figure.Height * 5));
                Vector2Int rightDownPos = new Vector2Int(Mathf.RoundToInt(gridX + figure.Width * 5),
                    Mathf.RoundToInt(gridY - figure.Height * 5));

                List<Vector2Int> probablyFinishPos = new List<Vector2Int>();

                for (int x = leftUpPos.x; x <= rightDownPos.x; x++)
                    for (int y = leftUpPos.y; y >= rightDownPos.y; y--)
                    {
                        if (x < _leftGridConstrain || x >= _rightGridConstrain
                            || y < 0 || y > goal.y)
                            continue;

                        bool checkPos = true;
                        bool checkTouch = false;

                        foreach (Vector2Int pos in figure.GetForm())
                        {
                            if (x + pos.x < _leftGridConstrain || x + pos.x >= _rightGridConstrain
                            || y - pos.y < 0 || y - pos.y > goal.y)
                            {
                                checkPos = false;
                                break;
                            }
                            else
                            {
                                if (_gameSpace.figureGrid[x + pos.x, y - pos.y] != null)
                                {
                                    checkPos = false;
                                    break;
                                }

                                if (x + pos.x - 1 >= _leftGridConstrain)
                                    if (_gameSpace.figureGrid[x + pos.x - 1, y - pos.y] == figure)
                                        checkTouch = true;

                                if (x + pos.x + 1 < _rightGridConstrain)
                                    if (_gameSpace.figureGrid[x + pos.x + 1, y - pos.y] == figure)
                                        checkTouch = true;

                                if (y - pos.y - 1 >= 0)
                                    if (_gameSpace.figureGrid[x + pos.x, y - pos.y - 1] == figure)
                                        checkTouch = true;

                                if (y - pos.y + 1 <= goal.y)
                                    if (_gameSpace.figureGrid[x + pos.x, y - pos.y + 1] == figure)
                                        checkTouch = true;
                            }
                        }

                        if (checkPos && checkTouch)
                        {
                            probablyFinishPos.Add(new Vector2Int(x, y));
                        }
                    }

                foreach (Vector2Int position in probablyFinishPos)
                {
                    costSoFar.Clear();
                    ways.Clear();

                    Vector2Int currentCell = position;

                    if (currentCell.x < _leftGridConstrain || currentCell.x == _rightGridConstrain
                        || currentCell.y > goal.y)
                        continue;


                    ways[currentCell] = 0;
                    costSoFar[currentCell] = 0;

                    bool checkWay = false;

                    while (ways.Count > 0)
                    {
                        //выбрать текущую
                        int prior = 10000;
                        foreach (Vector2Int cell in ways.Keys)
                            if (ways[cell] < prior)
                            {
                                prior = ways[cell];
                                currentCell = cell;
                            }


                        if (currentCell == goal)
                        {
                            _timedFigureSpawnPercent[figure.Index] += 10;
                            checkWay = true;
                            break;
                        }

                        foreach (Vector2Int offset in neighborsOffsets)
                        {
                            int newCost = costSoFar[currentCell] + 1;

                            Vector2Int nextCell = currentCell;
                            nextCell.x += offset.x;
                            nextCell.y += offset.y;

                            if (nextCell.x < _leftGridConstrain || nextCell.x == _rightGridConstrain
                                || nextCell.y > goal.y)
                                continue;

                            bool placeCheck = true;

                            foreach (Vector2Int pos in figure.GetForm())
                            {
                                if (nextCell.x + pos.x < _leftGridConstrain || nextCell.x + pos.x == _rightGridConstrain
                                    || nextCell.y - pos.y < 0 || nextCell.y - pos.y > goal.y)
                                {
                                    placeCheck = false;
                                    break;
                                }
                                else
                                {
                                    if (_gameSpace.figureGrid[nextCell.x + pos.x, nextCell.y - pos.y] != null)
                                    {
                                        if (_gameSpace.figureGrid[nextCell.x + pos.x, nextCell.y - pos.y] != figure)
                                        {
                                            placeCheck = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!placeCheck)
                                continue;

                            if (!costSoFar.ContainsKey(nextCell))
                            {
                                costSoFar[nextCell] = newCost;
                                int priority = newCost + heuristic(goal, nextCell);
                                ways[nextCell] = priority;
                            }
                            else if (newCost < costSoFar[nextCell])
                            {
                                costSoFar[nextCell] = newCost;
                                int priority = newCost + heuristic(goal, nextCell);
                                ways[nextCell] = priority;
                            }
                        }

                        ways.Remove(currentCell);
                    }

                    if (checkWay)
                        break;
                }
            }

        }
       
        int sum = 0;

        for (int i = 0; i < _figureSpawnPercent.Length; ++i)
        {
            sum += _figureSpawnPercent[i] + _timedFigureSpawnPercent[i];
        }

        int figureDefinitor = Random.Range(0, sum);

        int index = 0;
        sum = _figureSpawnPercent[index] + _timedFigureSpawnPercent[index];
        while (sum <= figureDefinitor)
        {
            index++;
            sum += _figureSpawnPercent[index] + _timedFigureSpawnPercent[index];
        }

        if (index == lastFigureIndex)
            index = (index + 1) % 25;

        return index;
    }

    private int heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    public void SetDoubleCost()
    {
        _doubleCost = true;
    }
    public void SetTrayBorders()
    {
        _trayBorders = true;
    }
    public void SetStickyTray()
    {
        _stickyTray = true;
    }
    public void SetTrayRotationSlow()
    {
        _trayAngle *= 0.5f;
    }

    public void SetTriplets()
    {
        _triplets = true;
    }

    public void SetSpoilerTriplets()
    {
        _spoilerTriplets = true;
    }

    public void SetGift(int giftNumber)
    {
        _doubleCost = false; // 5
        _trayBorders = false; // 4
        _triplets = false; // 7
        _stickyTray = false; // 10
        _spoilerTriplets = false; // 13
        _goldenFish = false; // 1
        _goldenTea = false; // 3
        _goldenSmallFood = false; // 2
        if (_traySlowMo) // 8 - traySlowMo
        {
            _traySlowMo = false;
            _trayAngle *= 2.0f;
        }

        switch(giftNumber)
        {
            case 1: _goldenFish = true; break;
            case 3: _goldenTea = true; break;
            case 2: _goldenSmallFood = true; break;
            case 4: _trayBorders = true; break;
            case 5: _doubleCost = true; break;
            case 7: _triplets = true; break;
            case 8:
                _traySlowMo = true;
                _trayAngle *= 0.5f;
                break;
            case 10: _stickyTray = true; break;
            case 13: _spoilerTriplets = true; break;
        }
    }

    public void SetGridWidth(int trayWidth) // TO DO: use Unity Event
    {
        _leftGridConstrain -= 1;
        _rightGridConstrain += 1;
        _trayNumber = Mathf.RoundToInt((trayWidth - 10) * 0.5f);
    }
    public void DropAllFigures()
    {
        foreach(Figure figure in _figureList)
        {
            figure.FlyAway(); ;
        }
        _figureList.Clear();
        _handControls.AddFigures(_figureList);

        ServiceLocator.Current.Get<AudioService>().PlayTetrisJingle();
    }
    public void ResetTetris()
    {
        for (int i = 0; i < _gameSpace.width; ++i)
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.figureGrid[i, j] = null;

        foreach (var figure in _figureList)
        {
            Destroy(figure.gameObject);
        }
        
        transform.parent.rotation = Quaternion.identity;

        _figureList.Clear();
        _flyingFigure = null;
        _movementTimer = 0;
        _figureListTimer = 0;
    }

    #region DEBUG

    private void OnDrawGizmos()
    {
        Vector2 pos = this.transform.localPosition;
        pos.x -= _gridXOffsetFromWorld;
        for (int i = 0; i <= _gameSpace.width; ++i)
        {
            Vector2 offset = new Vector2(i, 0);
            Gizmos.DrawLine(pos + offset, pos + offset + new Vector2(0, _gameSpace.height));
        }
        for (int j = 0; j <= _gameSpace.height; ++j)
        {
            Vector2 offset = new Vector2(0, j);
            Gizmos.DrawLine(pos + offset + new Vector2(_gameSpace.width, 0), pos + offset );
        }

        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
            {
                Gizmos.color = Color.gray;
                if (_gameSpace.figureGrid[i, j] != null)
                    Gizmos.color = Color.yellow;
                Gizmos.DrawCube(pos + new Vector2(i + 0.5f, j + 0.5f), Vector2.one * 0.3f);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pos + new Vector2(_figureStartPos.x + 0.5f, _figureStartPos.y + 0.5f), Vector2.one * 0.3f);
    }

    #endregion
}
