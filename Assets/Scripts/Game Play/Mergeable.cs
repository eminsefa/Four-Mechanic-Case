using UnityEngine;

public class Mergeable : MonoBehaviour
{
    /// <summary>
    /// This scripts checks for merge and destroy selfs if merge succeeds
    /// </summary>
    private GameEngine _gameEngine;
    public int ID { get; private set; }
    public int MergedCount { get; set; }
    private void OnEnable()
    {
        ID = gameObject.GetInstanceID();
        _gameEngine=GameEngine.Instance;
        if (TryGetComponent(out Pusher pusher))
        {
            MergedCount = 5;
        }
        else
        {
            var rb=GetComponent<Rigidbody>();
            rb.drag = 0;
            rb.freezeRotation = true;
            transform.localScale=Vector3.one*2f;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        var otherFruit = other.gameObject;
        if (otherFruit.TryGetComponent(out Mergeable mergeable))
        {
            if (MergedCount == mergeable.MergedCount)
            {
                if (ID < mergeable.ID)
                {
                    var middlePos=(transform.position+other.transform.position)/2f;
                    _gameEngine.FruitsMerged(middlePos,MergedCount);
                }
                Destroy(gameObject);
            }
        }
    }
}
