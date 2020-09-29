// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Runner.Core
{
    /// <summary>
    /// Defines an enumeration of stage values used by stage components.
    /// </summary>
    [Flags]
    public enum Stages
    {
        /// <summary>
        /// Represents no stage.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents the discover stage.
        /// </summary>
        Discover = 1,

        /// <summary>
        /// Represents the parse stage.
        /// </summary>
        Parse = 2,

        /// <summary>
        /// Represents the analyze stage.
        /// </summary>
        Analyze = 4,

        /// <summary>
        /// Represents the report stage.
        /// </summary>
        Report = 8,

        /// <summary>
        /// Represents the convert stage.
        /// </summary>
        Convert = 16,

        /// <summary>
        /// Represents the verify stage.
        /// </summary>
        Verify = 32,

        /// <summary>
        /// Represents all stages.
        /// </summary>
        All = Discover | Parse | Analyze | Report | Convert | Verify,

        /// <summary>
        /// Represents assessment stages Discover, Parse, Analyze and Report.
        /// </summary>
        Assessment = Discover | Parse | Analyze | Report,

        /// <summary>
        /// Represents conversion stages Convert and Verify.
        /// </summary>
        Conversion = Convert | Verify
    }
}
