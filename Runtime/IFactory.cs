namespace Fraktal.DesignPatterns
{
    /// <summary>
    /// Factory interface for creating objects of type <typeparamref name="TOutput"/> from a single input parameter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input parameter required for object creation.</typeparam>
    /// <typeparam name="TOutput">The type of object to be created by the factory.</typeparam>
    public interface IFactory<in TInput, out TOutput>
    {
        /// <summary>
        /// Creates an instance of type <typeparamref name="TOutput"/> using the provided input.
        /// </summary>
        /// <param name="input">The input parameter used to create the object.</param>
        /// <returns>A new instance of type <typeparamref name="TOutput"/>.</returns>
        public TOutput Create(TInput input);
    }

    /// <summary>
    /// Factory interface for creating objects of type <typeparamref name="TOutput"/> from two input parameters.
    /// </summary>
    /// <typeparam name="TInput1">The type of the first input parameter required for object creation.</typeparam>
    /// <typeparam name="TInput2">The type of the second input parameter required for object creation.</typeparam>
    /// <typeparam name="TOutput">The type of object to be created by the factory.</typeparam>
    public interface IFactory<in TInput1, in TInput2, out TOutput>
    {
        /// <summary>
        /// Creates an instance of type <typeparamref name="TOutput"/> using the provided input parameters.
        /// </summary>
        /// <param name="input1">The first input parameter used to create the object.</param>
        /// <param name="input2">The second input parameter used to create the object.</param>
        /// <returns>A new instance of type <typeparamref name="TOutput"/>.</returns>
        public TOutput Create(TInput1 input1, TInput2 input2);
    }

    /// <summary>
    /// Factory interface for creating objects of type <typeparamref name="T"/> without any input parameters.
    /// </summary>
    /// <typeparam name="T">The type of object to be created by the factory.</typeparam>
    public interface IFactory<out T>
    {
        /// <summary>
        /// Creates an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        public T Create();
    }
}