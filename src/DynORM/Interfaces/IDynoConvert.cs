using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynORM.Interfaces
{
    public interface IDynoConvert
    {
        /// <summary>
        /// Returns a tuple 
        ///     <h1>Item1</h1>=Object as string that would be save as it on database
        ///     <h1>Item2</h1>=Original type of the object
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Tuple Item1=Object as string, Item2=Original object type</returns>
        AttributeValue ToItem(object value);

        /// <summary>
        /// Returns an object from item value
        /// </summary>
        /// <param name="item">String that was saved in database</param>
        /// <returns>Object value</returns>
        object ToValue(string item);
    }
}
