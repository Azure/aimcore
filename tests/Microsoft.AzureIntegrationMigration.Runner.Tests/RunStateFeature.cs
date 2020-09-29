// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
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
    /// Defines the test spec for the <see cref="RunState"/> class.
    /// </summary>
    public class RunStateFeature
    {
        /// <summary>
        /// Defines a mocked configuration.
        /// </summary>
        private Mock<IRunnerConfiguration> _mockConfig;

        /// <summary>
        /// Defines a mocked model.
        /// </summary>
        private Mock<IApplicationModel> _mockModel;

        #region Before Each Scenario

        /// <summary>
        /// Sets up state before each scenario.
        /// </summary>
        [Background]
        public void Setup()
        {
            "Given a new mock configuration"
                .x(() => _mockConfig = new Mock<IRunnerConfiguration>());

            "Given a new mock model"
                .x(() => _mockModel = new Mock<IApplicationModel>());
        }

        #endregion

        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null config is passed.
        /// </summary>
        /// <param name="state">The runner state.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullConfig(IRunState state, IRunnerConfiguration config, Exception e)
        {
            "Given the runner state"
                .x(() => state.Should().BeNull());

            "And null runner configuration"
                .x(() => config.Should().BeNull());

            "When constructing with null runner configuration"
                .x(() => e = Record.Exception(() => new RunState(config)));

            "Then the runner state constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("config"));
        }

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null config is passed.
        /// </summary>
        /// <param name="state">The runner state.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="model">The model state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithModelAndNullConfig(IRunState state, IRunnerConfiguration config, IApplicationModel model, Exception e)
        {
            "Given the runner state"
                .x(() => state.Should().BeNull());

            "And null runner configuration"
                .x(() => config.Should().BeNull());

            "And null model state"
                .x(() => model = _mockModel.Object);

            "When constructing with null model state"
                .x(() => e = Record.Exception(() => new RunState(config, model)));

            "Then the runner state constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("config"));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when null model is passed.
        /// </summary>
        /// <param name="state">The runner state.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="model">The model state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullModel(IRunState state, IRunnerConfiguration config, IApplicationModel model, Exception e)
        {
            "Given the runner state"
                .x(() => state.Should().BeNull());

            "And runner configuration"
                .x(() => config = _mockConfig.Object);

            "And null model state"
                .x(() => model.Should().BeNull());

            "When constructing with null model state"
                .x(() => e = Record.Exception(() => state = new RunState(config, model)));

            "Then the runner state constructor should throw an exception"
                .x(() => e.Should().BeNull());

            "And the config should be available"
                .x(() => state.Configuration.Should().NotBeNull().And.BeSameAs(config));

            "And the model should be null"
                .x(() => state.Model.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="state">The runner state.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithJustConfigWithSuccess(IRunState state, IRunnerConfiguration config, Exception e)
        {
            "Given the runner state"
                .x(() => state.Should().BeNull());

            "And the runner configuration"
                .x(() => config = _mockConfig.Object);

            "When constructing the runner state"
                .x(() => e = Record.Exception(() => state = new RunState(config)));

            "Then the runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the config should be available"
                .x(() => state.Configuration.Should().NotBeNull().And.BeSameAs(config));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="state">The runner state.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="model">The model state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithConfigAndModelWithSuccess(IRunState state, IRunnerConfiguration config, IApplicationModel model, Exception e)
        {
            "Given the runner state"
                .x(() => state.Should().BeNull());

            "And the runner configuration"
                .x(() => config = _mockConfig.Object);

            "And the model state"
                .x(() => model = _mockModel.Object);

            "When constructing the runner state"
                .x(() => e = Record.Exception(() => state = new RunState(config, model)));

            "Then the runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the config should be available"
                .x(() => state.Configuration.Should().NotBeNull().And.BeSameAs(config));

            "And the model state should be available"
                .x(() => state.Model.Should().NotBeNull().And.BeSameAs(model));
        }

        #endregion
    }
}
