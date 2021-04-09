using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHumanMediator
{
    private readonly HumanView _humanView;

    public DebugHumanMediator(GameObject humanGO)
    {
        _humanView = humanGO.GetComponent<HumanView>();

        Activate();
    }

    private void Activate()
    {
        DebugDispatcher.Instance.ShowHumanSide += OnRequestShowSide;
        DebugDispatcher.Instance.ShowHumanAnimation += OnRequestShowHumanAnimation;
        DebugDispatcher.Instance.ShowFaceAnimation += OnRequestShowFaceAnimation;
        DebugDispatcher.Instance.ShowGlasses += OnRequestShowGlasses;
        DebugDispatcher.Instance.ShowHair += OnRequestShowHair;
        DebugDispatcher.Instance.ShowTopClothes += OnRequestTopClothes;
        DebugDispatcher.Instance.ShowBottomClothes += OnRequestBottomClothes;
    }

    private void OnRequestShowFaceAnimation(int animationIndex)
    {
        _humanView.ShowFaceAnimation(animationIndex);
    }

    private void OnRequestShowGlasses(int glassesId)
    {
        _humanView.SetGlasses(glassesId);
    }

    private void OnRequestShowHumanAnimation(int animationIndex)
    {
        _humanView.SetBodyState((BodyState)animationIndex);
    }

    private void OnRequestShowSide(int sideId)
    {
        _humanView.ShowSide(sideId);
    }

    private void OnRequestShowHair(int hairNum)
    {
        _humanView.SetHair(hairNum);
    }

    private void OnRequestTopClothes(int clothesNum)
    {
        _humanView.SetBodyClothes(clothesNum);
    }

    private void OnRequestBottomClothes(int clothesNum)
    {
        _humanView.SetFootClothes(clothesNum);
    }
}
