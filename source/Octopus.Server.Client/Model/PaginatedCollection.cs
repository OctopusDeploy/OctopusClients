using System.Collections.Generic;

namespace Octopus.Client.Model
{
    class PaginatedCollection<TResource>
    {
        public int TotalResults { get; set; }

        public int ItemsPerPage { get; set; }

        public int NumberOfPages { get; set; }

        public IList<TResource> Items { get; set; } = new List<TResource>();
    }
}
