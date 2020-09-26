using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an interface for runner configuration.
    /// </summary>
    public interface IRunnerConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the runner should abort immediately
        /// if a stage component fails to execute.
        /// </summary>
        bool FailFast { get; set; }

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
        Stages FailStages { get; set; }

        /// <summary>
        /// Gets or sets the stages that this runner will execute.
        /// </summary>
        Stages Stages { get; set; }

        /// <summary>
        /// Gets a list of stage runners that this runner will execute if the relevant stages are enabled.
        /// </summary>
        IList<IStageRunner> StageRunners { get; }

        /// <summary>
        /// Gets a dictionary of arguments that can be used by implemented classes to access
        /// arbitrary configuration values.
        /// </summary>
        IDictionary<string, object> Args { get; }
    }
}
