using System;

namespace eShopOnContainers.Core.Models.Location
{
    public class PositionEventArgs : EventArgs
    {
        public Position Position { get; private set; }

        public PositionEventArgs(Position position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            Position = position;
        }
    }
}
