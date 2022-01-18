namespace AnonCommandSystem
{
    /// <summary>
    /// The interface that needs to be implemented as a custom parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandParameter<T>
    {
        /// <summary>
        /// Whether the string can be converted to value
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="getValue">return value</param>
        /// <returns></returns>
        bool TryParse(string input, out T getValue);
        /// <summary>
        /// Get the complete list of the parameter
        /// </summary>
        /// <param name="preInput">Pre-Input string</param>
        /// <returns>complietion list</returns>
        string[] GetParameteCompletion(string preInput);
    }
}