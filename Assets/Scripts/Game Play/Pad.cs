using UnityEngine;

public class Pad : MonoBehaviour
{
    public static Pad Instance;
    private InputManager _inputManager;
    private Ball _ball;
    private Rigidbody _rb;
    private float _localScaleX;
    private bool _ballLaunched;
    private bool _controlReversed;
    [SerializeField] private float sensivity;
    [SerializeField] private LayerMask borderLayer;
    [SerializeField] private bool autoPlay;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputManager=InputManager.Instance;
        _ball=Ball.Instance;
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        var deltaX = _inputManager.InputDelta.x;
        if(!_ballLaunched && deltaX>0) LaunchBall();
        if(autoPlay)
        {
            var p = transform.position;
            p.x = _ball.transform.position.x;
            transform.position = p;
            return;
        }
        _localScaleX = transform.localScale.x / 3.5f;
        var pos = transform.position;
        var i = deltaX == 0 ? 0 : deltaX > 0 ? _localScaleX : -_localScaleX;
        if (_controlReversed) i *= -1;
        pos.x += i * sensivity * Time.fixedDeltaTime;
        if(!Physics.Raycast(pos + Vector3.right * i + Vector3.up*3f,Vector3.down,4f,borderLayer))
        {
            _rb.MovePosition(pos);
        }
    }
    private void LaunchBall()
    {
        _ballLaunched = true;
        var randomDir = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(5f, 10f));
        _ball.AddForce(randomDir);
    }
    public void IncreaseLength()
    {
        var scale = transform.localScale;
        scale.x += 0.25f;
        transform.localScale = scale;
    }
    public void ReverseControl()
    {
        if (_controlReversed) _controlReversed = false;
        else _controlReversed = true;
    }
}
