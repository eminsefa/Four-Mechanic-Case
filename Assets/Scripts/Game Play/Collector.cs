using UnityEngine;

public class Collector : MonoBehaviour
{
    private GameEngine _gameEngine;
    private SphereCollider _collectCollider;
    private void Start()
    {
        _gameEngine=GameEngine.Instance;
        _collectCollider = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        _collectCollider.radius += 0.005f;
        _gameEngine.FruitDestroyed();
    }
}
