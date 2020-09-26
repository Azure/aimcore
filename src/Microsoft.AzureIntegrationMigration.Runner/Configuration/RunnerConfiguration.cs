using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;

namespace Microsoft.AzureIntegrationMigration.Runner.Configuration
{
    /// <summary>
    /// Defines a class that holds runner configuration.
    /// </summary>
    public class RunnerConfiguration : IRunnerConfiguration
    {
        /// <summary>
        /// Defines a list of stage runners.
        /// </summary>
        private readonly List<IStageRunner> _stageRunners = new List<IStageRunner>();

        /// <summary>
        /// Constructs a new instance of the <see cref="RunnerConfiguration" /> class.
        /// </summary>
        public RunnerConfiguration()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="RunnerConfiguration" /> class with a list of stage runners.
        /// </summary>
        /// <param name="stageRunners">The stage runners to store in configuration state.</param>
        public RunnerConfiguration(IList<IStageRunner> stageRunners)
        {
            // Validate and set state
            _stageRunners.AddRange(stageRunners ?? throw new ArgumentNullException(nameof(stageRunners)));
        }

        #region IRunnerConfiguration Interface Implementation

        /// <summary>
        /// Gets or sets a value indicating whether the runner should abort immediately
        /// if a stage component fails to execute.
        /// </summary>
        public bool FailFast { get; set; }

        /// <summary>
        /// Gets or sets flags that are used to determine if a stage is enabled or disabled
        /// to abort execution at the end of a stage if any component in that stage fails.
        /// </summary>
        /// <remarks>
        /// To enable a flag for a stage perform a bitwise OR, for example:
        /// <code>
        /// config.FailStages |= Stages.Report;
        /// </code>
        /// </remarks>
        public Stages FailStages { get; set; } = Stages.None;

        /// <summary>
        /// Gets or sets the stages that the runner will execute.
        /// </summary>
        public Stages Stages { get; set; } = Stages.All;

        /// <summary>
        /// Gets a list of stage runners that this runner will execute if the relevant stages are enabled.
        /// </summary>
        public IList<IStageRunner> StageRunners => _stageRunners;

        /// <summary>
        /// Gets a dictionary of arguments that can be used by implemented classes to access
        /// arbitrary configuration values.
        /// </summary>
        public IDictionary<string, object> Args { get; } = new Dictionary<string, object>();

        #endregion
    }
}
