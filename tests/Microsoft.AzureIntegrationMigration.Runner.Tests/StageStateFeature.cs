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
    /// Defines the test spec for the <see cref="StageState"/> class.
    /// </summary>
    public class StageStateFeature
    {
        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state"
                .x(() => e = Record.Exception(() => state = new StageState()));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region Property Get Scenarios

        /// <summary>
        /// Scenario tests that the current flag can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetIsCurrentWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state with a current flag value"
                .x(() => e = Record.Exception(() => state = new StageState() { IsCurrent = true }));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the current flag should be set to the expected value"
                .x(() => state.IsCurrent.Should().BeTrue());
        }

        /// <summary>
        /// Scenario tests that the stage can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStageWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state with a stage value"
                .x(() => e = Record.Exception(() => state = new StageState() { Stage = Stages.Discover }));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the stage should be set to the expected value"
                .x(() =>
                {
                    state.Stage.Should().HaveFlag(Stages.Discover);
                    state.Stage.Should().NotHaveFlag(Stages.Parse);
                    state.Stage.Should().NotHaveFlag(Stages.Analyze);
                    state.Stage.Should().NotHaveFlag(Stages.Report);
                    state.Stage.Should().NotHaveFlag(Stages.Convert);
                    state.Stage.Should().NotHaveFlag(Stages.Verify);
                });
        }

        /// <summary>
        /// Scenario tests that the state can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStateWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state with a state value"
                .x(() => e = Record.Exception(() => state = new StageState() { State = State.Ready }));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the state should be set to the expected value"
                .x(() => state.State.Should().Be(State.Ready));
        }

        /// <summary>
        /// Scenario tests that the started timestamp can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStartedTimestampWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state with a started timestamp"
                .x(() => e = Record.Exception(() => state = new StageState() { Started = DateTimeOffset.MaxValue }));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the started timestamp should be set to the expected value"
                .x(() => state.Started.Should().Be(DateTimeOffset.MaxValue));
        }

        /// <summary>
        /// Scenario tests that the completed timestamp can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetCompletedTimestampWithSuccess(StageState state, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "When constructing the stage state with a completed timestamp"
                .x(() => e = Record.Exception(() => state = new StageState() { Completed = DateTimeOffset.MaxValue }));

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the completed timestamp should be set to the expected value"
                .x(() => state.Completed.Should().Be(DateTimeOffset.MaxValue));
        }

        /// <summary>
        /// Scenario tests that the stage runner execution state collection can be retrieved successfully.
        /// </summary>
        /// <param name="state">The stage execution state.</param>
        /// <param name="discoverer">The discover stage runner.</param>
        /// <param name="parser">The parser stage runner.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void GetStageRunnerExecutionStateWithSuccess(StageState state, IStageRunner discoverer, IStageRunner parser, Exception e)
        {
            "Given the stage state"
                .x(() => state.Should().BeNull());

            "And a mocked discoverer stage runner"
                .x(() =>
                {
                    var discovererMock = new Mock<IStageRunner>();
                    discovererMock.SetupGet(r => r.Stages).Returns(Stages.Discover);
                    discoverer = discovererMock.Object;
                });

            "And a mocked parser stage runner"
                .x(() =>
                {
                    var parserMock = new Mock<IStageRunner>();
                    parserMock.SetupGet(r => r.Stages).Returns(Stages.Parse);
                    parser = parserMock.Object;
                });

            "When constructing the stage state with state runner execution state"
                .x(() =>
                {
                    e = Record.Exception(() =>
                    {
                        state = new StageState();
                        state.ExecutionState.Add(new StageRunnerState() { State = State.Running, Started = DateTimeOffset.MaxValue, StageRunner = discoverer });
                        state.ExecutionState.Add(new StageRunnerState() { State = State.Ready, Started = DateTimeOffset.MaxValue, StageRunner = parser });
                    });
                });

            "Then the stage state constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the stage runners should be set to the expected value"
                .x(() =>
                {
                    state.ExecutionState.Should().NotBeNull().And.HaveCount(2);
                    state.ExecutionState[0].Should().NotBeNull();
                    state.ExecutionState[0].State.Should().Be(State.Running);
                    state.ExecutionState[0].Started.Should().Be(DateTimeOffset.MaxValue);
                    state.ExecutionState[0].StageRunner.Should().NotBeNull();
                    state.ExecutionState[0].StageRunner.Stages.Should().HaveFlag(Stages.Discover);
                    state.ExecutionState[1].Should().NotBeNull();
                    state.ExecutionState[1].State.Should().Be(State.Ready);
                    state.ExecutionState[1].Started.Should().Be(DateTimeOffset.MaxValue);
                    state.ExecutionState[1].StageRunner.Should().NotBeNull();
                    state.ExecutionState[1].StageRunner.Stages.Should().HaveFlag(Stages.Parse);
                });
        }

        #endregion
    }
}
