using UnityEngine;

public class CashDeskView : ShopObjectViewBase
{
    [SerializeField]
    private SpriteRenderer _hatSprite;

    public void SetHatSprite(Sprite hatSprite)
    {
        _hatSprite.sprite = hatSprite;
        _hatSprite.gameObject.SetActive(hatSprite != null);
    }
}
