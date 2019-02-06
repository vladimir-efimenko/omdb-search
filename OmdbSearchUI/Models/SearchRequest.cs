
using System;

namespace OmdbSearchUI.Models
{
    public class SearchRequest : IEquatable<SearchRequest>
    {
        public SearchRequest()
        {
        }

        public SearchRequest(string title, int page = 1)
        {
            Title = title;
            Page = page;
        }

        public string Title { get; set; }

        public int Page { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchRequest) obj);
        }

        public bool Equals(SearchRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Title, other.Title) && Page == other.Page;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Title != null ? Title.GetHashCode() : 0) * 397) ^ Page;
            }
        }
    }
}
