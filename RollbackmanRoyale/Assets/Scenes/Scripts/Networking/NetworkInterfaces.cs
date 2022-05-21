using System.Collections;

using UnityEngine;
using Unity.Collections;

namespace Sinistral.GGPO
{
    public class Connections
    {
        public ushort port;
        public string ip;
        public bool spectator;
    }

    public interface IGame
    {
        int Framenumber { get; }
        int Checksum { get; }

        void Update(long[] inputs, int disconnectFlags);
        void FromBytes(NativeArray<byte> data);
        NativeArray<byte> ToByptes();
        long ReadInputs(int controllerId);
        void LogInfo(string filename);
        void FreeBytes(NativeArray<byte> data);
    }

    public interface IGameRunner
    {
        IGame Game { get; }
       
    }
}