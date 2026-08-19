namespace Octopus.Client.Model
{

    public class VariableIdentifier
    {
        /// <summary>
        ///  Identifies a variable by name and owner within a variable snapshot 
        /// </summary>
        public VariableIdentifier()
        {
        }

        public VariableIdentifier(string variableName, string ownerId)
        {
            Name = variableName;
            OwnerId = ownerId;
        }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The set id that the variable belongs to e.g. ProjectId for project variables or VariableSetId for library variables
        /// </summary>
        public string OwnerId { get; set; }

    }
}
