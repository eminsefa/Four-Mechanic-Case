using UnityEngine;

public class Brick : MonoBehaviour
{
    private GameEngine _gameEngine;
    private Pad _pad;
    private Ball _ball;
    private Rigidbody _rb;
    public int BrickType { get; set; }
    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.localScale = Vector3.one * 1.25f;
        gameObject.layer = 15;
    }
    private void Start()
    {
        _pad=Pad.Instance;
        _ball=Ball.Instance;
        _gameEngine=GameEngine.Instance;
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (BrickType)
        {
            case 0:
                _ball.SpeedUp();
                break;
            case 1:
                _pad.IncreaseLength();
                break;
            case 2:
                _pad.ReverseControl();
                break;
            case 3:
                _ball.WobbleBall();
                break;
            case 4:
                _ball.IncreaseSize();
                break;
        }
        _gameEngine.FruitDestroyed();
        Destroy(gameObject);
    }
}
