/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.WPF.UI.Infrastructure                   *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public static class VisualTreeHelperExtended
    {
        /// <summary>
        /// Searches the visual tree for an element with the specified name
        /// </summary>
        /// <param name="reference">The parent visual referenced as a System.Windows.DependencyObject</param>
        /// <param name="name">The Name to search for.</param>
        /// <returns></returns>
        public static object FindElementByName(DependencyObject reference, string name)
        {

            if (reference is FrameworkElement element && element.Name == name)
                return reference;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(reference); i++)
            {
                var result = FindElementByName(VisualTreeHelper.GetChild(reference, i), name);

                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Returns a list of all children of the reference DependencyObject that are from the specified type T.
        /// </summary>
        /// <typeparam name="T">The type of elements to look for.</typeparam>
        /// <param name="reference">The parent visual referenced as a System.Windows.DependencyObject</param>
        /// <returns></returns>
        public static List<T> FindElementsByType<T>(DependencyObject reference)
        {
            List<T> result = new List<T>();

            if (reference.GetType() == typeof(T))
                result.Add((T)Convert.ChangeType(reference, typeof(T), null));

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(reference); i++)
            {
                result.AddRange(FindElementsByType<T>(VisualTreeHelper.GetChild(reference, i)));
            }

            return result;
        }
    }
}