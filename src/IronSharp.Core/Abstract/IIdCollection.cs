using System.Collections.Generic;

namespace IronSharp.Core
{
    public interface IIdCollection
    {
        IEnumerable<string> GetIds();
    }
}