using System.Collections.Generic;
using IronIO.Core.Extensions;

namespace IronSharp.IronMQ
{
    internal static class ReservationUtil
    {
        public static Dictionary<string, object> BuildReservationFields(MessageReservationOptions reservationOptions)
        {
            reservationOptions = reservationOptions ?? new MessageReservationOptions();

            var payload = new Dictionary<string, object>
            {
                {"n", reservationOptions.Number.WithRange(1, 100)}
            };

            SetReservationOptions(reservationOptions, payload);

            return payload;
        }

        public static Dictionary<string, object> BuildReservationFields(ReservationOptions reservationOptions)
        {
            reservationOptions = reservationOptions ?? new MessageReservationOptions();

            var payload = new Dictionary<string, object>
            {
                {"n", 1}
            };

            SetReservationOptions(reservationOptions, payload);

            return payload;
        }

        private static void SetReservationOptions(ReservationOptions reservationOptions, IDictionary<string, object> payload)
        {
            if (reservationOptions.Timeout.HasValue)
            {
                payload.Add("timeout", reservationOptions.Timeout.GetSeconds(60, 30, 86400));
            }

            if (reservationOptions.Wait.HasValue)
            {
                payload.Add("wait", reservationOptions.Wait.GetSeconds(0, 0, 30));
            }

            if (reservationOptions.Delete.GetValueOrDefault())
            {
                payload.Add("delete", true);
            }
        }
    }
}