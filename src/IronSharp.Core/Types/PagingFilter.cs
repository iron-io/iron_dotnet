using System;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    public class PagingFilter : IPagingFilter
    {
        public PagingFilter()
        {
        }

        public PagingFilter(int? page = null, int? perPage = null)
        {
            Page = page.GetValueOrDefault();
            PerPage = perPage.GetValueOrDefault();
        }

        public PagingFilter(IPagingFilter filter)
        {
            if (filter == null)
            {
                return;
            }
            Page = filter.Page;
            PerPage = filter.PerPage;
        }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        public static implicit operator NameValueCollection(PagingFilter filter)
        {
            var collection = new NameValueCollection();

            if (filter != null && filter.Page > 0)
            {
                collection.Add("page", Convert.ToString(filter.Page));
            }

            if (filter != null && filter.PerPage > 0)
            {
                collection.Add("per_page", Convert.ToString(filter.PerPage));
            }

            return collection;
        }
    }
}