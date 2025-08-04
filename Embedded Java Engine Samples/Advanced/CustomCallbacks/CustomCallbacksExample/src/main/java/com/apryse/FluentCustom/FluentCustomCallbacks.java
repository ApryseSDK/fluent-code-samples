package com.apryse.FluentCustom;

import net.windward.datasource.DataSourceProvider;
import net.windward.env.DataSourceException;
import net.windward.tags.BaseTag;
import net.windward.util.callback.IWindwardCallback;

/**
 * Class containing Fluent custom user callbacks
 */
public class FluentCustomCallbacks implements IWindwardCallback {

    /**
     * Variable to set if we want to prohibit addresses being exposed
     */
    private boolean hideAddresses = false;

    /**
     * This is a sample function that will check to see if the user is accessing the salary node in our customCallback database.
     * If they are, then it throws a new DataSourceException. It also does other things for the sake of being a sample, too.
     * If you modify this, make sure to keep the method name the same.
     *
     * @param select The select statement
     * @param provider The datasource provider.
     * @param xmlTag The xmlTag
     * @return The updated select
     * @throws DataSourceException
     */
    public String approveSelect(String select, DataSourceProvider provider, BaseTag xmlTag) throws DataSourceException {
        //In a custom callback, you can modify the select tag.
        if(select.equals("$$THIS_IS_INCORRECTLY_FORMATTED$$"))
            return "$$THIS_IS_CORRECTLY_FORMMATED$$";

        // We can also prevent people from accessing unwanted data, such as, perhaps, the address of a customer
        if(hideAddresses && select.toLowerCase().contains("address"))
            throw new DataSourceException("Select for this tag was denied by custom callback because we don't want someone seeing our customer's address",0);

        return select;
    }
}
