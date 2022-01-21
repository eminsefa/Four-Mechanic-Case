using UnityEngine;

public class Collectable : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private bool _collected;
    private Vector3 _collectorPoint;
    private Rigidbody _rb;
    private void OnEnable()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        transform.localScale=Vector3.one*1.25f;
        gameObject.layer = 9;
    }
    private void FixedUpdate()
    {
        if(!_collected) return;
        var dist = Vector3.Distance(_rb.position, _collectorPoint);
        var speed = Mathf.Lerp(100, 200, dist/0.5f);
        _rb.velocity = (_collectorPoint - _rb.position) * speed * Time.fixedDeltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.layer = 10;
        _meshRenderer.material.color = Color.yellow;
        _collectorPoint = other.transform.position;
        _collected = true;
        _rb.freezeRotation = true;
    }
}
