using UnityEngine;

public class BGMoving : MonoBehaviour
{
    [SerializeField] private float _moveDistance = 0.5f;
    [SerializeField] private float _speed = 1f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float x = Mathf.Sin(Time.time * _speed) * _moveDistance;
        transform.localPosition = startPos + new Vector3(x, 0, 0);
    }
}
