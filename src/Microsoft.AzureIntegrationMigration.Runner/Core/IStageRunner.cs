using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an interface for a component that runs in a stage.
    /// </summary>
    public interface IStageRunner : IRunnerComponent
    {
        /// <summary>
        /// Gets the name of the stage runner.
        /// </summary>
        /// <remarks>
        /// This should be as unique as possible because the CLI uses this to set stage runner
        /// properties, such as Priority and Skip.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Gets or sets the priority of the stage runner, where priority is in descending order, with 0 highest priority.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the runner should skip execution of this stage component.
        /// </summary>
        bool Skip { get; set; }

        /// <summary>
        /// Gets the stage or stages that this component supports.
        /// </summary>
        Stages Stages { get; }

        /// <summary>
        /// Runs the stage implemented by the stage component.
        /// </summary>
        /// <param name="state">The execution state that can be added to or modified by the implementing class.</param>
        /// <param name="token">The cancellation token to use to check for async operation cancellation.</param>
        Task RunAsync(IRunState state, CancellationToken token);
    }
}
