using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PkosTestEngine.Controls;

namespace PkosTestEngine
{
    public static class SearchEngine
    {


        private static T FindById<T>(this UITestControl control, string controlId) where T : HtmlControl, new()
        {
            var allResult = FindAll<T>(control, new WebControl() { Id = controlId });
            T resultControl = default(T);

            if (allResult.Any())
            {
                resultControl = allResult[0];
            }

            return resultControl;
        }

        /// <summary>
        /// Search HTML control by class name.
        /// </summary>
        /// <typeparam name="T">Type of the element to find.</typeparam>
        /// <param name="control">Control to extend.</param>
        /// <param name="className">Class name fo find.</param>
        /// <param name="contains">Determines control contains only one class with given name or given class name is part of css classes for control.</param>
        /// <returns></returns>
        private static T FindFirstByCssClass<T>(this UITestControl control, string className, bool contains = true) where T : HtmlControl, new()
        {
            if (contains)
            {
                className = string.Format("*{0}", className);
            }

            var allResult = FindAll<T>(control, new WebControl() { Class = className.Trim() });
            T resultControl = default(T);

            if (allResult.Any())
            {
                resultControl = allResult[0];
            }

            return resultControl;
        }

        private static ReadOnlyCollection<T> FindAll<T>(this UITestControl control, WebControl selectorDefinition, PropertyExpressionCollection morePropertyCollection = null) where T : HtmlControl, new()
        {
            var result = new List<T>();
            T selectorElement = new T { Container = control };
            selectorElement.SearchProperties.AddRange(selectorDefinition.ToPropertyCollection());

            if (morePropertyCollection != null)
            {
                selectorElement.SearchProperties.AddRange(morePropertyCollection);
            }
            try
            {
                if (!selectorElement.Exists)
                {
                    Trace.WriteLine(string.Format("Html {0} element not exists for given selector {1}.", typeof(T).Name, selectorDefinition), "UI CodedTest");
                    return result.AsReadOnly();
                }
            }
            catch
            {
                Trace.WriteLine(string.Format("Html {0} element not exists for given selector {1}.", typeof(T).Name, selectorDefinition), "UI CodedTest");
                return result.AsReadOnly();
            }

            return selectorElement.FindMatchingControls().Select(c => (T)c).ToList().AsReadOnly();
        }

        private static IList<string> TagName(Type T)
        {
            if (typeof(HtmlEdit).Name == T.Name)
            {
                return new List<string>() { "input" };
            }
            if (typeof(HtmlDiv).Name == T.Name)
            {
                return new List<string>() { "div" };
            }
            if (typeof(HtmlList).Name == T.Name)
            {
                return new List<string>() { "ul", "ol" };
            }
            if (typeof(HtmlInputButton).Name == T.Name)
            {
                return new List<string>() { "input" };
            }
            if (typeof(HtmlButton).Name == T.Name)
            {
                return new List<string>() { "button" };
            }
            if (typeof(HtmlListItem).Name == T.Name)
            {
                return new List<string>() { "li", "option" };
            }
            if (typeof(HtmlHyperlink).Name == T.Name)
            {
                return new List<string>() { "a" };
            }
            if (typeof(HtmlHyperlink).Name == T.Name)
            {
                return new List<string>() { "a" };
            }
            if (typeof(HtmlTable).Name == T.Name)
            {
                return new List<string>() { "table" };
            }
            if (typeof(HtmlImage).Name == T.Name)
            {
                return new List<string>() { "img", "input" };
            }
            if (typeof(HtmlComboBox).Name == T.Name)
            {
                return new List<string>() { "select" };
            }
            return new List<string>();
        }

        /// <summary>
        /// Finds the first item using selector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T FindControl<T>(this UITestControl control, string selector) where T : HtmlControl, new()
        {

            if (control == null)
            {
                throw new Exception(string.Format("Container null for selector:{0}", selector));
            }
            if (selector.IndexOf('#') == 0)
            {
                return control.FindById<T>(selector.Trim().Substring(1));
            }
            if (selector.IndexOf('.') == 0)
            {
                return control.FindFirstByCssClass<T>(selector.Trim().Substring(1));
            }
            throw new AssertFailedException(string.Format("Could not find a control with \"{0}\"", selector));
        }

