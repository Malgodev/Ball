using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerInfo : IEquatable<UserData>, INetworkSerializable
{
    public string PlayerName;
    public EPlayerRole Role;
    public Vector2 Offset;

    public bool Equals(UserData other)
    {
        return PlayerName == other.PlayerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Role);
        serializer.SerializeValue(ref Offset);
    }

    //public PlayerInfo(EPlayerRole role, Vector2 offset)
    //{
    //    Role = role;
    //    Offset = offset;
    //}

    public override string ToString()
    {
        return $"PlayerName {PlayerName} Role {Role} Offset {Offset}";
    }
}


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