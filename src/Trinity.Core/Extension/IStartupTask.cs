namespace Trinity.Extension
{
    /// <summary>
    /// Provides an entry point for executing a startup initialization task.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Provides an entry point for executing a startup initialization task.
    /// </summary>
    public interface IStartupTaskEx : IStartupTask
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        void Run(string[] args);
        /// <summary>
        /// Supported arguments. Only matched arguments will be passed to the entry point.
        /// </summary>
        string[] SupportedArguments { get; }
    }
}
