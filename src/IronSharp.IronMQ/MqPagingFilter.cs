using System;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class MqPagingFilter : PagingFilter
    {
        public MqPagingFilter()
        {
        }

        public MqPagingFilter(string previous = null, int? perPage = null)
        {
            Previous = previous;
            PerPage = perPage.GetValueOrDefault();
        }

        public MqPagingFilter(IPagingFilter filter)
        {
            if (filter == null)
            {
                return;
            }
            PerPage = filter.PerPage;
            if (filter is MqPagingFilter)
                Previous = ((MqPagingFilter)filter).Previous;
        }

        private const string PAGE_NOT_SUPPORTED = "Pagination principle in List Queues changed. It doesn’t support Page parameter. You should specify the name of queue prior to the first desirable queue in result.";
        [JsonProperty("page")]
        public override int Page
        {
            get { return default(int); }
            set { throw new NotSupportedException(PAGE_NOT_SUPPORTED); }
        }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }


        /// <summary>
        /// The name of queue prior to the first desirable queue in result. 
        /// If queue with specified name doesn’t exist result will contain first 
        /// <see cref="PerPage"/> amount of queues that lexicographically greater than Previous
        /// </summary>
        [JsonProperty("previous")]
        public string Previous { get; set; }

        public static implicit operator NameValueCollection(MqPagingFilter filter)
        {
            var collection = new NameValueCollection();

            if (filter != null && filter.PerPage > 0)
            {
                collection.Add("per_page", Convert.ToString(filter.PerPage));
            }

            if (filter != null && !String.IsNullOrEmpty(filter.Previous))
            {
                collection.Add("previous", filter.Previous);
            }

            return collection;
        }
    }
}