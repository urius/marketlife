using TMPro;
using UnityEngine;

public class BillboardView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Collider _collider;
    [SerializeField] private SpriteRenderer[] _highlightableSprites;

    public Collider Collider => _collider;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetIsHiglighted(bool isHighlighted)
    {
        foreach (var sprite in _highlightableSprites)
        {
            sprite.color = sprite.color.SetAlpha(isHighlighted ? 0.5f : 1);
        }
    }
}
