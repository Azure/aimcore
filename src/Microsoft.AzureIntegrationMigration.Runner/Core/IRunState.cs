using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner.Engine;
using Microsoft.AzureIntegrationMigration.Runner.Model;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an interface for the execution state associated with a runner instance.
    /// </summary>
    public interface IRunState
    {
        /// <summary>
        /// Gets the execution state for each of the stages.
        /// </summary>
        IDictionary<string, StageState> ExecutionState { get; }

        /// <summary>
        /// Gets the configuration supplied to the runner.
        /// </summary>
        IRunnerConfiguration Configuration { get; }

        /// <summary>
        /// Gets the application model state.
        /// </summary>
        IApplicationModel Model { get; }
    }
}
