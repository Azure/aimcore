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
    /// Defines an interface for a component that finds and provides stage components to the runner.
    /// </summary>
    public interface IStageComponentProvider
    {
        /// <summary>
        /// Runs the stage implemented by the stage component.
        /// </summary>
        /// <param name="config">The configuration provided to the runner.</param>
        IEnumerable<IStageRunner> FindComponents(IRunnerConfiguration config);
    }
}
