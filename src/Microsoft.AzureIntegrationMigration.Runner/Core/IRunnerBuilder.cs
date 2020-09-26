using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an interface for a builder that constructs a runner.
    /// </summary>
    public interface IRunnerBuilder
    {
        /// <summary>
        /// Finds stage runners by using the supplied provider.
        /// </summary>
        /// <param name="provider">The provider used to find stage runners.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder FindStageRunners(IStageComponentProvider provider);

        /// <summary>
        /// Adds a stage runner to the execution list which will be sorted based on stage and priority.
        /// </summary>
        /// <param name="runner">The stage runner to add to the list.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder AddStageRunner(IStageRunner runner);

        /// <summary>
        /// Skips one or more stage runners based on the predicate logic used to match stage runners.
        /// </summary>
        /// <param name="skipFunc">The predicate function used to match stage runners to skip.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder SkipStageRunner(Predicate<IStageRunner> skipFunc);

        /// <summary>
        /// Set the priority of one or more stage runners based on the supplied function used to match stage runners.
        /// </summary>
        /// <param name="priorityFunc">The function used to match the stage runners and return the new priority.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder SetStageRunnerPriority(Func<IStageRunner, int> priorityFunc);

        /// <summary>
        /// Disables the specified stage.
        /// </summary>
        /// <param name="stage">The stage to disable.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder DisableStage(Stages stage);

        /// <summary>
        /// Enables the ability to abort execution at the first stage runner to fail.
        /// </summary>
        /// <returns>The builder.</returns>
        IRunnerBuilder EnableFailFast();

        /// <summary>
        /// Enables the ability to abort execution at the completion of a stage if any stage runner in that stage failed.
        /// </summary>
        /// <param name="stage">The stage at which to abort execution.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder EnableFailStage(Stages stage);

        /// <summary>
        /// Sets the supplied arguments on the runner configuration.
        /// </summary>
        /// <param name="args">The arguments supplied to the process.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder SetArgs(IDictionary<string, object> args);

        /// <summary>
        /// Sets the logger for the execution.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder SetLogger(ILogger<Engine.Runner> logger);

        /// <summary>
        /// Sets the application model to be used by stage runner components to store and retrieve state
        /// about the application being assessed.
        /// </summary>
        /// <param name="modelFunc">The function that provides the application model state.</param>
        /// <returns>The builder.</returns>
        IRunnerBuilder SetModel(Func<IApplicationModel> modelFunc);

        /// <summary>
        /// Builds the runner with the required configuration.
        /// </summary>
        /// <returns>The runner.</returns>
        IRunner Build();
    }
}
