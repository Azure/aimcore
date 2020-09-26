using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AzureIntegrationMigration.Runner.Builder;
using Microsoft.AzureIntegrationMigration.Runner.Configuration;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xbehave;
using Xunit;

namespace Microsoft.AzureIntegrationMigration.Runner.Tests
{
    /// <summary>
    /// Defines the test spec for the <see cref="RunnerBuilder"/> class.
    /// </summary>
    public class RunnerBuilderFeature
    {
        /// <summary>
        /// Defines a mocked stage component provider.
        /// </summary>
        private Mock<IStageComponentProvider> _mockProvider;

        /// <summary>
        /// Defines a mocked list of stage runners.
        /// </summary>
        private List<Mock<IStageRunner>> _mockStageRunners;

        /// <summary>
        /// Defines a mocked logger object.
        /// </summary>
        private Mock<ILogger<Engine.Runner>> _mockLogger;

        /// <summary>
        /// Defines a mocked model object.
        /// </summary>
        private Mock<IApplicationModel> _mockModel;

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
                    discoverer.SetupProperty(r => r.Priority);
                    discoverer.SetupProperty(r => r.Skip);

                    var parser = new Mock<IStageRunner>();
                    parser.SetupGet(r => r.Stages).Returns(Stages.Parse);
                    parser.SetupProperty(r => r.Priority);
                    parser.SetupProperty(r => r.Skip);

                    var analyzer = new Mock<IStageRunner>();
                    analyzer.SetupGet(r => r.Stages).Returns(Stages.Parse);
                    analyzer.SetupProperty(r => r.Priority);
                    analyzer.SetupProperty(r => r.Skip);

                    var reporter = new Mock<IStageRunner>();
                    reporter.SetupGet(r => r.Stages).Returns(Stages.Report);
                    reporter.SetupProperty(r => r.Priority);
                    reporter.SetupProperty(r => r.Skip);

