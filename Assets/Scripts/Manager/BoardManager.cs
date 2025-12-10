using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Header("Objects: ")]
    public GameObject cellBrightPrefab;
    public GameObject cellDarkPrefab;
    public GameObject tilePrefab;

    [Header("Size: ")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    public int Width => _width;
    public int Height => _height;

    [Header("Array: ")]
    [SerializeField] private Sprite[] _tileSprites;
    private Tile[,] _tiles;
    private bool[,] _isBright;

    [Header("Action: ")]
    private Tile _selectedTile;
    private bool _isProcessing = false;
    public bool IsProcessing => _isProcessing;

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }
    private IEnumerator StartGameRoutine()
    {
        GenerateBoard();
        yield return StartCoroutine(ResolveMatchesLoop());

        yield return null;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.CenterCamera();
    }

    private void GenerateBoard()
    {
        _tiles = new Tile[_width, _height];
        _isBright = new bool[_width, _height];

        GameObject[,] cellMap = new GameObject[_width, _height];

        //Spawn BrightCell
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject cell = Instantiate(cellBrightPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.transform.parent = transform;

                cellMap[x, y] = cell;
                _isBright[x, y] = true;
            }
        }
        //Random DarkCell
        List<Vector2Int> allPos = new List<Vector2Int>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                allPos.Add(new Vector2Int(x, y));
            }
        }
        allPos = allPos.OrderBy(a => Random.value).ToList();

        int darkCount = Random.Range(15, 41);

        for (int i = 0; i < darkCount; i++)
        {
            Vector2Int p = allPos[i];

            Destroy(cellMap[p.x, p.y]);

            GameObject dark = Instantiate(cellDarkPrefab, new Vector3(p.x, p.y, 0), Quaternion.identity);
            dark.transform.parent = transform;

            cellMap[p.x, p.y] = dark;
            _isBright[p.x, p.y] = false;
        }
        //Spawn Tile lên ô Bright
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_isBright[x, y])
                {
                    SpawnTile(x, y);
                }
            }
        }
    }


    private void SpawnTile(int x, int y)
    {
        GameObject obj = Instantiate(tilePrefab);
        obj.transform.SetParent(transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = new Vector3(x, y, -1);

        Tile tile = obj.GetComponent<Tile>();

        int type;
        do
        {
            type = Random.Range(0, _tileSprites.Length);
        }
        while (WillCauseMatch(x, y, type));

        obj.GetComponent<SpriteRenderer>().sprite = _tileSprites[type];

        _tiles[x, y] = tile;
        tile.Initialize(x, y, type, this);
    }
    private bool WillCauseMatch(int x, int y, int type)
    {
        if (x >= 2)
        {
            Tile tile1 = _tiles[x - 1, y];
            Tile tile2 = _tiles[x - 2, y];
            if (tile1 != null && tile2 != null && tile1.type == type && tile2.type == type)
                return true;
        }
        if (y >= 2)
        {
            Tile type1 = _tiles[x, y - 1];
            Tile type2 = _tiles[x, y - 2];
            if (type1 != null && type2 != null && type1.type == type && type2.type == type)
                return true;
        }
        return false;
    }
    //Select type & swap type in Board
    public void SelectedTile(Tile tile)
    {
        if (_isProcessing) return;

        if (_selectedTile == null)
        {
            _selectedTile = tile;
            return;
        }

        if (IsAdjacent(_selectedTile, tile))
        {
            StartCoroutine(SwapTiles(_selectedTile, tile));
        }
    }
    private bool IsAdjacent(Tile tile1, Tile tile2)
    {
        return (Mathf.Abs(tile1.x - tile2.x) == 1 && tile1.y == tile2.y) ||
            (Mathf.Abs(tile1.y - tile2.y) == 1 && tile1.x == tile2.x);
    }
    //Swap animation & Check-match
    private IEnumerator SwapTiles(Tile tile1, Tile tile2)
    {
        _isProcessing = true;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.swapTileClip);

        int x1 = tile1.x;
        int y1 = tile1.y;
        int x2 = tile2.x;
        int y2 = tile2.y;

        Vector3 pos1 = tile1.transform.position;
        Vector3 pos2 = tile2.transform.position;

        float duration = 0.2f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            tile1.transform.position = Vector3.Lerp(pos1, pos2, k);
            tile2.transform.position = Vector3.Lerp(pos2, pos1, k);
            yield return null;
        }

        _tiles[x1, y1] = tile2;
        _tiles[x2, y2] = tile1;

        tile1.x = x2; tile1.y = y2;
        tile2.x = x1; tile2.y = y1;

        var matches = Findmatches();

        if (matches.Count == 0)
        {
            t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                tile1.transform.position = Vector3.Lerp(pos2, pos1, k);
                tile2.transform.position = Vector3.Lerp(pos1, pos2, k);
                yield return null;
            }

            _tiles[x1, y1] = tile1;
            _tiles[x2, y2] = tile2;

            tile1.x = x1; tile1.y = y1;
            tile2.x = x2; tile2.y = y2;

            _selectedTile = null;
            _isProcessing = false;
            yield break;
        }

        yield return StartCoroutine(ResolveMatchesLoop());

        _selectedTile = null;
        _isProcessing = false;
    }


    private List<Tile> Findmatches()
    {
        List<Tile> result = new List<Tile>();

        //Check ngang 
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width - 2; x++)
            {
                Tile a = _tiles[x, y];
                Tile b = _tiles[x + 1, y];
                Tile c = _tiles[x + 2, y];

                if (a == null || b == null || c == null) continue;

                if (a.type == b.type && a.type == c.type)
                {
                    result.Add(a);
                    result.Add(b);
                    result.Add(c);
                }
            }
        }

        //Check dọc
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height - 2; y++)
            {
                Tile a = _tiles[x, y];
                Tile b = _tiles[x, y + 1];
                Tile c = _tiles[x, y + 2];

                if (a == null || b == null || c == null) continue;

                if (a.type == b.type && a.type == c.type)
                {
                    result.Add(a);
                    result.Add(b);
                    result.Add(c);
                }
            }
        }

        return result.Distinct().ToList();
    }

    private IEnumerator ClearMatches(List<Tile> matches)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.completeTileClip);
        int matchCount = matches.Count;

        ScoreManager.Instance.AddScore(matchCount);

        foreach (var tile in matches)
        {
            _tiles[tile.x, tile.y] = null;

            SpriteRenderer sprite = tile.GetComponent<SpriteRenderer>();

            float t = 0;
            Color color = sprite.color;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                sprite.color = new Color(color.r, color.g, color.b, 1 - t * 5);
                yield return null;
            }
            Destroy(tile.gameObject);
        }
    }
    private IEnumerator ResolveMatchesLoop()
    {
        yield return null;

        List<Tile> matches = Findmatches();
        while (matches.Count > 0)
        {
            yield return StartCoroutine(ClearMatches(matches));
            yield return StartCoroutine(FallDown());
            yield return StartCoroutine(Refill());

            yield return null;
            matches = Findmatches();
        }
    }
    private IEnumerator FallDown()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y] == null) continue;
                Tile tile = _tiles[x, y];

                int ny = y;

                while (ny > 0)
                {
                    if (!_isBright[x, ny - 1])
                        break;

                    if (_tiles[x, ny - 1] != null)
                        break;

                    ny--;
                }

                if (ny == y) continue;

                _tiles[x, y] = null;
                _tiles[x, ny] = tile;
                tile.y = ny;

                Vector3 start = tile.transform.localPosition;
                Vector3 target = new Vector3(x, ny, -1);

                float t = 0;
                while (t < 0.25f)
                {
                    t += Time.deltaTime;
                    tile.transform.localPosition = Vector3.Lerp(start, target, t * 4);
                    yield return null;
                }
            }
        }
    }
    private IEnumerator Refill()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (!_isBright[x, y]) continue;
                if (_tiles[x, y] != null) continue;

                int targetY = y;

                GameObject obj = Instantiate(tilePrefab);
                obj.transform.SetParent(transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(x, _height + 2, -1);

                Tile tile = obj.GetComponent<Tile>();

                int type = Random.Range(0, _tileSprites.Length);
                obj.GetComponent<SpriteRenderer>().sprite = _tileSprites[type];

                tile.Initialize(x, targetY, type, this);
                _tiles[x, targetY] = tile;

                Vector3 start = tile.transform.localPosition;
                Vector3 target = new Vector3(x, targetY, -1);

                float t = 0;
                while (t < 0.25f)
                {
                    t += Time.deltaTime;
                    tile.transform.localPosition = Vector3.Lerp(start, target, t * 4);
                    yield return null;
                }
            }
        }
    }
    //Anti stuck
    public bool HasMove()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = _tiles[x,y];
                if (tile == null) continue;

                //Swap tile bên phải
                if (x < _width - 1)
                {
                    Tile right = _tiles[x + 1, y];

                    if (right != null && tile.type != right.type)
                    {
                        SwapData(tile, right);

                        bool match = CheckMatchAt(tile) || CheckMatchAt(right);

                        SwapData(tile, right);

                        if (match) return true;
                    }
                }
                //Swap tile bên trên
                if (y < _height - 1)
                {
                    Tile up = _tiles[x, y + 1];

                    if (up != null && tile.type != up.type)
                    {
                        SwapData(tile, up);

                        bool match = CheckMatchAt(tile) || CheckMatchAt(up);

                        SwapData(tile, up);

                        if(match) return true;
                    }
                }
            }
        }
        return false;
    }
    private void SwapData(Tile a, Tile b)
    {
        _tiles[a.x, a.y] = b;
        _tiles[b.x, b.y] = a;

        int ax = a.x;
        int ay = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = ax;
        b.y = ay;
    }

    //Check tile có nằm  trong chuỗi match-3 ko.
    private bool CheckMatchAt(Tile t)
    {
        int x = t.x;
        int y = t.y;
        int type = t.type;

        int count = 1;

        // --- Check hàng ngang ---
        int left = x - 1;
        while (left >= 0 && _tiles[left, y] != null && _tiles[left, y].type == type)
        {
            count++;
            left--;
        }

        int right = x + 1;
        while (right < _width && _tiles[right, y] != null && _tiles[right, y].type == type)
        {
            count++;
            right++;
        }

        if (count >= 3)
            return true;

        // --- Check hàng dọc ---
        count = 1;

        int down = y - 1;
        while (down >= 0 && _tiles[x, down] != null && _tiles[x, down].type == type)
        {
            count++;
            down--;
        }

        int up = y + 1;
        while (up < _height && _tiles[x, up] != null && _tiles[x, up].type == type)
        {
            count++;
            up++;
        }

        if (count >= 3)
            return true;

        return false;
    }
    //Drag
    public void OnTileDragged(Tile tile, int dx, int dy)
    {
        int nx = tile.x + dx;
        int ny = tile.y + dy;

        if (nx < 0 || nx >= _width || ny < 0 || ny >= _height) return;

        Tile target = _tiles[nx, ny];
        if (target == null) return;

        StartCoroutine(SwapTiles(tile, target));
    }
}
