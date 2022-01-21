using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public static Ball Instance;
    private UIManager _uiManager;
    private Rigidbody _rb;
    private bool _wobbling;
    private int _hitBorder;
    [SerializeField] private float moveSpeed;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _uiManager=UIManager.Instance;
        _uiManager.LevelPlaying += LevelPlaying;
    }

    private void FixedUpdate()
    {
        //Is this wobbling?
        if(!_wobbling) return;
        var randV = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        var randF = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        _rb.AddForceAtPosition(randF * moveSpeed * Time.fixedDeltaTime,transform.position+randV);
    }
    private void OnDisable()
    {
        if(_uiManager) _uiManager.LevelPlaying -= LevelPlaying;
    }
    
    public void SpeedUp()
    {
        moveSpeed += 50f;
    }
    public void WobbleBall()
    {
        _wobbling = true;
    }
    public void IncreaseSize()
    {
        var scale = transform.localScale;
        scale *= 1.15f;
        transform.localScale = scale;
    }

    private void OnCollisionEnter(Collision other)
    {
        var dir = (_rb.transform.position - other.GetContact(0).point).normalized;
        var randomV = Vector3.zero;
        if (other.transform.gameObject.layer == 13)
        {
            //To prevent stuck position
            _hitBorder++;
            randomV = new Vector3(0, 0, Random.Range(0, 0.05f)*_hitBorder);
        }
        else _hitBorder = 0;
        AddForce(dir+randomV);
    }

    private void OnTriggerEnter(Collider other)
    {
        LevelManager.RestartLevel();
    }

    public void AddForce(Vector3 dir)
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(dir.normalized * moveSpeed);
    }
    private void LevelPlaying(bool playing)
    {
        if (playing) _rb.isKinematic = false;
        else _rb.isKinematic = true;
    }
}
