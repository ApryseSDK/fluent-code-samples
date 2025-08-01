/*
* Copyright (c) 2012 by Windward Studios, Inc. All rights reserved.
*
* This software is the confidential and proprietary information of
* Windward Studios ("Confidential Information").  You shall not
* disclose such Confidential Information and shall use it only in
* accordance with the terms of the license agreement you entered into
* with Windward Studios, Inc.
*/

/*
 * When writing your own custom functions the following data types can be
 * used for a function parameters and its return value.
 * 
 *      - object
 *      - string
 *      - double
 *      - long
 *      - arrays of the types listed above
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace FluentCustom
{
    public class FluentCustomFunctions
    {
        [FunctionDescription("Returns a custom string.")]
        public static string CUSTOMFUNCTION()
        {
            return "My Custom Function Works!";
        }

        /**
         * Leaving these here as examples of how to write custom functions with parameters.
         * These are not used in our sample template
         */
        [FunctionDescription("Returns the square root of a number.")]
        public static double SQRT(
            [ParameterDescription("is the number for which you want the square root.")] double num)
        {
            return Math.Sqrt(num);
        }

        [FunctionDescription("Returns a value equal to all the values of a dataset multiplied together.")]
        public static double MULTIPLYALL(
            [ParameterDescription("is the dataset whose values you want to multiply.")] object[] nums)
        {
            if ((nums == null) || (nums.Length == 0))
                return 0.0;

            double total = 1.0;
            for (int i = 0; i < nums.Length; i++)
            {
                try
                {
                    total *= double.Parse(nums[i].ToString());
                }
                catch (Exception)
                {
                    // Ignore it if we've got not a number.
                }
            }

            return total;
        }
    }
}
