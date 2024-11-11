using System;
using Unity.Collections;
using Unity.Netcode;

public struct UserData : IEquatable<UserData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes PlayerName;

    public bool Equals(UserData other)
    {
        return clientId == other.clientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref PlayerName);
    }
}
