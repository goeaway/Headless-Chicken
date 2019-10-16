using System;
using System.Threading.Tasks;

namespace HeadlessChicken.Core.Pausing
{
    public struct PauseToken
    {
        // https://stackoverflow.com/a/20186155
        private readonly PauseTokenSource source;

        internal PauseToken(PauseTokenSource source)
        {
            this.source = source;
        }

        public bool IsPaused => source != null && source.IsPaused;
    }
}
