// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AzureIntegrationMigration.Runner.Configuration;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xbehave;
using Xunit;

namespace Microsoft.AzureIntegrationMigration.Runner.Tests
{
    /// <summary>
    /// Defines the test spec for the <see cref="RunnerConfiguration"/> class.
    /// </summary>
    public class RunnerConfigurationFeature
    {
        /// <summary>
        /// Defines a mocked list of stage runners.
        /// </summary>
        private List<Mock<IStageRunner>> _mockStageRunners;

        #region Before Each Scenario

        /// <summary>
        /// Sets up state before each scenario.
        /// </summary>
        [Background]
        public void Setup()
        {
            "Given a new collection of mock stage runners"
                .x(() =>
                {
                    var discoverer = new Mock<IStageRunner>();
                    discoverer.SetupGet(r => r.Stages).Returns(Stages.Discover);

                    var parser = new Mock<IStageRunner>();
                    parser.SetupGet(r => r.Stages).Returns(Stages.Parse);

                    var analyzer = new Mock<IStageRunner>();
                    analyzer.SetupGet(r => r.Stages).Returns(Stages.Parse);

                    var reporter = new Mock<IStageRunner>();
                    reporter.SetupGet(r => r.Stages).Returns(Stages.Report);

                    _mockStageRunners = new List<Mock<IStageRunner>>(){ discoverer, parser, analyzer, reporter };
                });
        }

        #endregion

        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null stage runners is passed.
        /// </summary>
        /// <param name="stageRunners">The stage runners.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullStageRunners(IList<IStageRunner> stageRunners, IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config.Should().BeNull());

            "When constructing with null stage runners"
                .x(() => e = Record.Exception(() => new RunnerConfiguration(stageRunners)));

            "Then the runner configuration constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("stageRunners"));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config.Should().BeNull());

            "When constructing the runner configuration"
                .x(() => e = Record.Exception(() => config = new RunnerConfiguration()));

            "Then the runner configuration constructor should succeed"
                .x(() => e.Should().BeNull());

            "And configuration should be available"
                .x(() =>
                {
                    config.Stages.Should().Be(Stages.All);
                    config.StageRunners.Should().NotBeNull().And.HaveCount(0);
                    config.Args.Count.Should().Be(0);
                    config.FailFast.Should().BeFalse();
                    config.FailStages.Should().Be(Stages.None);
                });
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds with a list of stage runners.
        /// </summary>
        /// <param name="stageRunners">The list of stage runners.</param>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithStageRunnersSuccess(IList<IStageRunner> stageRunners, IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config.Should().BeNull());

            "And a list of stage runners"
                .x(() => stageRunners = _mockStageRunners.ConvertAll(m => m.Object));

            "When constructing the runner configuration"
                .x(() => e = Record.Exception(() => config = new RunnerConfiguration(stageRunners)));

            "Then the runner configuration constructor should succeed"
                .x(() => e.Should().BeNull());

            "And configuration should be available"
                .x(() =>
                {
                    config.Stages.Should().Be(Stages.All);
                    config.StageRunners.Should().NotBeNull().And.HaveCount(4);
                    config.Args.Count.Should().Be(0);
                    config.FailFast.Should().BeFalse();
                    config.FailStages.Should().Be(Stages.None);
                });
        }

        #endregion

        #region FailFast Scenarios

        /// <summary>
        /// Scenario tests that the FailFast property can be set successfully.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailFast(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config = new RunnerConfiguration());

            "When setting the FailFast property to True"
                .x(() => e = Record.Exception(() => config.FailFast = true));

            "Then the property set should succeed"
                .x(() => e.Should().BeNull());

            "And the FailFast property should be set to True"
                .x(() => config.FailFast.Should().BeTrue());
        }

        #endregion

        #region FailStages Scenarios

        /// <summary>
        /// Scenario tests that a single stage can be enabled successfully.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetFailStageWithSingleStage(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config = new RunnerConfiguration());

            "When enabling the fail fast flag for a single stage"
                .x(() => e = Record.Exception(() => config.FailStages |= Stages.Verify));

            "Then enabling the flag should succeed"
                .x(() => e.Should().BeNull());

            "And the FailStages property should have the single stage enabled"
                .x(() =>
                {
                    config.FailStages.Should().HaveFlag(Stages.Verify);
                    config.FailStages.Should().NotHaveFlag(Stages.Discover);
                    config.FailStages.Should().NotHaveFlag(Stages.Parse);
                    config.FailStages.Should().NotHaveFlag(Stages.Analyze);
                    config.FailStages.Should().NotHaveFlag(Stages.Report);
                    config.FailStages.Should().NotHaveFlag(Stages.Convert);
                });
        }

        /// <summary>
        /// Scenario tests that all stages can be enabled successfully.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetFailStageWithAllStages(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config = new RunnerConfiguration());

            "When enabling the fail fast flag for all stages"
                .x(() => e = Record.Exception(() => config.Stages |= Stages.All));

            "Then enabling all flags should succeed"
                .x(() => e.Should().BeNull());

            "And the FailStages property should have all stages enabled"
                .x(() => config.Stages.Should().HaveFlag(Stages.All));
        }

        #endregion

        #region Stages Scenarios

        /// <summary>
        /// Scenario tests that the Stages property can be set successfully.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetStage(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config = new RunnerConfiguration());

            "When setting the Stages property to Assessment"
                .x(() => e = Record.Exception(() => config.Stages = Stages.Assessment));

            "Then the property set should succeed"
                .x(() => e.Should().BeNull());

            "And the Stages property should be set to Assessment"
                .x(() => config.Stages.Should().Be(Stages.Assessment));
        }

        /// <summary>
        /// Scenario tests that a stage can be disabled successfully.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <param name="e">The runner configuration exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void DisableStage(IRunnerConfiguration config, Exception e)
        {
            "Given runner configuration"
                .x(() => config = new RunnerConfiguration());

            "When disabling the Verify stage"
                .x(() => e = Record.Exception(() => config.Stages -= Stages.Verify));

            "Then the property set should succeed"
                .x(() => e.Should().BeNull());

            "And the Stages property should not have the Verify stage enabled"
                .x(() => config.Stages.Should().NotHaveFlag(Stages.Verify));
        }

        #endregion
    }
}
