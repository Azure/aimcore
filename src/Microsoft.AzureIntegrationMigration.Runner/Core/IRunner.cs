// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an interface for a runner that will control the execution of stage components
    /// as part of a migration to Azure.
    /// </summary>
    public interface IRunner
    {
#pragma warning disable CA1713 // Events should not have 'Before' or 'After' prefix
        /// <summary>
        /// Defines an event that is fired before the runner starts executing components in a stage.
        /// </summary>
        event EventHandler<ModelStateEventArgs> BeforeStage;

        /// <summary>
        /// Defines an event that is fired after the runner finishes executing components in a stage.
        /// </summary>
        event EventHandler<ModelStateEventArgs> AfterStage;

        /// <summary>
        /// Defines an event that is fired before the runner starts executing a stage runner.
        /// </summary>
        event EventHandler<ModelStateEventArgs> BeforeStageRunner;

        /// <summary>
        /// Defines an event that is fired after the runner finishes executing a stage runner.
        /// </summary>
        event EventHandler<ModelStateEventArgs> AfterStageRunner;
#pragma warning restore CA1713 // Events should not have 'Before' or 'After' prefix

        /// <summary>
        /// Gets the execution state of the runner.
        /// </summary>
        IRunState RunState { get; }

        /// <summary>
        /// Executes the runner which in turn executes the stage components.
        /// </summary>
        /// <param name="token">The cancellation token to use to check for async operation cancellation.</param>
        /// <returns>A task used to await this operation.</returns>
        Task RunAsync(CancellationToken token);
    }
}
