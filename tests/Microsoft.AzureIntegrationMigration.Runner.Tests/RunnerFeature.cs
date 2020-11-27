// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xbehave;
using Xunit;

namespace Microsoft.AzureIntegrationMigration.Runner.Tests
{
    /// <summary>
    /// Defines the test spec for the <see cref="Engine.Runner"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is handled by xBehave.net and the background attribute.")]
    public class RunnerFeature
    {
        /// <summary>
        /// Defines a mocked configuration object.
        /// </summary>
        private Mock<IRunnerConfiguration> _mockConfig;

        /// <summary>
        /// Defines a mocked model object.
        /// </summary>
        private Mock<IApplicationModel> _mockModel;

        /// <summary>
        /// Defines a mocked logger object.
        /// </summary>
        private Mock<ILogger<Engine.Runner>> _mockLogger;

        /// <summary>
        /// Defines a mocked list of stage runners.
        /// </summary>
        private List<Mock<IStageRunner>> _mockStageRunners;

        /// <summary>
        /// Defines a cancellation token source that is used to create cancellation tokens for async method calls
        /// that support cancellation.
        /// </summary>
        private CancellationTokenSource _source;

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
                    discoverer.SetupGet(r => r.Name).Returns("MockDiscoverer");
                    discoverer.SetupGet(r => r.Stages).Returns(Stages.Discover);
                    discoverer.SetupProperty(r => r.Priority);
                    discoverer.SetupProperty(r => r.Skip);

                    var parser = new Mock<IStageRunner>();
                    parser.SetupGet(r => r.Name).Returns("MockParser");
                    parser.SetupGet(r => r.Stages).Returns(Stages.Parse);
                    parser.SetupProperty(r => r.Priority);
                    parser.SetupProperty(r => r.Skip);

                    var analyzer = new Mock<IStageRunner>();
                    analyzer.SetupGet(r => r.Name).Returns("MockAnalyzer");
                    analyzer.SetupGet(r => r.Stages).Returns(Stages.Analyze);
                    analyzer.SetupProperty(r => r.Priority);
                    analyzer.SetupProperty(r => r.Skip);

                    var reporter = new Mock<IStageRunner>();
                    reporter.SetupGet(r => r.Name).Returns("MockReporter");
                    reporter.SetupGet(r => r.Stages).Returns(Stages.Report);
                    reporter.SetupProperty(r => r.Priority);
                    reporter.SetupProperty(r => r.Skip);

                    var converter = new Mock<IStageRunner>();
                    converter.SetupGet(r => r.Name).Returns("MockConverter");
                    converter.SetupGet(r => r.Stages).Returns(Stages.Convert);
                    converter.SetupProperty(r => r.Priority);
                    converter.SetupProperty(r => r.Skip);

                    var verifier = new Mock<IStageRunner>();
                    verifier.SetupGet(r => r.Name).Returns("MockVerifier");
                    verifier.SetupGet(r => r.Stages).Returns(Stages.Verify);
                    verifier.SetupProperty(r => r.Priority);
                    verifier.SetupProperty(r => r.Skip);

