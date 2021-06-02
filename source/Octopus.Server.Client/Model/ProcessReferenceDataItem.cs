namespace Octopus.Client.Model
{
    public class ProcessReferenceDataItem: ReferenceDataItem {
        public ProcessType ProcessType { get; set; }

        public ProcessReferenceDataItem(string id, string name, ProcessType processType) : base(id, name)
        {
            ProcessType = processType;
        }
    }
}