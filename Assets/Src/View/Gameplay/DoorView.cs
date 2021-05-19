using UnityEngine;

public class DoorView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private SpritesProvider _spritesProvider;

    public void Awake()
    {
        _spritesProvider = SpritesProvider.Instance;
    }

    public void SetDoorId(int doorId)
    {
        _spriteRenderer.sprite = _spritesProvider.GetDoorSprite(doorId);
    }
}
