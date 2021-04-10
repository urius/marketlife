using UnityEngine;

public class WallView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private GraphicsManager _graphicsManager;

    public void Awake()
    {
        _graphicsManager = GraphicsManager.Instance;
    }

    public void ToRightState()
    {
        var euler = transform.eulerAngles;
        euler.x = 45;
        transform.eulerAngles = euler;
    }

    public void ToLeftState()
    {
        var euler = transform.eulerAngles;
        euler.x = 315;
        transform.eulerAngles = euler;
    }

    public void SetWallId(int wallId)
    {
        _spriteRenderer.sprite = _graphicsManager.GetWallSprite(wallId);
    }
}
