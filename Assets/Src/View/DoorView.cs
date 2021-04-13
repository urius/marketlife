using UnityEngine;

public class DoorView : AbstractWallView
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private GraphicsManager _graphicsManager;

    public void Awake()
    {
        _graphicsManager = GraphicsManager.Instance;
    }

    public void SetDoorId(int doorId)
    {
        _spriteRenderer.sprite = _graphicsManager.GetDoorSprite(doorId);
    }
}
