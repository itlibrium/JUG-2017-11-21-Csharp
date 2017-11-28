using System.Collections.Generic;

namespace JUG.Domain
{
    public struct ServiceAction
    {
        public ServiceActionType Type { get; }
        public Duration Duration { get; }
        public IReadOnlyList<int> SparePartIds { get; }

        public ServiceAction(ServiceActionType type, Duration duration, IReadOnlyList<int> sparePartIds)
        {
            Type = type;
            Duration = duration;
            SparePartIds = sparePartIds;
        }
    }
}