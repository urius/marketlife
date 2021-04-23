using UnityEngine;

public class WallView : AbstractWallView
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _windowSpriteRenderer;

    private SpritesProvider _spritesProvider;

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