                    _mockStageRunners = new List<Mock<IStageRunner>>() { discoverer, parser, analyzer, reporter, converter, verifier };
                });

            "Given a new mock runner configuration"
                .x(() =>
                {
                    _mockConfig = new Mock<IRunnerConfiguration>();
                    _mockConfig.SetupProperty(c => c.Stages);
                    _mockConfig.SetupProperty(c => c.FailStages);
                    _mockConfig.SetupProperty(c => c.FailFast);
                    _mockConfig.SetupGet(c => c.StageRunners).Returns(_mockStageRunners.Select(r => r.Object).ToList());
                });

            "Given a new mock model"
                .x(() => _mockModel = new Mock<IApplicationModel>());

            "Given a new mock logger"
                .x(() =>
                {
                    _mockLogger = new Mock<ILogger<Engine.Runner>>();
                    _mockLogger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
                });

            "Given a new cancellation token source"
                .x(() => _source = new CancellationTokenSource());
        }

        #endregion

        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null config is passed.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullConfig(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner"
                .x(() => runner.Should().BeNull());

            "And null configuration"
                .x(() => config.Should().BeNull());

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When constructing with null configuration"
                .x(() => e = Record.Exception(() => new Engine.Runner(config, model, logger)));

            "Then the runner constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("config"));
        }

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null config is passed.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNoModelAndNullConfig(IRunner runner, IRunnerConfiguration config, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner"
                .x(() => runner.Should().BeNull());

            "And null configuration"
                .x(() => config.Should().BeNull());

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When constructing with null configuration"
                .x(() => e = Record.Exception(() => new Engine.Runner(config, logger)));

            "Then the runner constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("config"));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds when null model is passed.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullModel(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner"
                .x(() => runner.Should().BeNull());

            "And runner configuration"
                .x(() => config = _mockConfig.Object);

            "And a null model"
                .x(() => model.Should().BeNull());

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When constructing with null model"
                .x(() => e = Record.Exception(() => runner = new Engine.Runner(config, model, logger)));

            "Then the runner constructor should succeed"
                .x(() => e.Should().BeNull());

            "And configuration should be available"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull().And.BeSameAs(config);
                });

            "And model should be null"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Model.Should().BeNull();
                });
        }

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null logger is passed.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullLogger(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner"
                .x(() => runner.Should().BeNull());

            "And runner configuration"
                .x(() => config = _mockConfig.Object);

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a null logger"
                .x(() => logger.Should().BeNull());

            "When constructing with null logger"
                .x(() => e = Record.Exception(() => new Engine.Runner(config, model, logger)));

            "Then the runner constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("logger"));
        }

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null logger is passed.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNoModelAndNullLogger(IRunner runner, IRunnerConfiguration config, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given a runner"
                .x(() => runner.Should().BeNull());

            "And runner configuration"
                .x(() => config = _mockConfig.Object);

            "And a null logger"
                .x(() => logger.Should().BeNull());

            "When constructing with null logger"
                .x(() => e = Record.Exception(() => new Engine.Runner(config, logger)));

            "Then the runner constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("logger"));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given runner configuration"
                .x(() => config = _mockConfig.Object);

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When constructing the runner"
                .x(() => e = Record.Exception(() => runner = new Engine.Runner(config, model, logger)));

            "Then the runner constructor should succeed"
                .x(() => e.Should().BeNull());

            "And configuration should be available"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull().And.BeSameAs(config);
                });

            "And model should be available"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Model.Should().NotBeNull().And.BeSameAs(model);
                });
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructNoModelWithSuccess(IRunner runner, IRunnerConfiguration config, ILogger<Engine.Runner> logger, Exception e)
        {
            "Given runner configuration"
                .x(() => config = _mockConfig.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "When constructing the runner"
                .x(() => e = Record.Exception(() => runner = new Engine.Runner(config, logger)));

            "Then the runner constructor should succeed"
                .x(() => e.Should().BeNull());

            "And configuration should be available"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Configuration.Should().NotBeNull().And.BeSameAs(config);
                });

            "And model should be null"
                .x(() =>
                {
                    runner.RunState.Should().NotBeNull();
                    runner.RunState.Model.Should().BeNull();
                });
        }

        #endregion

        #region RunAsync Scenarios

        /// <summary>
        /// Scenario tests that the runner succeeds at running, given the configuration.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should succeed without error"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify completed log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner succeeds at running where stage runners have no names, given the configuration.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithNullStageRunnerNamesWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    _mockStageRunners.ForEach(r => r.SetupGet(s => s.Name).Returns((string)null));

                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should succeed without error"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify completed log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner succeeds at running with event handlers, given the configuration.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="beforeStage">Flag to indicate whether event fired.</param>
        /// <param name="afterStage">Flag to indicate whether event fired.</param>
        /// <param name="beforeStageRunner">Flag to indicate whether event fired.</param>
        /// <param name="afterStageRunner">Flag to indicate whether event fired.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithEventsWithSuccess(
            IRunner runner,
            IRunnerConfiguration config,
            IApplicationModel model,
            bool beforeStage,
            bool afterStage,
            bool beforeStageRunner,
            bool afterStageRunner,
            ILogger<Engine.Runner> logger,
            Exception e,
            CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And event handlers attached"
                .x(() =>
                {
                    runner.BeforeStage += (s, e) => beforeStage = true;
                    runner.AfterStage += (s, e) => afterStage = true;
                    runner.BeforeStageRunner += (s, e) => beforeStageRunner = true;
                    runner.AfterStageRunner += (s, e) => afterStageRunner = true;
                });

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should succeed without error"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify completed log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));

                    // Verify events fired
                    beforeStage.Should().BeTrue();
                    afterStage.Should().BeTrue();
                    beforeStageRunner.Should().BeTrue();
                    afterStageRunner.Should().BeTrue();
                });
        }

        /// <summary>
        /// Scenario tests that the runner succeeds at running with event handlers, given the configuration and no application model.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="beforeStage">Flag to indicate whether event fired.</param>
        /// <param name="afterStage">Flag to indicate whether event fired.</param>
        /// <param name="beforeStageRunner">Flag to indicate whether event fired.</param>
        /// <param name="afterStageRunner">Flag to indicate whether event fired.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithEventsNoModelWithSuccess(
            IRunner runner,
            IRunnerConfiguration config,
            IApplicationModel model,
            bool beforeStage,
            bool afterStage,
            bool beforeStageRunner,
            bool afterStageRunner,
            ILogger<Engine.Runner> logger,
            Exception e,
            CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a null model"
                .x(() => model.Should().BeNull());

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And event handlers attached"
                .x(() =>
                {
                    runner.BeforeStage += (s, e) => beforeStage = true;
                    runner.AfterStage += (s, e) => afterStage = true;
                    runner.BeforeStageRunner += (s, e) => beforeStageRunner = true;
                    runner.AfterStageRunner += (s, e) => afterStageRunner = true;
                });

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should succeed without error"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify completed log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));

                    // Verify events not fired
                    beforeStage.Should().BeFalse();
                    afterStage.Should().BeFalse();
                    beforeStageRunner.Should().BeFalse();
                    afterStageRunner.Should().BeFalse();

                    // Verify trace message fired (6 of each event, for each stage runner and stage)
                    _mockLogger.VerifyLog(l => l.LogDebug("*skipping raising event*"), Times.Exactly(24));
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given no stages, outputs a warning message and exits.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithNoStagesWithWarning(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.None;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should exit with a warning due to no stages set in config"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify warning log message output once
                    _mockLogger.VerifyLog(l => l.LogWarning("*configuration doesn't define any stages to run*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given no stage runners, outputs a warning message and exits.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithNoStageRunnersWithWarning(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                    config.StageRunners.Clear();
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should exit with a warning due to no stage runners set in config"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify warning log message output once
                    _mockLogger.VerifyLog(l => l.LogWarning("*configuration doesn't define any stage runners to run*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given all stage runners set to skip, outputs a warning message and exits.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithAllStageRunnersSkippedWithWarning(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;

                    foreach (var stageRunner in config.StageRunners)
                    {
                        stageRunner.Skip = true;
                    }
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should exit with a warning due to stage runners all set to skip in config"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify warning log message output once
                    _mockLogger.VerifyLog(l => l.LogWarning("*configuration defines 6 stage runners, but is set to skip the execution*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner cancels the execution of a stage.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithStageExecutionCancelled(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When cancelling the token at stage execution"
                .x(() =>
                {
                    _mockLogger.Setup(l => l.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Information),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Running stage")),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Callback(() => _source.Cancel());
                });

            "And executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should exit with an operation cancelled exception"
                .x(() =>
                {
                    // Verify operation cancelled exception
                    e.Should().NotBeNull().And.BeOfType<OperationCanceledException>();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify warning log message output once
                    _mockLogger.VerifyLog(l => l.LogWarning("*Runner has been cancelled by the caller*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner cancels the execution of a stage runner.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithStageRunnerExecutionCancelled(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When cancelling the token at stage runner execution"
                .x(() =>
                {
                    _mockLogger.Setup(l => l.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Information),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Running stage runner")),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))).Callback(() => _source.Cancel());
                });

            "And executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should exit with an operation cancelled exception"
                .x(() =>
                {
                    // Verify operation cancelled exception
                    e.Should().NotBeNull().And.BeOfType<OperationCanceledException>();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify warning log message output once
                    _mockLogger.VerifyLog(l => l.LogWarning("*Runner has been cancelled by the caller*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given one stage set to skip, outputs a message and continues.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithOneStageSkippedWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.Discover | Stages.Parse | Stages.Analyze | Stages.Convert | Stages.Verify;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should output a message for the skipped stage, but execute all other stages"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify skip log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*Skipping stage 'Report'*"));

                    // Verify execute log message output 5 times
                    _mockLogger.VerifyLog(l => l.LogInformation(
                        It.Is<string>(v =>
                            v.Contains("Running stage 'Discover'") ||
                            v.Contains("Running stage 'Parse'") ||
                            v.Contains("Running stage 'Analyze'") ||
                            v.Contains("Running stage 'Convert'") ||
                            v.Contains("Running stage 'Verify'")), Times.Exactly(5)));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given one stage runner set to skip, outputs a message and continues.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithOneStageRunnerSkippedWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;

                    foreach (var stageRunner in config.StageRunners)
                    {
                        stageRunner.Skip = (stageRunner.Stages & Stages.Convert) == Stages.Convert ? true : false;
                    }
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should output a message for the skipped stage runner, but execute all other stage runners"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify skip log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*Skipping stage runner*"));

                    // Verify execute log message output 5 times
                    _mockLogger.VerifyLog(l => l.LogInformation("*Running stage runner*"), Times.Exactly(5));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given one stage runner with null name set to skip, outputs a message and continues.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithOneStageRunnerWithNullNameSkippedWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    _mockStageRunners.ForEach(r => r.SetupGet(s => s.Name).Returns((string)null));

                    config = _mockConfig.Object;
                    config.Stages = Stages.All;

                    foreach (var stageRunner in config.StageRunners)
                    {
                        stageRunner.Skip = (stageRunner.Stages & Stages.Convert) == Stages.Convert ? true : false;
                    }
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should output a message for the skipped stage runner, but execute all other stage runners"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify skip log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*Skipping stage runner*"));

                    // Verify execute log message output 5 times
                    _mockLogger.VerifyLog(l => l.LogInformation("*Running stage runner*"), Times.Exactly(5));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner, given one stage runner found by name set to skip, outputs a message and continues.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithOneStageRunnerSkippedByNameWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                    config.StageRunners.Where(r => r.Name == "MockConverter").First().Skip = true;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should output a message for the skipped stage runner, but execute all other stage runners"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify skip log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*Skipping stage runner*"));

                    // Verify execute log message output 5 times
                    _mockLogger.VerifyLog(l => l.LogInformation("*Running stage runner*"), Times.Exactly(5));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });
        }

        /// <summary>
        /// Scenario tests that the runner will fail fast the stage if a stage runner fails.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunStageWithFailFast(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                    config.FailStages = Stages.Report;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a stage runner that will fail"
                .x(() => _mockStageRunners[3].Setup(m => m.RunAsync(It.IsAny<IRunState>(), It.IsAny<CancellationToken>())).ThrowsAsync(new DivideByZeroException()));

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should abort after the report stage when the report stage runner fails"
                .x(() =>
                {
                    // Verify runner exception
                    e.Should().NotBeNull().And.BeOfType<Engine.RunnerException>().Which.Message.Should().Contain("aborting runner");

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify abort log message output once
                    _mockLogger.VerifyLog(l => l.LogError("*aborting runner*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner will fail fast if a stage runner fails.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void RunWithFailFast(IRunner runner, IRunnerConfiguration config, IApplicationModel model, ILogger<Engine.Runner> logger, Exception e, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                    config.FailFast = true;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a stage runner that will fail"
                .x(() => _mockStageRunners[3].Setup(m => m.RunAsync(It.IsAny<IRunState>(), It.IsAny<CancellationToken>())).ThrowsAsync(new DivideByZeroException()));

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "Then the runner should abort immediately the report stage runner fails"
                .x(() =>
                {
                    // Verify runner exception
                    e.Should().NotBeNull().And.BeOfType<Engine.RunnerException>().Which.Message.Should().Contain("aborting runner immediately");

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify abort log message output once
                    _mockLogger.VerifyLog(l => l.LogError("*aborting runner immediately*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"), Times.Never);
                });
        }

        /// <summary>
        /// Scenario tests that the runner succeeds at running, given the configuration.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="config">The config.</param>
        /// <param name="model">The model.</param>
        /// <param name="json">The JSON string.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="e">The runner exception, if any.</param>
        /// <param name="se">The serialization exception, if any.</param>
        /// <param name="token">The token to use for cancellation.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void SerializeRunStateAsJsonWithSuccess(IRunner runner, IRunnerConfiguration config, IApplicationModel model, string json, ILogger<Engine.Runner> logger, Exception e, Exception se, CancellationToken token)
        {
            "Given runner configuration"
                .x(() =>
                {
                    config = _mockConfig.Object;
                    config.Stages = Stages.All;
                });

            "And a model"
                .x(() => model = _mockModel.Object);

            "And a logger"
                .x(() => logger = _mockLogger.Object);

            "And a new runner"
                .x(() => runner = new Engine.Runner(config, model, logger));

            "And a cancellation token"
                .x(() => token = _source.Token);

            "When executing the runner"
                .x(async () => e = await Record.ExceptionAsync(async () => await runner.RunAsync(token)));

            "And serializing the execution state to JSON"
                .x(() => se = Record.Exception(() => json = JsonSerializer.Serialize<IRunState>(runner.RunState, new JsonSerializerOptions() { WriteIndented = true })));

            "Then the runner should succeed without error"
                .x(() =>
                {
                    // Verify no exception
                    e.Should().BeNull();

                    // Verify starting log message output once
                    _mockLogger.VerifyLog(l => l.LogInformation("*starting*"));

                    // Verify completed log message never output
                    _mockLogger.VerifyLog(l => l.LogInformation("*completed*"));
                });

            "And the execution state can be serialized to JSON successfully"
                .x(() =>
                {
                    // Verify no exception
                    se.Should().BeNull();

                    // Verify string contains JSON
                    json.Should().NotBeNullOrEmpty();
                    new Action(() => JsonDocument.Parse(json)).Should().NotThrow();
                });
        }

        #endregion
    }
}