                    _mockStageRunners = new List<Mock<IStageRunner>>() { discoverer, parser, analyzer, reporter };
                });

            "Given a new mock stage component provider"
                .x(() =>
                {
                    _mockProvider = new Mock<IStageComponentProvider>();
                    _mockProvider.Setup(p => p.FindComponents(It.IsAny<IRunnerConfiguration>())).Returns(_mockStageRunners.Select(r => r.Object));
                });

            "Given a new mock logger"
                .x(() => _mockLogger = new Mock<ILogger<Engine.Runner>>());

            "Given a new mock model"
                .x(() => _mockModel = new Mock<IApplicationModel>());
        }

        #endregion

        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder.Should().BeNull());

            "When constructing the runner builder"
                .x(() => e = Record.Exception(() => builder = new RunnerBuilder()));

            "Then the runner builder constructor should succeed"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region FindStageRunners Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null provider is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="provider">The stage component provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void FindStageRunnersWithNullProvider(IRunnerBuilder builder, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null provider"
                .x(() => provider.Should().BeNull());

            "When finding stage runners using a provider"
                .x(() => e = Record.Exception(() => builder.FindStageRunners(provider)));

            "Then the find method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("provider"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a provider is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="provider">The stage component provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void FindStageRunnersWithSuccess(IRunnerBuilder builder, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider = _mockProvider.Object);

            "When finding stage runners using a provider"
                .x(() => e = Record.Exception(() => builder.FindStageRunners(provider)));

            "Then the find method should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the method succeeds if more than one provider is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="provider1">The first stage component provider.</param>
        /// <param name="provider2">The second stage component provider.</param>
        /// <param name="e1">The runner builder exception, if any.</param>
        /// <param name="e2">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void FindStageRunnersWithTwoProvidersWithSuccess(IRunnerBuilder builder, IStageComponentProvider provider1, IStageComponentProvider provider2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a first stage component provider"
                .x(() => provider1 = _mockProvider.Object);

            "And a second stage component provider"
                .x(() => provider2 = new Mock<IStageComponentProvider>().Object);

            "When finding stage runners using a provider"
                .x(() => e1 = Record.Exception(() => builder.FindStageRunners(provider1)));

            "And finding stage runners using a second instance of a provider"
                .x(() => e2 = Record.Exception(() => builder.FindStageRunners(provider2)));

            "Then the first find method should succeed"
                .x(() => e1.Should().BeNull());

            "And the second find method should succeed"
                .x(() => e2.Should().BeNull());
        }

        #endregion

        #region AddStageRunner Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null runner is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="runner">The stage runner.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void AddStageRunnerWithNullRunner(IRunnerBuilder builder, IStageRunner runner, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null runner"
                .x(() => runner.Should().BeNull());

            "When adding a stage runner"
                .x(() => e = Record.Exception(() => builder.AddStageRunner(runner)));

            "Then the add method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("runner"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a runner is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="runner">The stage runner.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void AddStageRunnerWithSuccess(IRunnerBuilder builder, IStageRunner runner, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage runner"
                .x(() => runner = _mockStageRunners.First().Object);

            "When adding a stage runner"
                .x(() => e = Record.Exception(() => builder.AddStageRunner(runner)));

            "Then the add method should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the method succeeds if two runners are supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="runner1">The first stage runner.</param>
        /// <param name="runner2">The second stage runner.</param>
        /// <param name="e1">The runner builder exception, if any.</param>
        /// <param name="e2">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void AddTwoStageRunnersWithSuccess(IRunnerBuilder builder, IStageRunner runner1, IStageRunner runner2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a first stage runner"
                .x(() => runner1 = _mockStageRunners.First().Object);

            "And a second stage runner"
                .x(() => runner2 = _mockStageRunners.Last().Object);

            "When adding first stage runner"
                .x(() => e1 = Record.Exception(() => builder.AddStageRunner(runner1)));

            "And adding second stage runner"
                .x(() => e2 = Record.Exception(() => builder.AddStageRunner(runner2)));

            "Then the first add method should succeed"
                .x(() => e1.Should().BeNull());

            "And the second add method should succeed"
                .x(() => e2.Should().BeNull());
        }

        #endregion

        #region SetStageRunnerPriority Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null delegate is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="priorityFunc">The priority function.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetStageRunnerPriorityWithNullDelegate(IRunnerBuilder builder, Func<IStageRunner, int> priorityFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null priority function"
                .x(() => priorityFunc.Should().BeNull());

            "When setting stage runner priority with a deferred delegate"
                .x(() => e = Record.Exception(() => builder.SetStageRunnerPriority(priorityFunc)));

            "Then the set method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("priorityFunc"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a delegate is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="priorityFunc">The priority function.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetStageRunnerPriorityWithSuccess(IRunnerBuilder builder, Func<IStageRunner, int> priorityFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a priority function"
                .x(() => priorityFunc = r => 999);

            "When setting stage runner priority with a deferred delegate"
                .x(() => e = Record.Exception(() => builder.SetStageRunnerPriority(priorityFunc)));

            "Then the set method should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the method succeeds if two delegates are supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="priorityFunc1">The first priority function.</param>
        /// <param name="priorityFunc2">The second priority function.</param>
        /// <param name="e1">The runner builder exception, if any.</param>
        /// <param name="e2">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetStageRunnerPriorityTwiceWithSuccess(IRunnerBuilder builder, Func<IStageRunner, int> priorityFunc1, Func<IStageRunner, int> priorityFunc2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a first priority function"
                .x(() => priorityFunc1 = r => (r.Stages & Stages.Discover) == Stages.Discover ? 10 : 0);

            "And a second priority function"
                .x(() => priorityFunc2 = r => (r.Stages & Stages.Parse) == Stages.Parse ? 20 : 0);

            "When setting first stage runner priority with a deferred delegate"
                .x(() => e1 = Record.Exception(() => builder.SetStageRunnerPriority(priorityFunc1)));

            "And setting second stage runner priority with a deferred delegate"
                .x(() => e2 = Record.Exception(() => builder.SetStageRunnerPriority(priorityFunc2)));

            "Then the first set method should succeed"
                .x(() => e1.Should().BeNull());

            "And the second set method should succeed"
                .x(() => e2.Should().BeNull());
        }

        #endregion

        #region SkipStageRunner Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null delegate is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="skipFunc">The skip predicate.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SkipStageRunnerWithNullDelegate(IRunnerBuilder builder, Predicate<IStageRunner> skipFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null skip function"
                .x(() => skipFunc.Should().BeNull());

            "When skipping stage runner with a deferred delegate"
                .x(() => e = Record.Exception(() => builder.SkipStageRunner(skipFunc)));

            "Then the skip method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("skipFunc"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a delegate is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="skipFunc">The skip predicate.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SkipStageRunnerWithSuccess(IRunnerBuilder builder, Predicate<IStageRunner> skipFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a skip function"
                .x(() => skipFunc = r => (r.Stages & Stages.Report) == Stages.Report ? true : false);

            "When skipping stage runner with a deferred delegate"
                .x(() => e = Record.Exception(() => builder.SkipStageRunner(skipFunc)));

            "Then the skip method should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that the method succeeds if two delegates are supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="skipFunc1">The first skip function.</param>
        /// <param name="skipFunc2">The second skip function.</param>
        /// <param name="e1">The runner builder exception, if any.</param>
        /// <param name="e2">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SkipStageRunnerTwiceWithSuccess(IRunnerBuilder builder, Predicate<IStageRunner> skipFunc1, Predicate<IStageRunner> skipFunc2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a first skip function"
                .x(() => skipFunc1 = r => (r.Stages & Stages.Discover) == Stages.Analyze ? true : false);

            "And a second skip function"
                .x(() => skipFunc2 = r => (r.Stages & Stages.Parse) == Stages.Report ? true : false);

            "When skipping first stage runner with a deferred delegate"
                .x(() => e1 = Record.Exception(() => builder.SkipStageRunner(skipFunc1)));

            "And skipping second stage runner with a deferred delegate"
                .x(() => e2 = Record.Exception(() => builder.SkipStageRunner(skipFunc2)));

            "Then the first skip method should succeed"
                .x(() => e1.Should().BeNull());

            "And the second skip method should succeed"
                .x(() => e2.Should().BeNull());
        }

        #endregion

        #region DisableStage Scenarios

        /// <summary>
        /// Scenario tests that disabling the None stage succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void DisableNoneStageSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When disabling the None stage"
                .x(() => e = Record.Exception(() => builder.DisableStage(Stages.None)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that disabling a single stage succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void DisableSingleStageSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When disabling a single stage"
                .x(() => e = Record.Exception(() => builder.DisableStage(Stages.Discover)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that disabling two stages succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void DisableTwoStagesSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When disabling a stage"
                .x(() => e = Record.Exception(() => builder.DisableStage(Stages.Discover)));

            "And disabling a second stage"
                .x(() => e = Record.Exception(() => builder.DisableStage(Stages.Parse)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that disabling all stages succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void DisableAllStagesSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When disabling all stages"
                .x(() => e = Record.Exception(() => builder.DisableStage(Stages.All)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region EnableFailFast Scenarios

        /// <summary>
        /// Scenario tests that enabling the flag to fail fast succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailFastSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When enabling the fail fast flag"
                .x(() => e = Record.Exception(() => builder.EnableFailFast()));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region EnableFailStage Scenarios

        /// <summary>
        /// Scenario tests that enabling the fail fast flag with the None stage succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailStageWithNoneStageSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When enabling the fail fast flag for the None stage"
                .x(() => e = Record.Exception(() => builder.EnableFailStage(Stages.None)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that enabling the fail fast flag for a single stage succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailStageWithSingleStageSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When enabling the fail fast flag for a single stage"
                .x(() => e = Record.Exception(() => builder.EnableFailStage(Stages.Discover)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that enabling the fail fast flag for a single stage succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailStageWithTwoStagesSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When enabling the fail fast flag for a single stage"
                .x(() => e = Record.Exception(() => builder.EnableFailStage(Stages.Discover)));

            "And enabling the fail fast flag for a second stage"
                .x(() => e = Record.Exception(() => builder.EnableFailStage(Stages.Parse)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that enabling the fail fast flag for all stages succeeds.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void EnableFailStageWithAllStagesSucceeds(IRunnerBuilder builder, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "When enabling the fail fast flag for all stages"
                .x(() => e = Record.Exception(() => builder.EnableFailStage(Stages.All)));

            "Then no exception occurs"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region SetArgs Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null set of arguments is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetArgsWithNullDictionary(IRunnerBuilder builder, IDictionary<string, object> args, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null arguments dictionary"
                .x(() => args.Should().BeNull());

            "When setting arguments"
                .x(() => e = Record.Exception(() => builder.SetArgs(args)));

            "Then the set method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("args"));
        }

        /// <summary>
        /// Scenario tests that a set of arguments can be added to the builder.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetArgsWithSuccess(IRunnerBuilder builder, IDictionary<string, object> args, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a set of arguments in a dictionary"
                .x(() => args = new Dictionary<string, object>() { { "arg1", "val1" }, { "arg2", "val2" } });

            "When setting arguments"
                .x(() => e = Record.Exception(() => builder.SetArgs(args)));

            "Then the set method should succeed"
                .x(() => e.Should().BeNull());
        }

        /// <summary>
        /// Scenario tests that a set of arguments can be added twice to the builder.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="args1">The first set of arguments.</param>
        /// <param name="args2">The second set of arguments.</param>
        /// <param name="e1">The first runner builder exception, if any.</param>
        /// <param name="e2">The second runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetArgsTwiceWithSuccess(IRunnerBuilder builder, IDictionary<string, object> args1, IDictionary<string, object> args2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a set of arguments in a dictionary"
                .x(() => args1 = new Dictionary<string, object>() { { "arg1", "val1" }, { "arg2", "val2" } });

            "And a second set of arguments in a dictionary"
                .x(() => args2 = new Dictionary<string, object>() { { "arg3", "val3" }, { "arg4", "val4" } });

            "When setting the first set of arguments"
                .x(() => e1 = Record.Exception(() => builder.SetArgs(args1)));

            "And setting the second set of arguments"
                .x(() => e2 = Record.Exception(() => builder.SetArgs(args2)));

            "Then both set methods should succeed"
                .x(() =>
                {
                    e1.Should().BeNull();
                    e2.Should().BeNull();
                });
        }

        /// <summary>
        /// Scenario tests that the same set of arguments can be added twice to the builder.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="args1">The first set of arguments.</param>
        /// <param name="args2">The second set of arguments.</param>
        /// <param name="e1">The first runner builder exception, if any.</param>
        /// <param name="e2">The second runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetSameArgsTwiceWithSuccess(IRunnerBuilder builder, IDictionary<string, object> args1, IDictionary<string, object> args2, Exception e1, Exception e2)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a set of arguments in a dictionary"
                .x(() => args1 = new Dictionary<string, object>() { { "arg1", "val1" }, { "arg2", "val2" } });

            "And a second set of the same arguments in a dictionary"
                .x(() => args2 = new Dictionary<string, object>() { { "arg1", "val1" }, { "arg2", "val2" } });

            "When setting the first set of arguments"
                .x(() => e1 = Record.Exception(() => builder.SetArgs(args1)));

            "And setting the second set of arguments"
                .x(() => e2 = Record.Exception(() => builder.SetArgs(args2)));

            "Then both set methods should succeed"
                .x(() =>
                {
                    e1.Should().BeNull();
                    e2.Should().BeNull();
                });
        }

        #endregion

        #region SetLogger Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null logger is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetLoggerWithNullLogger(IRunnerBuilder builder, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null logger"
                .x(() => logger.Should().BeNull());

            "When setting the logger"
                .x(() => e = Record.Exception(() => builder.SetLogger(logger)));

            "Then the set method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("logger"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a logger is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetLoggerWithSuccess(IRunnerBuilder builder, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When setting the logger"
                .x(() => e = Record.Exception(() => builder.SetLogger(logger)));

            "Then the set method should succeed"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region SetModel Scenarios

        /// <summary>
        /// Scenario tests that the method throws an exception if a null model is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="modelFunc">The model function.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetModelWithNullModel(IRunnerBuilder builder, Func<IApplicationModel> modelFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a null model function"
                .x(() => modelFunc.Should().BeNull());

            "When setting the model"
                .x(() => e = Record.Exception(() => builder.SetModel(modelFunc)));

            "Then the set method should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("modelFunc"));
        }

        /// <summary>
        /// Scenario tests that the method succeeds if a model is supplied.
        /// </summary>
        /// <param name="builder">The runner builder.</param>
        /// <param name="modelFunc">The model function.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SetModelWithSuccess(IRunnerBuilder builder, Func<IApplicationModel> modelFunc, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a model function"
                .x(() => modelFunc = () => _mockModel.Object);

            "When setting the model"
                .x(() => e = Record.Exception(() => builder.SetModel(modelFunc)));

            "Then the set method should succeed"
                .x(() => e.Should().BeNull());
        }

        #endregion

        #region Build Scenarios

        /// <summary>
        /// Scenario tests that the builder builds a runner when using a stage component provider
        /// to find stage runners.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="runner">The runner.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void BuildWithStageComponentProvider(IRunnerBuilder builder, IRunner runner, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider = _mockProvider.Object);

            "When building the runner"
                .x(() => e = Record.Exception(() => runner = builder.FindStageRunners(provider).Build()));

            "Then the build method should succeed"
                .x(() => e.Should().BeNull());

            "And there should be 4 stage runners found in config"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull();
                    runner.RunState.Configuration.StageRunners.Should().NotBeNull().And.Subject.Should().HaveCount(4);
                });
        }

        /// <summary>
        /// Scenario tests that the builder builds a runner when using two stage component providers
        /// to find stage runners.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="runner">The runner.</param>
        /// <param name="provider1">The first provider.</param>
        /// <param name="provider2">The second provider.</param>
        /// <param name="e">The first runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void BuildWithStageComponentProviderTwice(IRunnerBuilder builder, IRunner runner, IStageComponentProvider provider1, IStageComponentProvider provider2, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider1 = _mockProvider.Object);

            "And a second stage component provider"
                .x(() => provider2 = _mockProvider.Object);

            "When building the runner"
                .x(() => e = Record.Exception(() => runner = builder.FindStageRunners(provider1).FindStageRunners(provider2).Build()));

            "Then the build method should succeed"
                .x(() => e.Should().BeNull());

            "And there should be 8 stage runners found in config"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull();
                    runner.RunState.Configuration.StageRunners.Should().NotBeNull().And.Subject.Should().HaveCount(8);
                });
        }

        /// <summary>
        /// Scenario tests that the builder builds a runner when using a stage component provider
        /// to find stage runners and priority function that changes runner priorities.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="runner">The runner.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void BuildWithPriorityFunction(IRunnerBuilder builder, IRunner runner, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider = _mockProvider.Object);

            "When building the runner"
                .x(() => e = Record.Exception(() => runner = builder.FindStageRunners(provider).SetStageRunnerPriority(r => (r.Stages & Stages.Discover) == Stages.Discover ? 100 : 10).Build()));

            "Then the build method should succeed"
                .x(() => e.Should().BeNull());

            "And there should be 4 stage runners found in config with correct priorities"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull();
                    runner.RunState.Configuration.StageRunners.Should().NotBeNull().And.Subject.Should().HaveCount(4);
                    runner.RunState.Configuration.StageRunners.Where(r => r.Priority == 100 && (r.Stages & Stages.Discover) == Stages.Discover).Should().HaveCount(1);
                    runner.RunState.Configuration.StageRunners.Where(r => r.Priority == 10 && (r.Stages & Stages.Discover) != Stages.Discover).Should().HaveCount(3);
                });
        }

        /// <summary>
        /// Scenario tests that the builder builds a runner when using a stage component provider
        /// to find stage runners and skip function that sets the skip flag.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="runner">The runner.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void BuildWithSkipFunction(IRunnerBuilder builder, IRunner runner, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider = _mockProvider.Object);

            "When building the runner"
                .x(() => e = Record.Exception(() => runner = builder.FindStageRunners(provider).SkipStageRunner(r => (r.Stages & Stages.Discover) == Stages.Discover ? true : false).Build()));

            "Then the build method should succeed"
                .x(() => e.Should().BeNull());

            "And the Discover stage runner only should be set to skip execution"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull();
                    runner.RunState.Configuration.StageRunners.Should().NotBeNull().And.Subject.Should().HaveCount(4);
                    runner.RunState.Configuration.StageRunners.Where(r => r.Skip && (r.Stages & Stages.Discover) == Stages.Discover).Should().HaveCount(1);
                    runner.RunState.Configuration.StageRunners.Where(r => !r.Skip && (r.Stages & Stages.Discover) != Stages.Discover).Should().HaveCount(3);
                });
        }

        /// <summary>
        /// Scenario tests that the builder builds a runner when using a stage component provider
        /// to find stage runners and model function that sets the application model.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="runner">The runner.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="e">The runner builder exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void BuildWithModelFunction(IRunnerBuilder builder, IRunner runner, IStageComponentProvider provider, Exception e)
        {
            "Given a runner builder"
                .x(() => builder = new RunnerBuilder());

            "And a stage component provider"
                .x(() => provider = _mockProvider.Object);

            "When building the runner"
                .x(() => e = Record.Exception(() => runner = builder.FindStageRunners(provider).SetModel(() => _mockModel.Object).Build()));

            "Then the build method should succeed"
                .x(() => e.Should().BeNull());

            "And the model should have been set by the model function"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull();
                    runner.RunState.Model.Should().BeEquivalentTo(_mockModel.Object);
                });
        }

        #endregion
    }
}
