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
    /// Defines the test spec for the <see cref="StageRunnerState"/> class.
    /// </summary>
    public class StageRunnerStateFeature
    {
        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state"
                .x(() => e = Record.Exception(() => state = new StageRunnerState()));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region Property Get Scenarios

        /// <summary>
        /// Scenario tests that the current flag can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetIsCurrentWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state with a current flag value"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { IsCurrent = true }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the current flag should be set to the expected value"
                .x(() => state.IsCurrent.Should().BeTrue());
        }

        /// <summary>
        /// Scenario tests that the state can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStateWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state with a state value"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { State = State.Ready }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the state should be set to the expected value"
                .x(() => state.State.Should().Be(State.Ready));
        }

        /// <summary>
        /// Scenario tests that the error can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetErrorWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state with an error value"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { Error = new RunnerException("an error message") }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the error should be set to the expected value"
                .x(() => state.Error.Should().NotBeNull().And.BeOfType<RunnerException>().Which.Message.Should().Be("an error message"));
        }

        /// <summary>
        /// Scenario tests that the started timestamp can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStartedTimestampWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state with a started timestamp"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { Started = DateTimeOffset.MaxValue }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the started timestamp should be set to the expected value"
                .x(() => state.Started.Should().Be(DateTimeOffset.MaxValue));
        }

        /// <summary>
        /// Scenario tests that the completed timestamp can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetCompletedTimestampWithSuccess(StageRunnerState state, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "When constructing the stage runner state with a completed timestamp"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { Completed = DateTimeOffset.MaxValue }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the completed timestamp should be set to the expected value"
                .x(() => state.Completed.Should().Be(DateTimeOffset.MaxValue));
        }

        /// <summary>
        /// Scenario tests that the stage runner can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage runner state.</param>
        /// <param name="parser">The parser stage runner.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStageRunnerWithSuccess(StageRunnerState state, IStageRunner parser, Exception e)
        {
            "Given the stage runner state"
                .x(() => state.Should().BeNull());

            "And a mocked parser stage runner"
                .x(() =>
                {
                    var parserMock = new Mock<IStageRunner>();
                    parserMock.SetupGet(r => r.Stages).Returns(Stages.Parse);
                    parser = parserMock.Object;
                });

            "When constructing the stage runner state with the mocked stage runner"
                .x(() => e = Record.Exception(() => state = new StageRunnerState() { StageRunner = parser }));

            "Then the stage runner state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the stage runner should be set to the expected value"
                .x(() =>
                {
                    state.StageRunner.Should().NotBeNull();
                    state.StageRunner.Stages.Should().HaveFlag(Stages.Parse);
                    state.StageRunner.Stages.Should().NotHaveFlag(Stages.Discover);
                    state.StageRunner.Stages.Should().NotHaveFlag(Stages.Analyze);
                    state.StageRunner.Stages.Should().NotHaveFlag(Stages.Report);
                    state.StageRunner.Stages.Should().NotHaveFlag(Stages.Convert);
                    state.StageRunner.Stages.Should().NotHaveFlag(Stages.Verify);
                });
        }

        #endregion
    }
}
