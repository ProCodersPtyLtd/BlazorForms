using BlazorForms.Shared;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace BlazorForms.Platform.ProcessFlow
{
    public class UniqueIdGenerator : IUniqueIdGenerator
    {
        private static readonly int TOTAL_BITS = 32;
        private static readonly int EPOCH_BITS = 10;
        private static readonly int NODE_ID_BITS = 8;
        private static readonly int SEQUENCE_BITS = 14;

        private static readonly uint maxNodeId = (uint)(Math.Pow(2, NODE_ID_BITS) - 1);
        private static readonly uint maxEpoch = (uint)(Math.Pow(2, EPOCH_BITS) - 1);
        private static readonly uint maxSequence = (uint)(Math.Pow(2, SEQUENCE_BITS) - 1);

        private long _lastTimestamp = -1L;
        private volatile uint _sequence = 0;

        private readonly uint _nodeId;
        private ILogStreamer _logStreamer;

        public UniqueIdGenerator(ILogStreamer logStreamer)
        {
            _nodeId = GetThisNodeId() & maxNodeId;
            _logStreamer = logStreamer;
        }

        private uint GetThisNodeId()
        {
            var nodeId = NetworkInterface.GetAllNetworkInterfaces()
                .Select(i => i.GetPhysicalAddress().GetHashCode())
                .Aggregate((a, b) => HashCode.Combine(a, b));

            return (uint)nodeId;
        }

        public int NextId()
        {
            long currentTimestamp = DateTime.Now.Ticks;
            long lastTimestamp = Interlocked.Exchange(ref _lastTimestamp, currentTimestamp);

            if (currentTimestamp < lastTimestamp)
            {
                _logStreamer.TrackException(new Exception("Invalid System Clock!"));
                throw new Exception("Invalid System Clock!");
            }

            _sequence = (currentTimestamp == lastTimestamp) ? ++_sequence : 0;

            if (_sequence >= maxSequence)
            {
                // Sequence Exhausted, wait till next millisecond.
                // currentTimestamp = waitNextMillis(currentTimestamp);
                _logStreamer.TrackException(new Exception("Sequence Exhausted"));
                throw new Exception("Sequence Exhausted");
            }

            uint id = _nodeId << (TOTAL_BITS - NODE_ID_BITS);
            id |= (uint)(currentTimestamp & maxEpoch) << (TOTAL_BITS - EPOCH_BITS - EPOCH_BITS);
            id |= (_sequence & maxSequence);
            return (int)id;
        }
    }
}
