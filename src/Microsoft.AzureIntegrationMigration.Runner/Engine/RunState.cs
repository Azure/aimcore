using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;

namespace Microsoft.AzureIntegrationMigration.Runner.Engine
{
    /// <summary>
    /// Defines a class containing the execution state for the runner.
    /// </summary>
    public class RunState : IRunState
    {
        /// <summary>
        /// Defines the runner configuration specific to this instance.
        /// </summary>
        private readonly IRunnerConfiguration _config;

        /// <summary>
        /// Defines the model state for the application being assessed.
        /// </summary>
        private readonly IApplicationModel _model;

        /// <summary>
        /// Defines a list of stages and their associated run state.
        /// </summary>
        private readonly IDictionary<string, StageState> _executionState = new Dictionary<string, StageState>();

        /// <summary>
        /// Constructs a new instance of the <see cref="RunState" /> class with runner configuration.
        /// </summary>
        /// <param name="config">The configuration to be used for this runner instance.</param>
        public RunState(IRunnerConfiguration config)
            : this(config, null)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="RunState" /> class with runner configuration and an application model.
        /// </summary>
        /// <param name="config">The configuration to be used for this runner instance.</param>
        /// <param name="model">The application model state, or null.</param>
        public RunState(IRunnerConfiguration config, IApplicationModel model)
        {
            // Validate and set state
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _model = model;
        }

        #region IRunState Interface Implementation

        /// <summary>
        /// Gets the execution state for each of the stages.
        /// </summary>
        public IDictionary<string, StageState> ExecutionState => _executionState;

        /// <summary>
        /// Gets the configuration supplied to the runner.
        /// </summary>
        public IRunnerConfiguration Configuration => _config;

        /// <summary>
        /// Gets the application model state.
        /// </summary>
        public IApplicationModel Model => _model;

        #endregion
    }
}
