using UnityEngine;

public class WallView : AbstractWallView
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _windowSpriteRenderer;

    private GraphicsManager _graphicsManager;

    public void Awake()
    {
        _graphicsManager = GraphicsManager.Instance;
    }

    public void SetWallId(int wallId)
    {
        _spriteRenderer.sprite = _graphicsManager.GetWallSprite(wallId);
    }

    public void SetWindowId(int windowId)
    {
        _windowSpriteRenderer.gameObject.SetActive(true);
        _windowSpriteRenderer.sprite = _graphicsManager.GetWindowSprite(windowId);
    }

    public void RemoveWindow()
    {
        _windowSpriteRenderer.sprite = null;
        _windowSpriteRenderer.gameObject.SetActive(false);
    }
}
