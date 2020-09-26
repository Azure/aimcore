using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an enumeration of state values used by the runner.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Represents an initialized stage or stage runner not yet ready to run.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a stage or stage runner ready to run.
        /// </summary>
        Ready,

        /// <summary>
        /// Represents a stage or stage runner that is being executed by the runner.
        /// </summary>
        Running,

        /// <summary>
        /// Represents a stage or stage runner that has completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Represents a stage or stage runner that has been skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Represents a stage or stage runner that has been cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Represents a stage or stage runner that has failed.
        /// </summary>
        Failed
    }
}
