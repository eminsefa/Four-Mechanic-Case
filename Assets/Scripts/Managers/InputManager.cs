using UnityEngine;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// This script provides input for all levels with both mouse and touch input.
    /// </summary>
    public static InputManager Instance;
    private UIManager _uiManager;
    private Camera _mainCam;
    private Vector3 _lastInput;
    private Vector3 _screenPos;
    private bool _levelPlaying;
    private int _currentLevel;
    public Vector3 InputDelta { get; private set; }
    public Vector3 WorldMovePos { get; private set; }
    public Vector3 RayMovePos { get; private set; }
    public SlideMergeable SlideMergeable { get; set; }
    [SerializeField] private float deltaThreshold;
    [SerializeField] private LayerMask moveAreaLayer;
    [SerializeField] private LayerMask slideMergeableLayer;
    private void Awake()
    {
        Instance = this;
        _mainCam=Camera.main;
        _currentLevel = LevelManager.GetLevelIndex();
    }

    private void Start()
    {
        _uiManager=UIManager.Instance;
        _uiManager.LevelPlaying += LevelPlaying;
        var pusher = Pusher.Instance;
        if(pusher) RayMovePos = pusher.transform.position;
    }
    private void OnDisable()
    {
        if (_uiManager) _uiManager.LevelPlaying -= LevelPlaying;
    }
    private void Update()
    {
        if(!_levelPlaying) return;
        var hasInput = false;
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
            {
                _screenPos = Input.mousePosition;
                hasInput = true;
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                _screenPos = touch.position;
                hasInput = true;
            }
        }
        
        //This complexity provides level 3 for both swipe and tap input. And also works for level 4 input 
        var dif =_screenPos-_lastInput;
        if (dif.magnitude > deltaThreshold)
        {
            InputDelta = dif;
            if (!hasInput && _currentLevel == 3)
            {
                if (SlideMergeable) SlideMergeable.CheckToMove(InputDelta);
                SlideMergeable = null;
                return;
            }
            if (_currentLevel == 4)
            {
                _lastInput = _screenPos;
            }
        }
        if (!hasInput && _currentLevel!=3)
        {
            _lastInput = Vector3.zero;
            _screenPos = Vector3.zero;
            InputDelta=Vector3.zero;
        }
        if (_currentLevel == 3)
        {
            if(!SlideMergeable)
            {
                var ray = _mainCam.ScreenPointToRay(_screenPos);
                if (Physics.Raycast(ray, out var hit, 100f, slideMergeableLayer))
                {
                    SlideMergeable = hit.transform.GetComponent<SlideMergeable>();
                    _lastInput = _screenPos;
                }
            }
        }
        else if(hasInput)
        {
            var ray = _mainCam.ScreenPointToRay(_screenPos);
            if (Physics.Raycast(ray, out var hit, 100f, moveAreaLayer))
            {
                RayMovePos = hit.point;
            }
        }
       
    }
    private void LevelPlaying(bool playing)
    {
        _levelPlaying = playing;
    }
}
