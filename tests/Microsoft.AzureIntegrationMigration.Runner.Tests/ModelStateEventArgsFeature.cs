using System;
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
    /// Defines the test spec for the <see cref="ModelStateEventArgs"/> class.
    /// </summary>
    public class ModelStateEventArgsFeature
    {
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
            "Given a new mock model"
                .x(() => _mockModel = new Mock<IApplicationModel>());
        }

        #endregion

        #region Constructor Scenarios

        /// <summary>
        /// Scenario tests that the object construction throws an exception when null model state is passed.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <param name="model">The model state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithNullEventArgs(ModelStateEventArgs eventArgs, IApplicationModel model, Exception e)
        {
            "Given the event args"
                .x(() => eventArgs.Should().BeNull());

            "And the model state"
                .x(() => model.Should().BeNull());

            "When constructing with null model state"
                .x(() => e = Record.Exception(() => new ModelStateEventArgs(model)));

            "Then the event args constructor should throw an exception"
                .x(() => e.Should().NotBeNull().And.Subject.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("model"));
        }

        /// <summary>
        /// Scenario tests that the object construction succeeds.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <param name="model">The model state.</param>
        /// <param name="e">The runner exception, if any.</param>
        [Scenario]
        [Trait(TestConstants.TraitCategory, TestConstants.CategoryUnitTest)]
        public void ConstructWithSuccess(ModelStateEventArgs eventArgs, IApplicationModel model, Exception e)
        {
            "Given the event args"
                .x(() => eventArgs.Should().BeNull());

            "And the model state"
                .x(() => model = _mockModel.Object);

            "When constructing the event args"
                .x(() => e = Record.Exception(() => eventArgs = new ModelStateEventArgs(model)));

            "Then the event args constructor should succeed"
                .x(() => e.Should().BeNull());

            "And the model state should be available"
                .x(() => eventArgs.Model.Should().NotBeNull().And.BeSameAs(model));
        }

        #endregion
    }
}
