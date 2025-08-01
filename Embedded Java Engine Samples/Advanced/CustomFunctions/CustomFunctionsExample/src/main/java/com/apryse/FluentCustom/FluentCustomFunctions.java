package com.apryse.FluentCustom;

import net.windward.util.macro.IMacroState;
import net.windward.xmlreport.WindwardEventHandler;

public class FluentCustomFunctions {

    /**
     * Set this value for the number of custom functions you have defined
     */
    public static int numberOfFunctions = 1;

    public static WindwardEventHandler eventHandler;

    public static String[] functionName = new String[numberOfFunctions];
    public static String[] functionFullName = new String[numberOfFunctions];
    public static String[] functionDescription = new String[numberOfFunctions];
    public static Integer[] functionNumberOfArgument = new Integer[numberOfFunctions];
    public static String[][] functionArgumentName = new String[numberOfFunctions][];
    public static String[][] functionArgumentDescription = new String[numberOfFunctions][];
    public static String[][] functionArgumentType = new String[numberOfFunctions][];

    static
    {
        functionName[0] = "CUSTOMFUNCTION";
        functionFullName[0] = "CUSTOMFUNCTION()";
        functionDescription[0] = "Returns a custom string denoting our custom function";
        functionNumberOfArgument[0] = new Integer(0);
        functionArgumentName[0] =  null;
        functionArgumentDescription[0] =  null;
        functionArgumentType[0] = null;

		/*
		ADDTOTAL and GETTOTAL are built-in engine macros now.
		These are left here for the reference on how to use the macro state.
		*/
		/*
		functionName[1] = "ADDTOTAL";
		functionFullName[1] = "ADDTOTAL(number,text)";
		functionDescription[1] = "Adds number to running total";
		functionNumberOfArgument[1] = new Integer(2);
		functionArgumentName[1] = new String[] { "Number","Key" };
		functionArgumentDescription[1] = new String[] { "Is the number you want to add to the running total.","Is the name of the running total to use" };
		functionArgumentType[1] = new String[] { "number","text" };

		functionName[2] = "GETTOTAL";
		functionFullName[2] = "GETTOTAL(text)";
		functionDescription[2] = "Get number of running total";
		functionNumberOfArgument[2] = new Integer(1);
		functionArgumentName[2] = new String[] { "Key" };
		functionArgumentDescription[2] = new String[] { "Is the name of the running total you want to return" };
		functionArgumentType[2] = new String[] { "text" };
		*/
    }

    public static Object CUSTOMFUNCTION()
    {
        return "My Custom Function works!";
    }

	/*
	ADDTOTAL and GETTOTAL are built-in engine macros now.
	These are left here for the reference on how to use the macro state.
	*/
	/*
	public static Object ADDTOTAL(Number num, String key)
	{
		Double retVal;
		if (eventHandler == null)
			return "";
		if (eventHandler.getData(key) != null)
			retVal = new Double(((Number) eventHandler.getData(key)).doubleValue() + (num.doubleValue()));
		else
			retVal = new Double(num.doubleValue());
		eventHandler.setData(key, retVal);
		return "";
	}

	public static Object GETTOTAL(String key)
	{
		if (eventHandler == null || eventHandler.getData(key) == null)
			return "";
		return ((Double) eventHandler.getData(key));
	}
	*/

    public static void SetMacroState(IMacroState state)
    {
        eventHandler =  state.getEventHandler();
    }
}
