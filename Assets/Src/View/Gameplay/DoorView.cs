using UnityEngine;

public class DoorView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private SpritesProvider _spritesProvider;

    public Color Color { get { return _spriteRenderer.color; } set { _spriteRenderer.color = value; } }

    public void Awake()
    {
        _spritesProvider = SpritesProvider.Instance;
    }

    public void SetDoorId(int doorId)
    {
        _spriteRenderer.sprite = _spritesProvider.GetDoorSprite(doorId);
    }

    public void SetSortingLayerName(string sortingLayerName)
    {
        _spriteRenderer.sortingLayerName = sortingLayerName;
    }
}
