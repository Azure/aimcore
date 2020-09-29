// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Configuration;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Runner.Builder
{
    /// <summary>
    /// Defines a class that supports building a runner by building configuration in a fluent style.
    /// </summary>
    public class RunnerBuilder : IRunnerBuilder
    {
        /// <summary>
        /// Defines the configuration object that will be set up during build.
        /// </summary>
        private readonly IRunnerConfiguration _config = new RunnerConfiguration();

        /// <summary>
        /// Defines a list of stage component providers used to find components implementing stage runners.
        /// </summary>
        private readonly List<IStageComponentProvider> _providers = new List<IStageComponentProvider>();

        /// <summary>
        /// Defines a list of functions to set stage runner priorities.
        /// </summary>
        private readonly List<Func<IStageRunner, int>> _priorityFuncs = new List<Func<IStageRunner, int>>();

        /// <summary>
        /// Defines a list of functions to skip stage runners.
        /// </summary>
        private readonly List<Predicate<IStageRunner>> _skipFuncs = new List<Predicate<IStageRunner>>();

        /// <summary>
        /// Defines the logger used by the runner.  Default to a console logger.
        /// </summary>
        private ILogger<Engine.Runner> _logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Engine.Runner>();

        /// <summary>
        /// Defines the function that provides the application model to be used by the runner to store application state from the assessment.
        /// </summary>
        private Func<IApplicationModel> _modelFunc;

        #region IRunnerBuilder Interface Implementation

        /// <summary>
        /// Finds stage runners by using the supplied provider.
        /// </summary>
        /// <param name="provider">The provider used to find stage runners.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder FindStageRunners(IStageComponentProvider provider)
        {
            // Just add for now, will use to find components when building runner
            _providers.Add(provider ?? throw new ArgumentNullException(nameof(provider)));

            return this;
        }

        /// <summary>
        /// Adds a stage runner to the execution list which will be sorted based on stage and priority.
        /// </summary>
        /// <param name="runner">The stage runner to add to the list.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder AddStageRunner(IStageRunner runner)
        {
            // Add stage to configuration
            _config.StageRunners.Add(runner ?? throw new ArgumentNullException(nameof(runner)));

            return this;
        }

        /// <summary>
        /// Set the priority of one or more stage runners based on the supplied function used to match stage runners.
        /// </summary>
        /// <param name="priorityFunc">The function used to match the stage runners and return the new priority.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder SetStageRunnerPriority(Func<IStageRunner, int> priorityFunc)
        {
            // Add delegate to list to be run later during build
            _priorityFuncs.Add(priorityFunc ?? throw new ArgumentNullException(nameof(priorityFunc)));

            return this;
        }

        /// <summary>
        /// Skips one or more stage runners based on the predicate logic used to match stage runners.
        /// </summary>
        /// <param name="skipFunc">The predicate function used to match stage runners to skip.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder SkipStageRunner(Predicate<IStageRunner> skipFunc)
        {
            // Add delegate to list to be run later during build
            _skipFuncs.Add(skipFunc ?? throw new ArgumentNullException(nameof(skipFunc)));

            return this;
        }

        /// <summary>
        /// Disables the specified stage.
        /// </summary>
        /// <param name="stage">The stage to disable.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder DisableStage(Stages stage)
        {
            // Subtract flag value in configuration to disable stage
            _config.Stages -= stage;

            return this;
        }

        /// <summary>
        /// Enables the ability to abort execution at the first stage runner to fail.
        /// </summary>
        /// <returns>The builder.</returns>
        public IRunnerBuilder EnableFailFast()
        {
            // Switch on flag
            _config.FailFast = true;

            return this;
        }

        /// <summary>
        /// Enables the ability to abort execution at the completion of a stage if any stage runner in that stage failed.
        /// </summary>
        /// <param name="stage">The stage at which to abort execution.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder EnableFailStage(Stages stage)
        {
            // Switch on flag for a particular stage
            _config.FailStages |= stage;

            return this;
        }

        /// <summary>
        /// Sets the supplied arguments on the runner configuration.
        /// </summary>
        /// <param name="args">The arguments supplied to the process.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder SetArgs(IDictionary<string, object> args)
        {
            // Must provide args if explicitly calling this method
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // Merge and overwrite existing values with new, if needed
            foreach (var kvp in args)
            {
                if (_config.Args.ContainsKey(kvp.Key))
                {
                    _config.Args[kvp.Key] = kvp.Value;
                }
                else
                {
                    _config.Args.Add(kvp.Key, kvp.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the logger for the execution.
        /// </summary>
        /// <param name="logger">The logger to use, default is the console.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder SetLogger(ILogger<Engine.Runner> logger)
        {
            // Store logger to be passed to runner later
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            return this;
        }

        /// <summary>
        /// Sets the application model to be used by stage runner components to store and retrieve state
        /// about the application being assessed.
        /// </summary>
        /// <param name="modelFunc">The function that provides the application model state.</param>
        /// <returns>The builder.</returns>
        public IRunnerBuilder SetModel(Func<IApplicationModel> modelFunc)
        {
            // Store model to be passed to runner later
            _modelFunc = modelFunc ?? throw new ArgumentNullException(nameof(modelFunc));

            return this;
        }

        /// <summary>
        /// Builds the runner with the required configuration.
        /// </summary>
        /// <returns>The runner.</returns>
        public IRunner Build()
        {
            // Find stage runners first, using the providers if any
            foreach (var provider in _providers)
            {
                var runners = provider.FindComponents(_config);
                foreach (var runner in runners)
                {
                    _config.StageRunners.Add(runner);
                }
            }

            // Run functions to set stage priority and skip stage runners now
            // that a list of stage runners have been set up for the runner
            foreach (var runner in _config.StageRunners)
            {
                foreach (var priorityFunc in _priorityFuncs)
                {
                    runner.Priority = priorityFunc(runner);
                }

                foreach (var skipFunc in _skipFuncs)
                {
                    runner.Skip = skipFunc(runner);
                }
            }

            // Run function to get model
            IApplicationModel model = null;
            if (_modelFunc != null)
            {
                model = _modelFunc();
            }

            return new Engine.Runner(_config, model, _logger);
        }

        #endregion
    }
}
