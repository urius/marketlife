using Src.Common;
using Src.View.Gameplay;
using UnityEngine;

namespace Src.View
{
    public struct ViewsFactory
    {
        public SpriteRenderer CreateFloor(Transform parentTransform, int numericId)
        {
            var sprite = SpritesProvider.Instance.GetFloorSprite(numericId);
            return CreateFloorView(parentTransform, sprite);
        }

        public SpriteRenderer CreateWall(Transform parentTransform, int numericId)
        {
            var wallGo = GameObject.Instantiate(PrefabsHolder.Instance.WallPrefab, parentTransform);
            var result = wallGo.GetComponent<SpriteRenderer>();
            result.sprite = SpritesProvider.Instance.GetWallSprite(numericId);
            return result;
        }

        public (Transform transform, SpriteRenderer sprite) CreateWindow(Transform parentTransform, int numericId)
        {
            var windowGo = GameObject.Instantiate(PrefabsHolder.Instance.WindowPrefab, parentTransform);
            var spriteRenderer = windowGo.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = SpritesProvider.Instance.GetWindowSprite(numericId);
            return (windowGo.transform, spriteRenderer);
        }

        public SpriteRenderer CreateSpriteRenderer(Transform parentTransform, Sprite sprite, string name = null)
        {
            var go = name != null ? new GameObject(name) : new GameObject();
            go.transform.SetParent(parentTransform);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            return spriteRenderer;
        }

        public DoorView CreateDoor(Transform parentTransform, int numericId)
        {
            var doorGo = GameObject.Instantiate(PrefabsHolder.Instance.DoorPrefab, parentTransform);
            var doorlView = doorGo.GetComponent<DoorView>();
            doorlView.SetDoorId(numericId);
            return doorlView;
        }

        public SpriteRenderer CreateFloorView(Transform parentTransform, Sprite sprite)
        {
            var floorGo = GameObject.Instantiate(PrefabsHolder.Instance.FloorPrefab, parentTransform);
            var result = floorGo.GetComponent<SpriteRenderer>();
            result.sprite = sprite;
            return result;
        }
    }
}


