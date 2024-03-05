
namespace Octopus.Client.Model
{
    public class PreconditionStatus
    {
        /// <summary>
        /// The human-readable name of the precondition which has raised
        /// this message/status
        /// </summary>
        public string PreconditionName { get; set; }

        /// <summary>
        /// A human-readable statement indicating the status of the
        /// Precondition
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// True if this precondition will outright fail the task (terminal)
        /// False if this precondition will delay the task's execution
        /// </summary>
        public bool WillFailTask { get; set; }
    }
}