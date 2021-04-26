// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Engine;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xbehave;
using Xunit;

namespace Microsoft.AzureIntegrationMigration.Runner.Tests
{
    /// <summary>
    /// Defines the test spec for the <see cref="RunnerException"/> class.
    /// </summary>
    public class RunExceptionFeature
    {
        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction succeeds when null message is passed.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullMessage(RunnerException rex, Exception e)
        {
            "Given the runner exception"
                .x(() => rex.Should().BeNull());

            "When constructing with null message"
                .x(() => e = Record.Exception(() => new RunnerException(null)));

            "Then the runner exception constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when null inner exception is passed.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullInnerException(RunnerException rex, Exception inner, Exception e)
        {
            "Given the runner exception"
                .x(() => rex.Should().BeNull());

            "And a null inner exception"
                .x(() => inner.Should().BeNull());

            "When constructing with null message"
                .x(() => e = Record.Exception(() => new RunnerException("an error message", inner)));

            "Then the runner exception constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when a message is passed.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithMessageWithSuccess(RunnerException rex, Exception e)
        {
            "Given the runner exception"
                .x(() => rex.Should().BeNull());

            "When constructing with a message"
                .x(() => e = Record.Exception(() => new RunnerException("an error message")));

            "Then the runner exception constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when a message and an inner exception is passed.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithMessageAndInnerExceptionWithSuccess(RunnerException rex, Exception inner, Exception e)
        {
            "Given the runner exception"
                .x(() => rex.Should().BeNull());

            "And an inner exception"
                .x(() => inner = new Exception("an inner exception"));

            "When constructing with a message and inner exception"
                .x(() => e = Record.Exception(() => new RunnerException("an error message", inner)));

            "Then the runner exception constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(RunnerException rex, Exception e)
        {
            "Given the runner exception"
                .x(() => rex.Should().BeNull());

            "When constructing with default constructor"
                .x(() => e = Record.Exception(() => new RunnerException()));

            "Then the runner exception constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when serializing and deserializing the exception.
        /// </summary>
        /// <param name="rex">The runner exception.</param>
        /// <param name="formatter">The binary formatter.</param>
        /// <param name="stream">The memory stream used for serialization.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        public void ConstructWithSerializationSuccess(RunnerException rex, IFormatter formatter, MemoryStream stream, Exception e)
        {
            "Given the runner exception"
                .x(() => rex = new RunnerException("an error message"));

            "And a binary formatter for serializing"
                .x(() => formatter = new BinaryFormatter());

            "And a memory stream"
                .x(s => stream = new MemoryStream().Using(s));

            "And a serialized exception"
                .x(s =>
                {
                    formatter.Serialize(stream, rex);
                    stream.Position = 0;
                });

            "When deserializing the exception"
                .x(() => e = Record.Exception(() => rex = (RunnerException)formatter.Deserialize(stream)));

            "Then the runner exception deserializing constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the exception should have our custom message"
                .x(() => rex.Message.Should().Be("an error message"));
        }
#pragma warning restore SYSLIB0011 // Type or member is obsolete

        #endregion
    }
}
