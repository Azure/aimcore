namespace Microsoft.AzureIntegrationMigration.Runner.Model
{
    /// <summary>
    /// Defines a base interface for a provider that returns a model instance.
    /// </summary>
    public interface IApplicationModelProvider : IRunnerComponent
    {
        /// <summary>
        /// Returns a model.
        /// </summary>
        IApplicationModel GetModel();
    }
}
