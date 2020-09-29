// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.AzureIntegrationMigration.Runner.Resources;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Runner.Engine
{
    /// <summary>
    /// Defines a class that implements a runner to execute stages as part of an Azure migration.
    /// </summary>
    public class Runner : IRunner
    {
        /// <summary>
        /// Defines the execution state for this runner instance.
        /// </summary>
        private readonly IRunState _runState;

        /// <summary>
        /// Defines the logger to use for tracing in the runner.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Defines the list and order of stages to execute by default.
        /// </summary>
        private readonly Queue<Stages> _stages = new Queue<Stages>(6);

        /// <summary>
        /// Constructs a new instance of the <see cref="Runner" /> class with runner configuration.
        /// </summary>
        /// <param name="config">The configuration to be used for this runner instance.</param>
        /// <param name="logger">The logger to use for tracing.</param>
        public Runner(IRunnerConfiguration config, ILogger<Runner> logger)
            : this(config, null, logger)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Runner" /> class with runner configuration and an application model.
        /// </summary>
        /// <param name="config">The configuration to be used for this runner instance.</param>
        /// <param name="model">The initialized application model state for the application being assessed, or null.</param>
        /// <param name="logger">The logger to use for tracing.</param>
        public Runner(IRunnerConfiguration config, IApplicationModel model, ILogger<Runner> logger)
        {
            // Validate and set state
            _runState = new RunState(config ?? throw new ArgumentNullException(nameof(config)), model);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set stage order of execution
            _stages.Enqueue(Stages.Discover);
            _stages.Enqueue(Stages.Parse);
            _stages.Enqueue(Stages.Analyze);
            _stages.Enqueue(Stages.Report);
            _stages.Enqueue(Stages.Convert);
            _stages.Enqueue(Stages.Verify);
        }

        /// <summary>
        /// Fire event handler when starting execution of a stage.
        /// </summary>
        /// <param name="e">The model state event argument.</param>
        protected virtual void OnBeforeStage(ModelStateEventArgs e)
        {
            BeforeStage?.Invoke(this, e);
        }

        /// <summary>
        /// Fire event handler when having finished execution of a stage.
        /// </summary>
        /// <param name="e">The model state event argument.</param>
        protected virtual void OnAfterStage(ModelStateEventArgs e)
        {
            AfterStage?.Invoke(this, e);
        }

        /// <summary>
        /// Fire event handler when starting execution of a stage runner.
        /// </summary>
        /// <param name="e">The model state event argument.</param>
        protected virtual void OnBeforeStageRunner(ModelStateEventArgs e)
        {
            BeforeStageRunner?.Invoke(this, e);
        }

        /// <summary>
        /// Fire event handler when having finished execution of a stage runner.
        /// </summary>
        /// <param name="e">The model state event argument.</param>
        protected virtual void OnAfterStageRunner(ModelStateEventArgs e)
        {
            AfterStageRunner?.Invoke(this, e);
        }

        #region IRunner Interface Implementation

#pragma warning disable CA1713 // Events should not have 'Before' or 'After' prefix
        /// <summary>
        /// Defines an event that is fired before the runner starts executing components in a stage.
        /// </summary>
        public event EventHandler<ModelStateEventArgs> BeforeStage;

        /// <summary>
        /// Defines an event that is fired after the runner finishes executing components in a stage.
        /// </summary>
        public event EventHandler<ModelStateEventArgs> AfterStage;

        /// <summary>
        /// Defines an event that is fired before the runner starts executing a stage runner.
        /// </summary>
        public event EventHandler<ModelStateEventArgs> BeforeStageRunner;

        /// <summary>
        /// Defines an event that is fired after the runner finishes executing a stage runner.
        /// </summary>
        public event EventHandler<ModelStateEventArgs> AfterStageRunner;
#pragma warning restore CA1713 // Events should not have 'Before' or 'After' prefix

        /// <summary>
        /// Gets the execution state of the runner.
        /// </summary>
        public IRunState RunState => _runState;

        /// <summary>
        /// Executes the runner which in turn executes the stage components.
        /// </summary>
        /// <param name="token">The cancellation token to use to check for async operation cancellation.</param>
        /// <returns>A task used to await this operation.</returns>
        public async Task RunAsync(CancellationToken token)
        {
            _logger.LogInformation(InformationMessages.RunnerStarting);

            // Verify that there are stages to execute
            if (_runState.Configuration.Stages == Stages.None)
            {
                _logger.LogWarning(WarningMessages.ConfigurationDefinesNoStagesToRun);
                return;
            }

            // Verify that there are stage runners to execute
            if (_runState.Configuration.StageRunners.Count == 0)
            {
                _logger.LogWarning(WarningMessages.ConfigurationDefinesNoStageRunnersToRun);
                return;
            }

            // Verify that not all stage runners are set to be skipped
            if (_runState.Configuration.StageRunners.All(r => r.Skip))
            {
                _logger.LogWarning(WarningMessages.ConfigurationDefinesAllStageRunnersToSkipRun, _runState.Configuration.StageRunners.Count);
                return;
            }

            try
            {
                // Execute stages
                await ExecuteStages(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(WarningMessages.RunnerCancelledByCaller);

                throw;
            }

            _logger.LogInformation(InformationMessages.RunnerCompleted);
        }

        #endregion

        /// <summary>
        /// Executes all of the stages identified by the configuration.
        /// </summary>
        /// <param name="token">Cancellation token used to cancel the operation.</param>
        /// <returns>A task used to await the operation.</returns>
        private async Task ExecuteStages(CancellationToken token)
        {
            _logger.LogDebug(TraceMessages.RunningStages, _stages.Count);

            // Execute stages in the correct order, provided they are not skipped
            while (_stages.Count > 0)
            {
                // Get next stage from queue
                var stage = _stages.Dequeue();

                // Add execution state for stage
                _runState.ExecutionState.Add(stage.ToString("G"), new StageState() { Stage = stage, State = State.Ready, Started = DateTimeOffset.Now });

                // Is stage set to be skipped (turned off in config)?
                if ((_runState.Configuration.Stages & stage) == stage)
                {
                    _logger.LogInformation(InformationMessages.RunningStage, stage.ToString("G"));

                    token.ThrowIfCancellationRequested();
                    await ExecuteStage(stage, token).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogInformation(InformationMessages.SkippingStage, stage.ToString("G"));

                    _runState.ExecutionState[stage.ToString("G")].State = State.Skipped;
                    _runState.ExecutionState[stage.ToString("G")].Completed = DateTimeOffset.Now;
                }
            }
        }


        /// <summary>
        /// Executes the specific stage passed as an argument.
        /// </summary>
        /// <param name="stage">The stage to execute.</param>
        /// <param name="token">Cancellation token used to cancel the operation.</param>
        /// <returns>A task used to await the operation.</returns>
        private async Task ExecuteStage(Stages stage, CancellationToken token)
        {
            // Set state and timestamp
            _runState.ExecutionState[stage.ToString("G")].State = State.Running;
            _runState.ExecutionState[stage.ToString("G")].IsCurrent = true;
            RaiseEvent(OnBeforeStage);

            // Filter and sort list of stage runners for stage and in priority order
            var stageRunners = from runner in _runState.Configuration.StageRunners
                               where (runner.Stages & stage) == stage
                               orderby runner.Priority ascending
                               select runner;

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(TraceMessages.FilteredStageRunners, stageRunners.Count(), stage);

                var groupedRunners = from r in stageRunners
                                     group r by r.Priority;

                foreach (var priority in groupedRunners)
                {
                    _logger.LogDebug(TraceMessages.PriorityOrderForStageRunners, priority.Key, priority.Select(r => r.Name).ToArray());
                }
            }

            // Add stage runner state to stage state
            var states = stageRunners.Select(r => new StageRunnerState() { State = State.Ready, Started = DateTimeOffset.Now, StageRunner = r });
            foreach (var state in states)
            {
                _runState.ExecutionState[stage.ToString("G")].ExecutionState.Add(state);
            }

            // Execute each runner in turn, sorted by priority
            Exception stageFailure = null;
            foreach (var stageRunnerState in _runState.ExecutionState[stage.ToString("G")].ExecutionState)
            {
                try
                {
                    await ExecuteStageRunner(stage, stageRunnerState, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _runState.ExecutionState[stage.ToString("G")].State = State.Cancelled;
                    _runState.ExecutionState[stage.ToString("G")].Completed = DateTimeOffset.Now;
                    RaiseEvent(OnAfterStage);
                    _runState.ExecutionState[stage.ToString("G")].IsCurrent = false;

                    throw;
                }
                catch (Exception e)
                {
                    // Is runner set to abort on any stage runner exception (FailFast)?
                    if (_runState.Configuration.FailFast)
                    {
                        _logger.LogError(ErrorMessages.StageRunnerFailedAbortingRunner);

                        // Yes, re-throw
                        _runState.ExecutionState[stage.ToString("G")].State = State.Failed;
                        _runState.ExecutionState[stage.ToString("G")].Completed = DateTimeOffset.Now;
                        RaiseEvent(OnAfterStage);
                        _runState.ExecutionState[stage.ToString("G")].IsCurrent = false;

                        throw new RunnerException(ErrorMessages.StageRunnerFailedAbortingRunner, e);
                    }

                    // Capture most recent stage runner execution, so stage can be aborted if needed
                    stageFailure = e;
                }
            }

            // Got to the end of the stage, do we need to abort (FailStages)?
            if (stageFailure != null && (_runState.Configuration.FailStages & stage) == stage)
            {
                _logger.LogError(ErrorMessages.StageFailedAbortingRunner, stage);

                _runState.ExecutionState[stage.ToString("G")].State = State.Failed;
                _runState.ExecutionState[stage.ToString("G")].Completed = DateTimeOffset.Now;
                RaiseEvent(OnAfterStage);
                _runState.ExecutionState[stage.ToString("G")].IsCurrent = false;

                throw new RunnerException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.StageFailedErrorMessage, stage), stageFailure);
            }

            // Set state and timestamp
            _runState.ExecutionState[stage.ToString("G")].State = State.Completed;
            _runState.ExecutionState[stage.ToString("G")].Completed = DateTimeOffset.Now;
            RaiseEvent(OnAfterStage);
            _runState.ExecutionState[stage.ToString("G")].IsCurrent = false;
        }


        /// <summary>
        /// Executes the stage runner passed as an argument.
        /// </summary>
        /// <param name="stage">The stage currently being run.</param>
        /// <param name="state">The stage runner state, containing the runner, to execute.</param>
        /// <param name="token">Cancellation token used to cancel the operation.</param>
        /// <returns>A task used to await the operation.</returns>
        private async Task ExecuteStageRunner(Stages stage, StageRunnerState state, CancellationToken token)
        {
            try
            {
                // Set state and timestamp
                state.State = State.Running;
                state.IsCurrent = true;
                RaiseEvent(OnBeforeStageRunner);

                // Should it be skipped?
                if (!state.StageRunner.Skip)
                {
                    _logger.LogInformation(InformationMessages.RunningStageRunner, state.StageRunner.Name ?? state.StageRunner.GetType().FullName, stage.ToString("G"));

                    token.ThrowIfCancellationRequested();
                    await state.StageRunner.RunAsync(_runState, token).ConfigureAwait(false);

                    // Set state and timestamp
                    state.State = State.Completed;
                    state.Completed = DateTimeOffset.Now;
                    RaiseEvent(OnAfterStageRunner);
                    state.IsCurrent = false;
                }
                else
                {
                    _logger.LogInformation(InformationMessages.SkippingStageRunner, state.StageRunner.Name ?? state.StageRunner.GetType().FullName, stage.ToString("G"));

                    // Set skipped state
                    state.State = State.Skipped;
                    state.Completed = DateTimeOffset.Now;
                    RaiseEvent(OnAfterStageRunner);
                    state.IsCurrent = false;
                }
            }
            catch (OperationCanceledException)
            {
                state.State = State.Cancelled;
                state.Completed = DateTimeOffset.Now;
                RaiseEvent(OnAfterStageRunner);
                state.IsCurrent = false;

                throw;
            }
            catch (Exception e)
            {
                // Set stage runner failure
                state.State = State.Failed;
                state.Error = e;
                state.Completed = DateTimeOffset.Now;
                RaiseEvent(OnAfterStageRunner);
                state.IsCurrent = false;

                throw;
            }
        }

        /// <summary>
        /// Helper method to raise event only when there is an application model object.
        /// </summary>
        /// <param name="eventFunc">The event function to call if there is a model object.</param>
        private void RaiseEvent(Action<ModelStateEventArgs> eventFunc)
        {
            if (_runState.Model != null)
            {
                eventFunc(new ModelStateEventArgs(_runState.Model));
            }
            else
            {
                _logger.LogDebug(TraceMessages.NotRaisingEventNoModel, eventFunc.Method.Name);
            }
        }
    }
}
