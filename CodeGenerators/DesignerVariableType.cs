using System;
using System.Text.RegularExpressions;
using Microsoft.BizTalk.Component.Utilities;

namespace MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators
{
    /// <summary>
    /// used to strongly cast the string selection choosen by the user
    /// as designer variables types. primarily used to facilitate strong
    /// casting within the code creating the pipeline component using CodeDOM
    /// </summary>
    public static class DesignerVariableType
    {
        /// <summary>
        /// represents the string primitive type
        /// </summary>
        private const string DvtString = "string";
        /// <summary>
        /// represents the bool primitive type
        /// </summary>
        private const string DvtBoolean = "bool";
        /// <summary>
        /// represents the int primitive type
        /// </summary>
        private const string DvtInt = "int";
        /// <summary>
        /// represents the long primitive type
        /// </summary>
        private const string DvtLong = "long";
        /// <summary>
        /// represents the short primitive type
        /// </summary>
        private const string DvtShort = "short";
        /// <summary>
        /// represents the Microsoft.BizTalk.Component.Utilities.SchemaList type
        /// </summary>
        private const string DvtSchemaList = "SchemaList";
        /// <summary>
        /// represents the Microsoft.BizTalk.Component.Utilities.SchemaWithNone type
        /// </summary>
        private const string DvtSchemaWithNone = "SchemaWithNone";
        /// <summary>
        /// determines whether a reference to Microsoft.BizTalk.Component.Utilities
        /// is needed within the generated project
        /// </summary>
        private static bool _schemaListUsed;

        /// <summary>
        /// determines whether the user added one or more Designer properties of the
        /// SchemaList type
        /// </summary>
        public static bool SchemaListUsed => _schemaListUsed;

        /// <summary>
        /// checks whether the inbound string matches the regular expressions
        /// set up to catch the supported types and returns a typeof of the
        /// associated type.
        /// </summary>
        /// <param name="dataType">the representation of the type</param>
        /// <returns>the actual type, if supported. returns typeof(object) if no
        /// match can be found</returns>
        public static Type GetType(string dataType)
        {
            if (Regex.IsMatch(dataType, DvtBoolean, RegexOptions.IgnoreCase))
            {
                return typeof(bool);
            }
            else if (Regex.IsMatch(dataType, DvtInt, RegexOptions.IgnoreCase))
            {
                return typeof(int);
            }
            else if (Regex.IsMatch(dataType, DvtLong, RegexOptions.IgnoreCase))
            {
                return typeof(long);
            }
            else if (Regex.IsMatch(dataType, DvtSchemaList, RegexOptions.IgnoreCase))
            {
                // Microsoft.BizTalk.Component.Utilities needs to be referenced
                _schemaListUsed = true;

                return typeof(SchemaList);
            }
            else if (Regex.IsMatch(dataType, DvtSchemaWithNone, RegexOptions.IgnoreCase))
            {
                // Microsoft.BizTalk.Component.Utilities needs to be referenced
                _schemaListUsed = true;

                return typeof(SchemaWithNone);
            }
            else if (Regex.IsMatch(dataType, DvtShort, RegexOptions.IgnoreCase))
            {
                return typeof(short);
            }
            else if (Regex.IsMatch(dataType, DvtString, RegexOptions.IgnoreCase))
            {
                return typeof(string);
            }
            else
            {
                return typeof(object);
            }
        }

        public static bool IsSchemaList(string dataType)
        {
            return Regex.IsMatch(dataType, DvtSchemaWithNone, RegexOptions.IgnoreCase) ||
                   Regex.IsMatch(dataType, DvtSchemaList, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// returns a string array for all supported variable types.
        /// used to create the dropdownlist in which the user can
        /// choose what type designer property should encompass
        /// </summary>
        /// <returns></returns>
        public static object[] ToArray()
        {
            return new object[]
            {
                DvtString,
                DvtBoolean,
                DvtInt,
                DvtLong,
                DvtShort,
                DvtSchemaList,
                DvtSchemaWithNone
            };
        }
    }
}