using UnityEngine;

public class FruitVisualiser : MonoBehaviour
{
    [SerializeField] private bool isForInventory;
    
    private void OnEnable()
    {
        if(!isForInventory) Destroy(GetComponent<Rigidbody>());
    }
    void Update()
    {
        if(!isForInventory)
        {
            var pos = transform.position;
            pos.y += 2 * Time.deltaTime;
            if (pos.y < 5) transform.position = pos;
        }
        var rot = transform.rotation;
        rot = Quaternion.AngleAxis(2, Vector3.up) * rot;
        transform.rotation = rot;
    }
}
