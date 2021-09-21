using System;
using UnityEngine;

public class OldGameCompensationHolder
{
    public static readonly OldGameCompensationHolder Instance = new OldGameCompensationHolder();

    public event Action CompensationWasSetup = delegate { };

    static OldGameCompensationHolder()
    {
        Debug.Log("test static OldGameCompensationHolder");
    }

    public OldGameCompensationHolder()
    {
        Debug.Log("test OldGameCompensationHolder");
    }

    public OldGameCompensation Compensation { get; private set; }
    public bool CompensationIsSet { get; private set; } = false;

    public void SetupCompensation(OldGameCompensation compensation)
    {
        Compensation = compensation;
        CompensationIsSet = true;
        CompensationWasSetup();
    }
}

public struct OldGameCompensation
{
    public int AmountGold;
    public int AmountCash;
}
