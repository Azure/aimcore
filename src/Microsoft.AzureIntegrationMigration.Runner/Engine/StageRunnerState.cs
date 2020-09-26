using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;

namespace Microsoft.AzureIntegrationMigration.Runner.Engine
{
    /// <summary>
    /// Defines a class containing the execution state for a stage runner.
    /// </summary>
    public class StageRunnerState
    {
        /// <summary>
        /// Gets or sets a value indicating whether this stage runner is currently executing or not.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Gets or sets the state associated with the stage runner.
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Gets or sets the exception associated with a failed stage runner.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the stage runner was started to be executed.
        /// </summary>
        public DateTimeOffset Started { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the stage runner execution was completed.
        /// </summary>
        public DateTimeOffset Completed { get; set; }

        /// <summary>
        /// Gets or sets the stage runner.
        /// </summary>
        public IStageRunner StageRunner { get; set; }
    }
}
