using UnityEngine;

public class Pusher : MonoBehaviour
{
    public static Pusher Instance;
    private InputManager _inputManager;
    private Rigidbody _rb;
    private Quaternion _lastRot;
    private bool _levelPlaying;
    [SerializeField] private float threshold;
    [SerializeField] private float speed;
    [SerializeField] private bool isBanana;
    [SerializeField] private LayerMask borderLayer;
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _inputManager=InputManager.Instance;
        _rb = GetComponent<Rigidbody>();
        _lastRot = transform.rotation;
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        _rb.velocity = Vector3.zero;
        var movePos = _inputManager.RayMovePos;
        var dir = movePos - _rb.position;
        dir.y = 0;
        if (dir.magnitude > threshold)
        {
            var lookRot = Quaternion.LookRotation(dir.normalized);
            if (isBanana) _lastRot = Quaternion.AngleAxis(90, dir.normalized) * lookRot;
            else _lastRot = lookRot;
        }

        var nextPos = Vector3.MoveTowards(_rb.position, movePos, speed * Time.fixedDeltaTime);
        if(!Physics.Raycast(nextPos+Vector3.up*2,Vector3.down,3f,borderLayer))
        {
            _rb.MovePosition(nextPos);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, _lastRot, speed * 2 * Time.fixedDeltaTime));
        }
    }
}
