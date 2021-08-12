using System;
using UnityEngine;

public class TutorialArrowPointerView : MonoBehaviour
{
    [SerializeField] private Transform _arrowTransform;

    private Vector3 _arrowStartPosition;

    private void Awake()
    {
        _arrowStartPosition = _arrowTransform.position;
    }

    private void FixedUpdate()
    {
        var zOffset = Math.Sin(Time.frameCount * 0.2f) * 0.2f;
        _arrowTransform.LeanSetPosZ(_arrowStartPosition.z + (float)zOffset);
    }
}
