using UnityEngine;

public class SlideMergeable : MonoBehaviour
{
    /// <summary>
    /// Movement and merging are being checked with grid system.
    /// This script moves fruit and asks game engine to check for merge.
    /// If merge succeeds fruit gets the new grid position.
    /// If merge fails it returns to old grid position.
    /// If fruit falls to below, it checks for merge with the same system.
    /// </summary>
    private GameEngine _gameEngine;
    public bool IsMoving { get; private set; }
    private bool _slided;
    private int _lastGridNumber;
    private int _nextGridNumber;
    private Vector3 _lastPos;
    private Vector3 _nextPos;
    public (int gridNumber, int fruitNumber) GridDetail { get; set; }
    private void OnEnable()
    {
        _gameEngine=GameEngine.Instance;
        gameObject.layer=14;
        transform.localScale=Vector3.one*1.25f;
        Destroy(GetComponent<Rigidbody>());
    }
    private void Update()
    {
        if (IsMoving)
        {
            var dif = Vector3.Distance(transform.position, _nextPos);
            transform.position = Vector3.MoveTowards(transform.position, _nextPos, 5 * Time.deltaTime);
            if (dif < 0.01f)
            {
                IsMoving = false;
                transform.position = _nextPos;
                var gd = GridDetail;
                gd.gridNumber = _nextGridNumber;
                GridDetail = gd;
                _gameEngine.GridFruitDictionary[_nextGridNumber] = GridDetail.fruitNumber;
                _gameEngine.GridSmDictionary[_nextGridNumber] = this;
                _gameEngine.StartCoroutine(_gameEngine.CheckToMerge(this));
            }
        }
    }

    public void CheckToMove(Vector3 inputDelta)
    {
        var hit = new RaycastHit();
        if (Mathf.Abs(inputDelta.x) >= Mathf.Abs(inputDelta.y))
        {
            int i = inputDelta.x > 0 ? 1 : -1;
            if (!Physics.Raycast(transform.position+Vector3.right * i+Vector3.up, Vector3.down, out hit, 2f)) return;
        }
        else
        {
            int i = inputDelta.y > 0 ? 1 : -1;
            if(!Physics.Raycast(transform.position+Vector3.forward*i+Vector3.up,Vector3.down,out hit,2f)) return;
        }

        if(hit.transform.TryGetComponent(out SlideMergeable s))
        {
            s.Slide(this);
            Slide(s);
        }
    }

    public void Slide(SlideMergeable nextSlideMergeable)
    {
        _slided = true;
        _lastGridNumber = GridDetail.gridNumber;
        _lastPos = transform.position;
        _nextGridNumber = nextSlideMergeable.GridDetail.gridNumber;
        _nextPos = nextSlideMergeable.transform.position;
        IsMoving = true;
    }

    public void SlideBack()
    {
        if (_gameEngine.GridSmDictionary[_lastGridNumber])
        {
            if(!_slided) return;
            _nextPos = _lastPos;
            _nextGridNumber = _lastGridNumber;
            IsMoving = true;
            _slided = false;
        }
        else
        {
            _lastPos = transform.position;
            _slided = false;
        }
    }
    public void FillSpace(Vector3 pos)
    {
        _nextGridNumber = GridDetail.gridNumber;
        _nextPos = _lastPos = pos;
        IsMoving = true;
    }
}
