namespace Fraktal.DesignPatterns
{
    /// <summary>
    /// Defines a contract for objects that support cancellation state management.
    /// Implementations should provide thread-safe access to cancellation status and allow external control
    /// over the cancellation state for graceful operation termination.
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        /// Gets the current cancellation state of the object.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the operation has been cancelled; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method should be called periodically during long-running operations to check
        /// if cancellation has been requested. 
        /// 
        /// In Unity contexts, this is commonly checked within Update() methods, coroutines,
        /// or animation loops to allow for responsive cancellation.
        /// </remarks>
        public bool HasCancelled();
        
        /// <summary>
        /// Sets the cancellation state of the object.
        /// </summary>
        /// <param name="value">
        /// <c>true</c> to mark the operation as cancelled; <c>false</c> to reset the cancellation state.
        /// </param>
        /// <remarks>
        /// Setting this to <c>true</c> signals that any ongoing operations should terminate gracefully
        /// as soon as possible. Setting it to <c>false</c> can be used to reset the cancellation state,
        /// allowing the object to be reused for new operations.
        /// 
        /// In Unity, this method is typically called in response to user input (e.g., ESC key),
        /// UI button clicks, scene changes, or when objects are destroyed.
        /// </remarks>
        public void SetCancelled(bool value);
    }
}