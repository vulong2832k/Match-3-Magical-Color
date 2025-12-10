using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int type;
    public BoardManager board;

    private Vector3 _mouseDownScreenPos;
    private bool _dragging = false;

    [SerializeField] private bool enableDragPreview = false;
    private Vector3 _startLocalPos;

    public void Initialize(int x, int y, int type, BoardManager board)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.board = board;
    }

    private void OnMouseDown()
    {
        if (board == null || board.IsProcessing) return;

        _mouseDownScreenPos = Input.mousePosition;
        _dragging = true;

        if (enableDragPreview)
        {
            _startLocalPos = transform.localPosition;
        }
    }

    private void OnMouseUp()
    {
        _dragging = false;

        if (enableDragPreview)
        {
            transform.localPosition = _startLocalPos;
        }
    }

    private void OnMouseDrag()
    {
        if (!_dragging) return;
        if (board == null || board.IsProcessing) return;

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 delta = mouseScreen - _mouseDownScreenPos;

        if (delta.magnitude < 20f) return;

        int dx = 0;
        int dy = 0;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            dx = delta.x > 0 ? 1 : -1;
        }
        else
        {
            dy = delta.y > 0 ? 1 : -1;
        }

        board.OnTileDragged(this, dx, dy);

        _dragging = false;

        if (enableDragPreview)
            transform.localPosition = _startLocalPos;
    }

    private void Update()
    {
        if (!_dragging || !enableDragPreview || board == null || board.IsProcessing) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
        Vector3 target = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, 0.6f);
    }
}
