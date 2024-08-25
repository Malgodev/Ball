using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerRole 
{ 
    Goalkeeper,
    Midfielder,
    Fullback,
    Winger,
    Striker,
}

public enum EFormation
{
    Formation_3_3_2,
}

public class Formation
{
    public static List<EPlayerRole> Formation_3_3_2 = new List<EPlayerRole>
    {
        EPlayerRole.Goalkeeper,
        EPlayerRole.Fullback,
        EPlayerRole.Fullback,
        EPlayerRole.Fullback,
        EPlayerRole.Midfielder,
        EPlayerRole.Midfielder,
        EPlayerRole.Midfielder,
        EPlayerRole.Striker,
        EPlayerRole.Striker
    };
}


public struct PlayerInfo
{

}
