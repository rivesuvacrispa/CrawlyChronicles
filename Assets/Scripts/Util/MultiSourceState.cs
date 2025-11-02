using System;
using System.Collections.Generic;

namespace Util
{
    public class MultiSourceState
    {
        private readonly Action stateOn;
        private readonly Action stateOff;
        private readonly HashSet<int> voteSources = new();

        public bool State => voteSources.Count > 0;

        public void Vote(int sourceId)
        {
            if (voteSources.Contains(sourceId)) return;
            bool wasInactive = voteSources.Count == 0;
            voteSources.Add(sourceId);
            if (wasInactive)
                stateOn?.Invoke();
        }

        public void Unvote(int sourceId)
        {
            if (!voteSources.Contains(sourceId)) return;
            voteSources.Remove(sourceId);
            if (voteSources.Count == 0)
                stateOff?.Invoke();
        }

        public MultiSourceState(Action stateOn = null, Action stateOff = null)
        {
            this.stateOn = stateOn;
            this.stateOff = stateOff;
        }
    }
}