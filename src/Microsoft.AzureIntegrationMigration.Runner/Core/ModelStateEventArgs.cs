using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Model;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines a class for runner events that contains the application model.
    /// </summary>
    public class ModelStateEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="ModelStateEventArgs" /> class.
        /// </summary>
        /// <param name="model">The application model state.</param>
        public ModelStateEventArgs(IApplicationModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Gets the application model state.
        /// </summary>
        public IApplicationModel Model { get; private set; }
    }
}
