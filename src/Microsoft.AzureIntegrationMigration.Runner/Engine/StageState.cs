// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;

namespace Microsoft.AzureIntegrationMigration.Runner.Engine
{
    /// <summary>
    /// Defines a class containing the execution state for the stages.
    /// </summary>
    public class StageState
    {
        /// <summary>
        /// Gets or sets a value indicating whether this stage is currently executing or not.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Gets or sets the stage associated with this state object.
        /// </summary>
        public Stages Stage { get; set; }

        /// <summary>
        /// Gets or sets the execution state associated with this state object.
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the stage was started to be executed.
        /// </summary>
        public DateTimeOffset Started { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the stage execution was completed.
        /// </summary>
        public DateTimeOffset Completed { get; set; }

        /// <summary>
        /// Gets a list of the stage runners and their execution state in the correct order for execution.
        /// </summary>
        public IList<StageRunnerState> ExecutionState { get; } = new List<StageRunnerState>();
    }
}
