using System;

namespace Fraktal.DesignPatterns
{
    /// <summary>
    /// Defines a contract for a single step in a processing pipeline.
    /// Implementations should transform the input data and return the processed result.
    /// </summary>
    /// <typeparam name="T">The type of data that this pipeline step processes</typeparam>
    /// <remarks>
    /// <para>Pipeline steps are designed to be composable and reusable components that perform a single,
    /// well-defined transformation on data. Each step should follow the Single Responsibility Principle.</para>
    /// <para>Steps should be stateless when possible to ensure predictable behavior and thread safety.</para>
    /// <para>If a step needs to maintain state, consider the implications for pipeline reusability and concurrent execution.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example implementation: String trimming step
    /// public class TrimStep : IPipelineStep&lt;string&gt;
    /// {
    ///     public string Proccess(string input)
    ///     {
    ///         return input?.Trim() ?? string.Empty;
    ///     }
    /// }
    /// 
    /// // Example implementation: Mathematical operation step
    /// public class MultiplyStep : IPipelineStep&lt;int&gt;
    /// {
    ///     private readonly int multiplier;
    ///     
    ///     public MultiplyStep(int multiplier)
    ///     {
    ///         this.multiplier = multiplier;
    ///     }
    ///     
    ///     public int Proccess(int input)
    ///     {
    ///         return input * multiplier;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IPipelineStep<T> : ICancellable
    {
        /// <summary>
        /// Processes the input data and returns the transformed result.
        /// </summary>
        /// <param name="input">The input data to be processed by this pipeline step</param>
        /// <returns>The processed result that will be passed to the next step in the pipeline</returns>
        /// <remarks>
        /// <para>This method should perform a single, well-defined transformation on the input data.</para>
        /// <para>The implementation should be deterministic - given the same input, it should always produce the same output.</para>
        /// <para>If the step cannot process the input (e.g., due to invalid data), consider these approaches:</para>
        /// <list type="bullet">
        /// <item><description>Return the input unchanged for non-critical failures</description></item>
        /// <item><description>Return a default/safe value if appropriate</description></item>
        /// <item><description>Throw a descriptive exception for critical failures</description></item>
        /// </list>
        /// <para>Avoid side effects when possible. If side effects are necessary (logging, caching, etc.), 
        /// document them clearly in the implementing class.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Simple validation step
        /// public string Process(string input)
        /// {
        ///     if (string.IsNullOrWhiteSpace(input))
        ///         throw new ArgumentException("Input cannot be null or empty", nameof(input));
        ///         
        ///     return input.ToUpperInvariant();
        /// }
        /// 
        /// // Defensive processing step
        /// public int Proccess(int input)
        /// {
        ///     // Handle edge case gracefully
        ///     if (input &lt; 0)
        ///         return 0;
        ///         
        ///     return input * 2;
        /// }
        /// </code>
        /// </example>
        public T Process(T input);
    }
}