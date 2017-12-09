using System;
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
        public static bool IsSchemaList(Type dataType)
        {
            return dataType == typeof(SchemaList) || dataType == typeof(SchemaWithNone);
        }

        /// <summary>
        /// returns a Type array for all supported variable types.
        /// used to create the dropdownlist in which the user can
        /// choose what type designer property should encompass
        /// </summary>
        /// <returns></returns>
        public static readonly Type[] ToArray =
        {
            typeof(string),
            typeof(bool),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(SchemaList),
            typeof(SchemaWithNone)
        };
    }
}
