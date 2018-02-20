using System;
using System.Threading;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Location;

namespace eShopOnContainers.Core.Services.Location
{
    public interface ILocationServiceImplementation
    {
        event EventHandler<PositionErrorEventArgs> PositionError;
        event EventHandler<PositionEventArgs> PositionChanged;

        double DesiredAccuracy { get; set; }
        bool IsGeolocationAvailable { get; }
        bool IsGeolocationEnabled { get; }

        Task<Position> GetPositionAsync(TimeSpan? timeout = null, CancellationToken? token = null, bool includeHeading = false);
    }
}
