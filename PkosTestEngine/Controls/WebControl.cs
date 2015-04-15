using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace PkosTestEngine.Controls
{
  public class WebControl
  {
    /// <summary>
    /// Element Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// CSS class
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// HTML tag name
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// HTML Control Inner text 
    /// </summary>
    public string InnerText { get; set; }

    /// <summary>
    /// Element Name
    /// </summary>
    public string Name { get; set; }

    public PropertyExpressionCollection ToPropertyCollection( )
    {
            var resultCollection = new PropertyExpressionCollection();
            var type = typeof(WebControl);

            var properties = type.GetProperties();

            var propertyValue = string.Empty;
            foreach (var property in properties)
            {
                propertyValue = property.GetValue(this, null) as string;
                if (!string.IsNullOrWhiteSpace(propertyValue))
                {
                    if (propertyValue.Contains('*'))
                    {
                        propertyValue = propertyValue.Replace("*", string.Empty);
                        resultCollection.Add(property.Name, propertyValue, PropertyExpressionOperator.Contains);
                    }
                    else
                    {
                        resultCollection.Add(property.Name, propertyValue, PropertyExpressionOperator.EqualTo);
                    }
                }
            }

            return resultCollection;
        }

        /// <summary>
        /// Returns a string with all selector filters.
        /// </summary>
        /// <returns>String with all filters description.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var criteria = this.ToPropertyCollection();

            foreach (var searchParameter in criteria)
            {
                stringBuilder.AppendFormat(
                    "Name:{0}    Value:{1}.{2}",
                    searchParameter.PropertyName,
                    searchParameter.PropertyValue,
                    System.Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

  }
}
