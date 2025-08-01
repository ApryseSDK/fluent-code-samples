using net.windward.datasource;
using net.windward.env;
using net.windward.tags;
using net.windward.util.callback;

namespace FluentCustomCallbacks
{
    /// <summary>
    /// Class containing Fluent custom user callbacks.
    /// </summary>
    public class FluentCustomCallbacks : IWindwardCallback
    {
        // Variable to set if we want to prohibit addresses being exposed
        private bool hideAddresses = false;

        /// <summary>
        /// This sample function will modify the select statement for a tag if it contains a specific string.
        /// This sample function will also throw an exception if our hideAddresses variable is set to true and the select statement contains the word "address".
        /// </summary>
        /// <param name="select">The select of the tag</param>
        /// <param name="dsp">The datasource provider</param>
        /// <param name="bt">The tag whose select we are verifying</param>
        /// <returns>The updated select</returns>
        /// <exception cref="DataSourceException"></exception>
        public string approveSelect(string select, DataSourceProvider dsp, BaseTag bt)
        {
            //In a custom callback, you can modify the select tag.
            if (select.Equals("$$THIS_IS_INCORRECTLY_FORMATTED$$"))
                return "$$THIS_IS_CORRECTLY_FORMMATED$$";

            // We can also prevent people from accessing unwanted data, such as, perhaps, the address of a customer
            if (hideAddresses && select.ToLower().Contains("address"))
                throw new DataSourceException("Select for this tag was denied by custom callback because we don't want someone seeing our customer's address", 0);

            return select;
        }
    }
}
