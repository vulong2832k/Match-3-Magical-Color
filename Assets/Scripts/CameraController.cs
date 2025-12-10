using UnityEngine;

public class CameraController : MonoBehaviour
{
    public BoardManager _board;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        CenterCamera();
    }

    public void CenterCamera()
    {
        int width = _board.Width;
        int height = _board.Height;

        float tileSize = 1f;

        float boardWidth = width * tileSize;
        float boardHeight = height * tileSize;

        Vector3 center = new Vector3(boardWidth / 2f - 0.5f, boardHeight / 2f - 0.5f, -10f);

        transform.position = center;

        float halfW = boardWidth / 2f;
        float halfH = boardHeight / 2f;

        float sizeFromWidth = halfW / _cam.aspect;
        float sizeFromHeight = halfH;

        _cam.orthographicSize = Mathf.Max(sizeFromWidth, sizeFromHeight) + 3f;
    }
}
