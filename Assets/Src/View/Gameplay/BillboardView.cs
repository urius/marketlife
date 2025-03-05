using Src.Common_Utils;
using TMPro;
using UnityEngine;

namespace Src.View.Gameplay
{
    public class BillboardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private SpriteRenderer[] _highlightableSprites;
        [SerializeField] private Transform[] _boundPointTransforms;

        public Transform[] BoundPointTransforms => _boundPointTransforms;

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
}
