// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Runner.Engine
{
    /// <summary>
    /// Defines a runner exception.
    /// </summary>
    [Serializable]
    public class RunnerException : Exception
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="RunnerException"/> class.
        /// </summary>
        public RunnerException()
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="RunnerException"/> class with a custom message.
        /// </summary>
        /// <param name="message">A custom exception message.</param>
        public RunnerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="RunnerException"/> class with a custom message and an inner exception.
        /// </summary>
        /// <param name="message">A custom exception message.</param>
        /// <param name="innerException">An inner exception.</param>
        public RunnerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Supports custom serialization of the exception.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RunnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
