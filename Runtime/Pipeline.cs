using System;
using System.Collections.Generic;

namespace Fraktal.DesignPatterns
{
    /// <summary>
    /// Represents a pipeline that processes data through a sequence of steps.
    /// Each step transforms the input data and passes the result to the next step in the pipeline.
    /// </summary>
    /// <typeparam name="T">The type of data that flows through the pipeline</typeparam>
    /// <remarks>
    /// <para>The Pipeline class implements the Pipeline design pattern, allowing you to compose complex operations
    /// by chaining together simple, focused processing steps.</para>
    /// <para>Each step in the pipeline receives the output from the previous step as its input.</para>
    /// <para>This implementation is not thread-safe. External synchronization is required for concurrent access.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a string processing pipeline
    /// var pipeline = new Pipeline&lt;string&gt;();
    /// pipeline.Add(new TrimStep());
    /// pipeline.Add(new UpperCaseStep());
    /// pipeline.Add(new PrefixStep("PROCESSED: "));
    /// 
    /// string result = pipeline.Process("  hello world  ");
    /// // Result: "PROCESSED: HELLO WORLD"
    /// </code>
    /// </example>
    [Serializable]
    public class Pipeline<T>
    {
        private List<IPipelineStep<T>> steps = new ();
        /// <summary>
        /// Initializes a new empty instance of the Pipeline class.
        /// </summary>
        public Pipeline() { }

        /// <summary>
        /// Initializes a new instance of the Pipeline class with the specified steps.
        /// </summary>
        /// <param name="steps">The initial pipeline steps to add</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="steps"/> is null</exception>
        /// <example>
        /// <code>
        /// var pipeline = new Pipeline&lt;int&gt;(
        ///     new MultiplyStep(2),
        ///     new AddStep(10),
        ///     new SquareStep()
        /// );
        /// </code>
        /// </example>
        public Pipeline(params IPipelineStep<T>[] steps)
        {
            AddAll(steps);
        }
        
        /// <summary>
        /// Gets a read-only view of all pipeline steps in execution order.
        /// </summary>
        /// <value>A read-only list containing all pipeline steps</value>
        /// <remarks>
        /// The returned list reflects the current state of the pipeline. 
        /// Steps are ordered by their execution sequence.
        /// </remarks>
        public IReadOnlyList<IPipelineStep<T>> ReadOnlySteps => steps.AsReadOnly();

        /// <summary>
        /// Replaces an existing pipeline step with a new step.
        /// </summary>
        /// <param name="old">The existing step to replace</param>
        /// <param name="step">The new step to insert in place of the old step</param>
        /// <remarks>
        /// <para>This method finds the first occurrence of <paramref name="old"/> and replaces it with <paramref name="step"/>.</para>
        /// <para>If <paramref name="old"/> is not found in the pipeline, no action is taken.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="old"/> or <paramref name="step"/> is null</exception>
        /// <example>
        /// <code>
        /// var oldStep = new AddStep(5);
        /// var newStep = new AddStep(10);
        /// pipeline.Replace(oldStep, newStep);
        /// </code>
        /// </example>
        public void Replace(IPipelineStep<T> old, IPipelineStep<T> step)
        {
            Replace(steps.IndexOf(old), step);
        }
        
        /// <summary>
        /// Replaces the pipeline step at the specified index with a new step.
        /// </summary>
        /// <param name="index">The zero-based index of the step to replace</param>
        /// <param name="step">The new step to insert at the specified index</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to the number of steps</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="step"/> is null</exception>
        /// <example>
        /// <code>
        /// // Replace the second step (index 1) in the pipeline
        /// pipeline.Replace(1, new DifferentProcessingStep());
        /// </code>
        /// </example>
        public void Replace(int index, IPipelineStep<T> step)
        {
            steps[index] = step;
        }
        
        /// <summary>
        /// Removes the specified step from the pipeline.
        /// </summary>
        /// <param name="step">The step to remove from the pipeline</param>
        /// <returns><c>true</c> if the step was successfully removed; otherwise, <c>false</c></returns>
        /// <remarks>
        /// This method removes the first occurrence of <paramref name="step"/> from the pipeline.
        /// If the step is not found, the method returns <c>false</c> and the pipeline remains unchanged.
        /// </remarks>
        /// <example>
        /// <code>
        /// var stepToRemove = new OptionalProcessingStep();
        /// bool removed = pipeline.Remove(stepToRemove);
        /// if (!removed)
        /// {
        ///     Debug.LogWarning("Step was not found in pipeline");
        /// }
        /// </code>
        /// </example>
        public void Remove(IPipelineStep<T> step)
        {
            steps.Remove(step);
        }
        
        /// <summary>
        /// Removes the pipeline step at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the step to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to the number of steps</exception>
        /// <example>
        /// <code>
        /// // Remove the first step from the pipeline
        /// pipeline.RemoveAt(0);
        /// </code>
        /// </example>
        public void RemoveAt(int index)
        {
            steps.RemoveAt(index);
        }
        
        
        /// <summary>
        /// Adds a step to the end of the pipeline.
        /// </summary>
        /// <param name="step">The pipeline step to add</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="step"/> is null</exception>
        /// <remarks>
        /// The added step will be executed after all existing steps in the pipeline.
        /// </remarks>
        /// <example>
        /// <code>
        /// pipeline.Add(new ValidationStep());
        /// pipeline.Add(new TransformationStep());
        /// pipeline.Add(new LoggingStep());
        /// </code>
        /// </example>        
        public void Add(IPipelineStep<T> step) 
        {
            steps.Add(step);
        }

        /// <summary>
        /// Adds multiple steps to the end of the pipeline.
        /// </summary>
        /// <param name="pipeline">The collection of pipeline steps to add</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipeline"/> is null</exception>
        /// <remarks>
        /// Steps are added in the order they appear in the collection.
        /// All steps will be executed after any existing steps in the pipeline.
        /// </remarks>
        /// <example>
        /// <code>
        /// var additionalSteps = new IPipelineStep&lt;string&gt;[]
        /// {
        ///     new ValidationStep(),
        ///     new NormalizationStep(),
        ///     new CacheStep()
        /// };
        /// pipeline.AddAll(additionalSteps);
        /// </code>
        /// </example>
        public void AddAll(IEnumerable<IPipelineStep<T>> pipeline)
        {
            steps.AddRange(pipeline);
        }

        /// <summary>
        /// Processes the input value through all pipeline steps in sequence.
        /// </summary>
        /// <param name="val">The input value to process</param>
        /// <returns>The final processed value after all pipeline steps have been executed</returns>
        /// <remarks>
        /// <para>Each step receives the output from the previous step as its input.</para>
        /// <para>If the pipeline contains no steps, the original input value is returned unchanged.</para>
        /// <para>If any step throws an exception, pipeline processing stops and the exception propagates to the caller.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Process a value through the pipeline
        /// var input = "  Raw Data  ";
        /// var result = pipeline.Process(input);
        /// 
        /// // For empty pipeline
        /// var emptyPipeline = new Pipeline&lt;string&gt;();
        /// var unchanged = emptyPipeline.Process("test"); // Returns "test"
        /// </code>
        /// </example>
        public T Process(T val)
        {
            T currentVal = val;
            for (int i = 0; i < steps.Count; i++)
            {
                currentVal = steps[i].Process(currentVal);
            }

            return currentVal;
        }
    }
    
    
}