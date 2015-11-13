using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronIO.Core.Extensions
{
   public static class ExtensionsForInteger
    {
        /// <summary>
        /// Returns the specified <param name="value"></param> if it falls with the specified range.  Otherwise, the min or max value is returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public static int WithRange(this int value, int? min, int? max)
       {
           if (min.HasValue && value < min.Value)
           {
               return min.Value;
           }

           if (max.HasValue && value > max.Value)
           {
               return max.Value;
           }

           return value;
       }
    }
}
