using System;

namespace Octopus.Client.Model
{
    public class EventReference
    {
        readonly string referencedDocumentId;
        readonly int startIndex;
        readonly int length;

        public EventReference(string referencedDocumentId, int startIndex, int length)
        {
            this.referencedDocumentId = referencedDocumentId;
            this.startIndex = startIndex;
            this.length = length;
        }

        public string ReferencedDocumentId
        {
            get { return referencedDocumentId; }
        }

        public int StartIndex
        {
            get { return startIndex; }
        }

        public int Length
        {
            get { return length; }
        }
    }
}