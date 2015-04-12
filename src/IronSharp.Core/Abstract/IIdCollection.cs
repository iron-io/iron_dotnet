using System.Collections.Generic;

namespace IronIO.Core
{
    public interface IIdCollection
    {
        IEnumerable<string> GetIds();
    }
}