using System;
using System.Globalization;

namespace IronIO.Core
{
    public static class KeystoneUtil
    {
        public static bool CurrentTokenIsInvalid(AuthToken token, DateTime localExpiresAt, DateTime? now = null)
        {
            if (token == null)
            {
                return true;
            }

            return localExpiresAt.CompareTo(now.GetValueOrNow()) < 0;
        }

        private static DateTime GetValueOrNow(this DateTime? now)
        {
            return now.GetValueOrDefault(DateTime.Now);
        }

        public static DateTime ComputeNewExpirationDate(KestoneToken token, DateTime? now = null)
        {
            var issuedAt = DateTime.Parse(token.IssuedAt, null, DateTimeStyles.RoundtripKind);
            var expires = DateTime.Parse(token.Expires, null, DateTimeStyles.RoundtripKind);

            var timespan = expires.Subtract(issuedAt);

            return now.GetValueOrNow().Add(timespan);
        }
    }
}