        /// <summary>
        /// Find a collection of HtmlControls by CSS Class attribute
        /// </summary>
        /// <typeparam name="T">Html control needed to find</typeparam>
        /// <param name="control">Container control</param>
        /// <param name="className">class name</param>
        /// <param name="contains">If the control class name contains the className send or; false if needs to match exactly the same</param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> FindByCssClass<T>(this UITestControl control, string className, bool contains = true) where T : HtmlControl, new()
        {
            if (control == null)
            {
                throw new Exception(string.Format("Container null to search elements with class:{0}", className));
            }
            if (className.IndexOf('.') == 0)
            {
                className = className.Substring(1);
            }
            if (contains)
            {
                className = string.Format("*{0}", className);
            }
            var allResult = FindAll<T>(control, new WebControl() { Class = className.Trim() });
            T resultControl = default(T);

            if (allResult.Any())
            {
                return allResult;
            }

            return new ReadOnlyCollection<T>(new List<T>() { resultControl });
        }

        /// <summary>
        /// Returns HTML controls
        /// </summary>
        /// <typeparam name="T">HtmlControl expected</typeparam>
        /// <param name="control">Parent Control</param>
        /// <param name="selector">
        /// CSS Selector: controlTag #controId, .controlClass, div[attribute=value] (*=,|=,^=,$= means the same in this engine)
        /// </param>
        /// <returns>The expected HtmlControl T</returns>
        public static ReadOnlyCollection<T> FindControls<T>(this UITestControl control, string selector = "") where T : HtmlControl, new()
        {
            if (control == null)
            {
                throw new AssertFailedException("Container null ");
            }
            WebControl webControl = new WebControl();

            #region #controlId
            if (selector.IndexOf('#') == 0)
            {
                webControl = new WebControl() { Id = selector.Trim().Substring(1) };
                return FindAll<T>(control, webControl);
            }
            #endregion

            #region .controlClass
            if (selector.IndexOf('.') == 0)
            {
                webControl = new WebControl() { Class = selector.Substring(1) };
                return FindAll<T>(control, webControl);
            }
            #endregion

            // based on attributes
            #region Base on attributes
            PropertyExpressionCollection moreProperties = null;
            if (selector.Contains("["))
            {
                selector = selector.Replace("|", "*").Replace("^", "*").Replace("$", "*");

                string tagName = selector.Substring(0, selector.IndexOf("["));
                if (!TagName(typeof(T)).Contains(tagName))
                {
                    throw new Exception("TagName is not the same T expected");
                }
                string attribute = selector.Substring(selector.IndexOf("["));
                string[] fullSelector = attribute.Replace("'", "").Replace("[", "").Replace("]", "").Split('=');
                moreProperties = new PropertyExpressionCollection();
                if (fullSelector[0].EndsWith("*"))
                {
                    moreProperties.Add(fullSelector[0].Replace("*", string.Empty), fullSelector[1], PropertyExpressionOperator.Contains);
                }
                else
                {
                    moreProperties.Add(fullSelector[0], fullSelector[1], PropertyExpressionOperator.EqualTo);
                }

                return FindAll<T>(control, webControl, moreProperties);
            }
            #endregion

            // just TagName
            if (!string.IsNullOrEmpty(selector) && (TagName(typeof(T)).Contains(selector.Trim())))
            {
                throw new AssertFailedException("TagName is not the same T expected");
            }
            return FindAll<T>(control, webControl);
        }

        /// <summary>
        /// Click over the HtmlControl
        /// </summary>
        /// <param name="control"></param>
        public static void Click(this HtmlControl control)
        {
            try
            {

                Mouse.Click(control);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Find values in a table by criteria
        /// </summary>
        /// <param name="control">HtmlTable</param>
        /// <param name="searchCriteria">Criteria based on HtmlCell (td): eg.g td=> !string.IsNullOrEmpty(td.InnerText) && td.InnerText.Contains("Some Text")</param>
        /// <returns>Table Rows found</returns>
        public static ReadOnlyCollection<HtmlRow> FindValues(this HtmlTable control, Func<HtmlCell, bool> searchCriteria)
        {
            List<HtmlRow> results = new List<HtmlRow>();
            foreach (HtmlRow row in control.Rows)
            {
                foreach (HtmlCell cell in row.Cells)
                {
                    if (searchCriteria(cell))
                    {
                        results.Add(row);
                    }
                }
            }
            return new ReadOnlyCollection<HtmlRow>(results);
        }
    }
}
