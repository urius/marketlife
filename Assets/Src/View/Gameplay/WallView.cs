using UnityEngine;

public class WallView : AbstractWallView
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _windowSpriteRenderer;

    private SpritesProvider _spritesProvider;

    public Color WallColor { get { return _spriteRenderer.color; } set { _spriteRenderer.color = value; } }
    public Color WindowColor { get { return _windowSpriteRenderer.color; } set { _windowSpriteRenderer.color = value; } }

    public void Awake()
    {
        _spritesProvider = SpritesProvider.Instance;
    }

    public void SetWallId(int wallId)
    {
        _spriteRenderer.sprite = _spritesProvider.GetWallSprite(wallId);
    }

    public void SetWindowId(int windowId)
    {
        _windowSpriteRenderer.gameObject.SetActive(true);
        _windowSpriteRenderer.sprite = _spritesProvider.GetWindowSprite(windowId);
    }

    public void RemoveWindow()
    {
        _windowSpriteRenderer.sprite = null;
        _windowSpriteRenderer.gameObject.SetActive(false);
    }
}
