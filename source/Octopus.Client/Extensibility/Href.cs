using System;

namespace Octopus.Client.Extensibility
{
    public class Href : IEquatable<Href>
    {
        readonly string link;

        public Href(string link)
        {
            this.link = link;
        }

        public string AsString()
        {
            return link;
        }

        public bool Equals(Href other)
        {
            return string.Equals(link, other.link);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Href)obj);
        }

        public override int GetHashCode()
        {
            return (link != null ? link.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return link;
        }

        public static bool operator ==(Href left, Href right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Href left, Href right)
        {
            return !Equals(left, right);
        }

        public static implicit operator String(Href href)
        {
            return href.link;
        }

        public static implicit operator Href(string href)
        {
            return new Href(href);
        }
    }
}