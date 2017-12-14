using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component.Utilities;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators.CodeDom
{
	/// <summary>
	/// PipelineComponentCodeGenerator is responsible for generating the code 
	/// for the pipeline component itself
	/// </summary>
	public static class PipelineComponentCodeGenerator
	{
        /// <summary>
        /// contains the options we use to generate the sourcecode
        /// </summary>
		private static readonly CodeGeneratorOptions CodeGeneratorOptions = new CodeGeneratorOptions
        {
            BracingStyle = "C",
            VerbatimOrder = true
        };

		#region retrieveCodeGenerator, generates an instance of the requested ICodeGenerator class
        /// <summary>
        /// gets an <see cref="I:ICodeGenerator"/> instance for the specified language using the specified
        /// <see cref="C:StreamWriter"/> as it's output buffer
        /// </summary>
        /// <param name="streamWriter">the <see cref="C:StreamWriter"/> instance to generate code into</param>
        /// <param name="language">the <see cref="C:implementationLanguages"/> value, defining which programming
        /// language the code should be generated in</param>
        /// <returns>the requested <see cref="C:ICodeGenerator"/> instance, if supported</returns>
		private static ICodeGenerator RetrieveCodeGenerator(StreamWriter streamWriter, ImplementationLanguages language)
		{
            // determine which language was requested
			switch(language)
			{
				case ImplementationLanguages.CSharp:
					return new CSharpCodeProvider().CreateGenerator(streamWriter);
				case ImplementationLanguages.VbNet:
					return new VBCodeProvider().CreateGenerator(streamWriter);
				default:
                    throw new NotImplementedException(
                        $"The requested language support ({Enum.GetName(typeof(ImplementationLanguages), language)}) has not been implemented.");
			}
		}
		#endregion

		#region generatePipelineComponent, responsible for using CodeDOM to actually create the used class 

	    /// <summary>
	    /// generates the main class this pipeline component revolves around, uses CodeCOM to generate
	    /// code and store it in "fileName"
	    /// </summary>
	    /// <param name="fileName">the output filename to use</param>
	    /// <param name="classNameSpace">the class' classNameSpace</param>
	    /// <param name="className">the class' name</param>
	    /// <param name="implementsIProbeMessage"></param>
	    /// <param name="designerProperties">any designer properties defined</param>
	    /// <param name="componentCategory">the used component category</param>
	    /// <param name="language"></param>
	    public static void GeneratePipelineComponent(
			string fileName,
			string classNameSpace,
			string className,
			bool implementsIProbeMessage,
			IDictionary<string, Type> designerProperties,
			ComponentType componentCategory,
			ImplementationLanguages language)
		{
			#region variable definition
			// the component GUID, used to represent the component uniquely
			Guid componentGuid = Guid.NewGuid();

			// the classes normally used within our component
		    string[] usingClauses =
		    {
		        "System",
		        "System.IO",
		        "System.Text",
		        "System.Drawing",
		        "System.Resources",
		        "System.Reflection",
		        "System.Diagnostics",
		        "System.Collections",
		        "System.ComponentModel",
		        "Microsoft.BizTalk.Message.Interop",
		        "Microsoft.BizTalk.Component.Interop",
		        "Microsoft.BizTalk.Component",
		        "Microsoft.BizTalk.Messaging",
		        "Microsoft.BizTalk.Streaming"
		    };

		    string[] baseTypes =
		    {
		        "IBaseComponent",
		        "IPersistPropertyBag",
		        "IComponentUI",
		    };
			#endregion

            // create the output file
                // enable writing to the selected output stream
		    using (StreamWriter sw = new StreamWriter(fileName))
		    {
		        // codeGenerator will contain the generator for the choosen language
		        ICodeGenerator codeGenerator = RetrieveCodeGenerator(sw, language);

		        // create our namespace, in which our class will reside
		        CodeNamespace codeNamespace = new CodeNamespace(classNameSpace);

		        CodeThisReferenceExpression thisObject = new CodeThisReferenceExpression();

		        #region using clauses

		        // iterate the entire using clause list
		        foreach (string usingClause in usingClauses)
		        {
		            // and add Import / using clauses for it
		            codeNamespace.Imports.Add(new CodeNamespaceImport(usingClause));
		        }

		        // SchemaList is defined inside Microsoft.BizTalk.Component.Utilities
		        if (designerProperties.Values.Any(DesignerVariableType.IsSchemaList))
		        {
		            // so add it to the Import / using clauses
		            codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.BizTalk.Component.Utilities"));
		        }

		        #endregion

		        // create our class, this variable will entail the entire definition until
		        // we write it out
		        CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration {Name = className};

		        // start by setting the name of the class

		        #region class attributes

		        // add our attributes
		        codeTypeDeclaration.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "ComponentCategory",
		                new CodeAttributeArgument(new CodeTypeReferenceExpression("CategoryTypes.CATID_PipelineComponent"))));

		        // add our Guid
		        codeTypeDeclaration.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "System.Runtime.InteropServices.Guid",
		                new CodeAttributeArgument(new CodePrimitiveExpression(componentGuid.ToString()))));

		        // add the component category
		        codeTypeDeclaration.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "ComponentCategory",
		                new CodeAttributeArgument(
		                    new CodeTypeReferenceExpression("CategoryTypes.CATID_" + componentCategory))));

		        #endregion

		        #region resource manager

		        // add our ResourceManager instance
		        CodeMemberField cmf = new CodeMemberField(typeof(ResourceManager), "resourceManager")
		        {
		            InitExpression = new CodeObjectCreateExpression(
		                typeof(ResourceManager), new CodeSnippetExpression("\"" + classNameSpace + "." + className + "\""),
		                new CodeMethodInvokeExpression(
		                    new CodeVariableReferenceExpression("Assembly"),
		                    "GetExecutingAssembly")),
		            Attributes = MemberAttributes.Private
		        };

		        // initialize the resourceManager by calling it's default constructor,
		        // passing Assembly.GetExecutingAssembly

		        // set the accessor visibility

		        // finally, add the statement to the class
		        codeTypeDeclaration.Members.Add(cmf);

		        #endregion

		        #region designer properties

		        // process all properties of the class which will be exposed inside the designer
		        CodeMemberProperty codeMemberProperty;

		        // iterate all defined properties
		        foreach (var entry in designerProperties)
		        {
		            // try and lookup the type as the variable
		            Type designerPropertyType = entry.Value;

		            // add a member variable using the suggested name
		            cmf = new CodeMemberField(designerPropertyType, "_" + entry.Key) {Attributes = MemberAttributes.Private};

		            // set the visibility scope

		            // SchemaList has special needs because it's not a native variable type,
		            // it needs an instance
		            if (designerPropertyType == typeof(SchemaList))
		            {
		                cmf.InitExpression = new CodeObjectCreateExpression(
		                    typeof(SchemaList));
		            }
		            // SchemaWithNone also has special needs because it's not a native variable type,
		            // it too needs an instance and a default parameter (empty selection string)
		            else if (designerPropertyType == typeof(SchemaWithNone))
		            {
		                cmf.InitExpression = new CodeObjectCreateExpression(
		                    typeof(SchemaWithNone), new CodePrimitiveExpression(String.Empty));
		            }

		            // add the member variable to the class definition
		            codeTypeDeclaration.Members.Add(cmf);

		            // instantiate a new CodeMemberProperty, which defines a getter/setter combination
		            codeMemberProperty = new CodeMemberProperty
		            {
		                Name = entry.Key,
		                Attributes = MemberAttributes.Public,
		                Type = new CodeTypeReference(designerPropertyType),
		                HasGet = true
		            };
		            // set the name to reflect the designer property currently being iterated
		            // set it's visibility to public to allow the VS.NET designer to reflect upon it
		            // set it's "return" type to reflect the Type we looked up earlier
		            // indicate the Property has a Getter
		            // add a simple Getter implementation to the getter (get { return <variableName>; ))
		            codeMemberProperty.GetStatements.Add(
		                new CodeMethodReturnStatement(new CodeVariableReferenceExpression("_" + entry.Key)));
		            // indicate the Property also has a Setter
		            codeMemberProperty.HasSet = true;
		            if (designerPropertyType == typeof(SchemaList))
		            {
		                var setStatementList = new List<CodeStatement>();

		                string varName = "_" + entry.Key;

		                // this._property = value;
		                setStatementList.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(varName),
		                    new CodePropertySetValueReferenceExpression()));

		                /*
                         * 
                         * add comment only, as this cannot correctly and neatly be
                         * done in CodeDOM
                         *  
                         * */
		                setStatementList.AddRange(
		                    new CodeCommentStatementCollection(
		                        new[]
		                        {
		                            new CodeCommentStatement("As CodeDOM is limited in it's implementation"),
		                            new CodeCommentStatement(
		                                "(see http://blogs.msdn.com/bclteam/archive/2005/03/16/396915.aspx)"),
		                            new CodeCommentStatement("I don't provide any code for storing the "),
		                            new CodeCommentStatement("values of the SchemaList. Sorry about that..."),
		                            new CodeCommentStatement("Hint: store the name and namespace of the various SchemaList"),
		                            new CodeCommentStatement(
		                                "objects and use those to fill the propertybag when storing the"),
		                            new CodeCommentStatement("SchemaList")
		                        }).Cast<CodeCommentStatement>());
		            }
		            else
		            {
		                codeMemberProperty.SetStatements.Add(new CodeAssignStatement(
		                    new CodeVariableReferenceExpression("_" + entry.Key),
		                    new CodePropertySetValueReferenceExpression()));
		            }

		            codeMemberProperty.Attributes = MemberAttributes.Public;
		            codeMemberProperty.Attributes |= MemberAttributes.Final;
		            codeTypeDeclaration.Members.Add(codeMemberProperty);
		        }

		        #endregion

		        // used to define #region elements to group output code
		        // used to define #endregion directive
		        CodeRegionDirective crEndDirective = new CodeRegionDirective(CodeRegionMode.End, null);

		        #region implement IBaseComponent

		        var crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IBaseComponent members");

		        #region IBaseComponent.Name

		        // first, add IBaseComponent.Name
		        codeMemberProperty = new CodeMemberProperty();

		        // add the #region directive
		        codeMemberProperty.StartDirectives.Add(crDirective);

		        codeMemberProperty.Name = "Name";
		        codeMemberProperty.HasSet = false;
		        codeMemberProperty.HasGet = true;
		        codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(
		            "resourceManager.GetString(\"COMPONENTNAME\", System.Globalization.CultureInfo.InvariantCulture)")));
		        codeMemberProperty.Type = new CodeTypeReference(typeof(String));
		        codeMemberProperty.CustomAttributes.Add(new CodeAttributeDeclaration("Browsable",
		            new CodeAttributeArgument(new CodePrimitiveExpression(false))));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("Name of the component", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberProperty.Attributes = MemberAttributes.Public;
		        codeMemberProperty.Attributes |= MemberAttributes.Final;
		        codeMemberProperty.ImplementationTypes.Clear();
		        codeMemberProperty.ImplementationTypes.Add(typeof(IBaseComponent));
		        codeTypeDeclaration.Members.Add(codeMemberProperty);

		        #endregion

		        #region IBaseComponent.Version

		        // next, add IBaseComponent.Version
		        codeMemberProperty = new CodeMemberProperty
		        {
		            Name = "Version",
		            HasSet = false,
		            HasGet = true
		        };
		        codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(
		            "resourceManager.GetString(\"COMPONENTVERSION\", System.Globalization.CultureInfo.InvariantCulture)")));
		        codeMemberProperty.Type = new CodeTypeReference(typeof(String));
		        codeMemberProperty.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("Version of the component", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberProperty.Attributes = MemberAttributes.Public;
		        codeMemberProperty.Attributes |= MemberAttributes.Final;
		        codeMemberProperty.ImplementationTypes.Clear();
		        codeMemberProperty.ImplementationTypes.Add(typeof(IBaseComponent));
		        codeTypeDeclaration.Members.Add(codeMemberProperty);

		        #endregion

		        #region IBaseComponent.Description

		        // next, add IBaseComponent.Description
		        codeMemberProperty = new CodeMemberProperty
		        {
		            Name = "Description",
		            HasSet = false,
		            HasGet = true
		        };
		        codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(
		            "resourceManager.GetString(\"COMPONENTDESCRIPTION\", System.Globalization.CultureInfo.InvariantCulture)")));
		        codeMemberProperty.Type = new CodeTypeReference(typeof(String));
		        codeMemberProperty.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("Description of the component", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberProperty.Attributes = MemberAttributes.Public;
		        codeMemberProperty.Attributes |= MemberAttributes.Final;
		        //codeMemberProperty.PrivateImplementationType.BaseType = "IBaseComponent";

                // the code generator thinks we inherit this from the first interface we implement
                // due to a "bug" in the CodeDOM. clear it.
                codeMemberProperty.ImplementationTypes.Clear();
		        codeMemberProperty.ImplementationTypes.Add(typeof(IBaseComponent));

		        // add the #endregion directive
		        codeMemberProperty.EndDirectives.Add(crEndDirective);

		        codeTypeDeclaration.Members.Add(codeMemberProperty);

		        #endregion

		        #endregion

		        #region implement IPersistPropertyBag

		        crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IPersistPropertyBag members");

		        #region IPersistPropertyBag.GetClassID

		        // next, add IPersistPropertyBag.GetClassID
		        CodeMemberMethod codeMemberMethod = new CodeMemberMethod();

		        // add the #region directive
		        codeMemberMethod.StartDirectives.Add(crDirective);

		        codeMemberMethod.Name = "GetClassID";
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Guid), "classid"));
		        codeMemberMethod.Parameters[0].Direction = FieldDirection.Out;
		        codeMemberMethod.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression("classid"),
		            new CodeObjectCreateExpression(typeof(Guid), new CodeSnippetExpression("\"" + componentGuid + "\""))));
		        codeMemberMethod.ReturnType = new CodeTypeReference(typeof(void));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("Gets class ID of component for usage from unmanaged code.",
		            true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"classid\">", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("Class ID of the component", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</param>", true));
		        codeMemberMethod.Attributes = MemberAttributes.Public;
		        codeMemberMethod.Attributes |= MemberAttributes.Final;
		        codeMemberMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #region IPersistPropertyBag.InitNew

		        // next, implement IPersistPropertyBag.InitNew
		        codeMemberMethod = new CodeMemberMethod {Name = "InitNew"};
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("not implemented", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Attributes = MemberAttributes.Public;
		        codeMemberMethod.Attributes |= MemberAttributes.Final;
		        codeMemberMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #region IPersistPropertyBag.Load

		        // next, implement IPersistPropertyBag.Load
		        codeMemberMethod = new CodeMemberMethod {Name = "Load"};
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "errlog"));

		        if (designerProperties.Count > 0)
		        {
		            // object val = null;
		            codeMemberMethod.Statements.Add(
		                new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));
		            foreach (var entry in designerProperties)
		            {
		                // val = this.ReadPropertyBag(pb, "property");
		                codeMemberMethod.Statements.Add(
		                    new CodeAssignStatement(
		                        new CodeVariableReferenceExpression("val"),
		                        new CodeMethodInvokeExpression(
		                            new CodeMethodReferenceExpression(
		                                thisObject, "ReadPropertyBag"), new CodeArgumentReferenceExpression("pb"), new CodeSnippetExpression("\"" + entry.Key + "\""))));

		                // typeof(variable)
		                Type designerPropertyType = entry.Value;
		                CodeAssignStatement assignment;

		                if (designerPropertyType == typeof(SchemaList))
		                {
		                    codeMemberMethod.Statements.Add(
		                        new CodeSnippetStatement(
		                            $"#error please implement IPersistPropertyBag.Load for property \"{entry.Key}\""));
		                }
		                else if (designerPropertyType == typeof(SchemaWithNone))
		                {
		                    // this._property = new SchemaWithNone((string) val);
		                    assignment =
		                        new CodeAssignStatement(
		                            new CodeFieldReferenceExpression(thisObject, "_" + entry.Key),
		                            new CodeObjectCreateExpression(
		                                designerPropertyType,
		                                new CodeCastExpression(
		                                    typeof(string),
		                                    new CodeVariableReferenceExpression("val"))));

		                    // if(val != null)
		                    codeMemberMethod.Statements.Add(
		                        new CodeConditionStatement(
		                            new CodeBinaryOperatorExpression(
		                                new CodeVariableReferenceExpression("val"),
		                                CodeBinaryOperatorType.IdentityInequality,
		                                new CodePrimitiveExpression(null)),
		                            assignment));
		                }
		                else
		                {
		                    // this._property = (type) val;
		                    assignment =
		                        new CodeAssignStatement(
		                            new CodeFieldReferenceExpression(thisObject, "_" + entry.Key),
		                            new CodeCastExpression(entry.Value,
		                                new CodeVariableReferenceExpression("val")));

		                    // if(val != null)
		                    codeMemberMethod.Statements.Add(
		                        new CodeConditionStatement(
		                            new CodeBinaryOperatorExpression(
		                                new CodeVariableReferenceExpression("val"),
		                                CodeBinaryOperatorType.IdentityInequality,
		                                new CodePrimitiveExpression(null)),
		                            assignment));
		                }
		            }
		        }

		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("Loads configuration properties for the component", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(
		            new CodeCommentStatement("<param name=\"pb\">Configuration property bag</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"errlog\">Error status</param>", true));
		        codeMemberMethod.ReturnType = new CodeTypeReference(typeof(void));
		        codeMemberMethod.Attributes = MemberAttributes.Public;
		        codeMemberMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #region IPersistPropertyBag.Save

		        // next, implement IPersistPropertyBag.Save
		        codeMemberMethod = new CodeMemberMethod {Name = "Save"};
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "fClearDirty"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "fSaveAllProperties"));

		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(
		            new CodeCommentStatement("Saves the current component configuration into the property bag", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(
		            new CodeCommentStatement("<param name=\"pb\">Configuration property bag</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"fClearDirty\">not used</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"fSaveAllProperties\">not used</param>", true));
		        codeMemberMethod.ReturnType = new CodeTypeReference(typeof(void));
		        codeMemberMethod.Attributes = MemberAttributes.Public;
		        codeMemberMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));

		        foreach (var entry in designerProperties)
		        {
		            Type designerPropertyType = entry.Value;

		            if (designerPropertyType == typeof(SchemaList))
		            {
		                codeMemberMethod.Statements.Add(
		                    new CodeSnippetStatement(
		                        $"#error please implement IPersistPropertyBag.Save for property \"{entry.Key}\""));
		            }
		            else if (designerPropertyType == typeof(SchemaWithNone))
		            {
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodInvokeExpression(
		                        thisObject, "WritePropertyBag", new CodeArgumentReferenceExpression("pb"), new CodePrimitiveExpression(entry.Key), new CodeFieldReferenceExpression(
		                            new CodeFieldReferenceExpression(thisObject, entry.Key), "SchemaName")));
		            }
		            else
		            {
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodInvokeExpression(
		                        thisObject, "WritePropertyBag", new CodeArgumentReferenceExpression("pb"), new CodePrimitiveExpression(entry.Key), new CodeFieldReferenceExpression(thisObject, entry.Key)));
		            }
		        }

		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #region safe version of ReadPropertyBag

		        crDirective = new CodeRegionDirective(CodeRegionMode.Start, "utility functionality");

		        // next, implement a safe version of ReadPropertyBag
		        codeMemberMethod = new CodeMemberMethod();

		        // add the #region directive
		        codeMemberMethod.StartDirectives.Add(crDirective);

		        codeMemberMethod.Name = "ReadPropertyBag";
		        codeMemberMethod.ReturnType = new CodeTypeReference(typeof(object));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propName"));

		        codeMemberMethod.Statements.Add(
		            new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));

		        // try running pb.Read()
		        CodeMethodInvokeExpression pbRead =
		            new CodeMethodInvokeExpression(
		                new CodeMethodReferenceExpression(
		                    new CodeArgumentReferenceExpression("pb"), "Read"), new CodeArgumentReferenceExpression("propName"), new CodeDirectionExpression(FieldDirection.Out,
		                    new CodeVariableReferenceExpression("val")), new CodePrimitiveExpression(0));

		        CodeMethodReturnStatement retVal = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("val"));
		        // if the choosen language supports try/catch blocks
		        if (codeGenerator.Supports(GeneratorSupport.TryCatchStatements))
		        {
		            CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();
		            tryCatch.TryStatements.Add(pbRead);
		            CodeCatchClause catchArgumentException =
		                new CodeCatchClause {CatchExceptionType = new CodeTypeReference(typeof(ArgumentException))};
		            catchArgumentException.Statements.Add(retVal);
		            tryCatch.CatchClauses.Add(catchArgumentException);

		            CodeCatchClause catchException = new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)));

		            // if no dice, throw an ApplicationException
		            catchException.Statements.Add(
		                new CodeThrowExceptionStatement(
		                    new CodeObjectCreateExpression(
		                        new CodeTypeReference(typeof(ApplicationException)), new CodeVariableReferenceExpression("e.Message"))));

		            // add the exception handling to the try/catch block
		            tryCatch.CatchClauses.Add(catchException);

		            codeMemberMethod.Statements.Add(tryCatch);
		        }
		        else
		        {
		            // unsafe
		            codeMemberMethod.Statements.Add(pbRead);
		        }

		        codeMemberMethod.Statements.Add(retVal);

		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("Reads property value from property bag", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Property bag</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"propName\">Name of property</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<returns>Value of the property</returns>", true));
		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #region safe version of WritePropertyBag

		        // next, implement a safe version of WritePropertyBag
		        codeMemberMethod = new CodeMemberMethod {Name = "WritePropertyBag"};
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propName"));
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "val"));

		        CodeMethodInvokeExpression pbWrite =
		            new CodeMethodInvokeExpression(
		                new CodeMethodReferenceExpression(
		                    new CodeArgumentReferenceExpression("pb"), "Write"), new CodeArgumentReferenceExpression("propName"), new CodeDirectionExpression(FieldDirection.Ref,
		                    new CodeVariableReferenceExpression("val")));

		        if (codeGenerator.Supports(GeneratorSupport.TryCatchStatements))
		        {
		            CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();
		            tryCatch.TryStatements.Add(pbWrite);
		            CodeCatchClause catchException = new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)));

		            // if no dice, throw an ApplicationException
		            catchException.Statements.Add(
		                new CodeThrowExceptionStatement(
		                    new CodeObjectCreateExpression(
		                        new CodeTypeReference(typeof(ApplicationException)), new CodeVariableReferenceExpression("e.Message"))));

		            // add the exception handling to the try/catch block
		            tryCatch.CatchClauses.Add(catchException);

		            codeMemberMethod.Statements.Add(tryCatch);
		        }
		        else
		        {
		            // unsafe
		            codeMemberMethod.Statements.Add(pbWrite);
		        }

		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("Writes property values into a property bag.", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Property bag.</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"propName\">Name of property.</param>", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"val\">Value of property.</param>", true));

		        // add the #endregion directive for the "utility functionality" methods
		        codeMemberMethod.EndDirectives.Add(crEndDirective);

		        // add the #endregion directive for the IPersistPropertyBag implementation
		        codeMemberMethod.EndDirectives.Add(crEndDirective);

		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #endregion

		        #region implement IComponentUI

		        crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IComponentUI members");

		        #region IComponentUI.Icon

		        // next, implement IComponentUI.Icon property
		        codeMemberProperty = new CodeMemberProperty();

		        // add the #region directive
		        codeMemberProperty.StartDirectives.Add(crDirective);

		        codeMemberProperty.Name = "Icon";
		        codeMemberProperty.HasSet = false;
		        codeMemberProperty.HasGet = true;

		        codeMemberProperty.GetStatements.Add(
		            new CodeMethodReturnStatement(
		                new CodeMethodInvokeExpression(
		                    new CodeCastExpression(
		                        typeof(Bitmap),
		                        new CodeMethodInvokeExpression(
		                            new CodeFieldReferenceExpression(thisObject, "resourceManager"),
		                            "GetObject", new CodeSnippetExpression("\"COMPONENTICON\""), new CodeSnippetExpression("System.Globalization.CultureInfo.InvariantCulture"))),
		                    "GetHicon")));

		        codeMemberProperty.Type = new CodeTypeReference("IntPtr");
		        codeMemberProperty.CustomAttributes.Add(
		            new CodeAttributeDeclaration(
		                "Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("Component icon to use in BizTalk Editor", true));
		        codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberProperty.Attributes = MemberAttributes.Public;
		        codeMemberProperty.Attributes |= MemberAttributes.Final;
		        codeMemberProperty.ImplementationTypes.Add(typeof(IComponentUI));

		        codeTypeDeclaration.Members.Add(codeMemberProperty);

		        #endregion

		        #region IComponentUI.Validate

		        // finally, implement IComponentUI.Validate
		        codeMemberMethod = new CodeMemberMethod {Name = "Validate"};
		        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "obj"));

		        // add sample code for the IComponentUI.Validate method
		        codeMemberMethod.Statements.Add(new CodeCommentStatement("example implementation:"));
		        codeMemberMethod.Statements.Add(new CodeCommentStatement("ArrayList errorList = new ArrayList();"));
		        codeMemberMethod.Statements.Add(new CodeCommentStatement("errorList.Add(\"This is a compiler error\");"));
		        codeMemberMethod.Statements.Add(new CodeCommentStatement("return errorList.GetEnumerator();"));

		        codeMemberMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));

		        codeMemberMethod.ReturnType = new CodeTypeReference(typeof(IEnumerator));

		        codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		        codeMemberMethod.Comments.Add(
		            new CodeCommentStatement("The Validate method is called by the BizTalk Editor during the build ", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("of a BizTalk project.", true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		        codeMemberMethod.Comments.Add(
		            new CodeCommentStatement("<param name=\"obj\">An Object containing the configuration properties.</param>",
		                true));
		        codeMemberMethod.Comments.Add(new CodeCommentStatement(
		            "<returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>",
		            true));

		        codeMemberMethod.Attributes = MemberAttributes.Public;
		        codeMemberMethod.Attributes |= MemberAttributes.Final;
		        codeMemberMethod.ImplementationTypes.Add(typeof(IComponentUI));

		        codeMemberMethod.EndDirectives.Add(crEndDirective);

		        codeTypeDeclaration.Members.Add(codeMemberMethod);

		        #endregion

		        #endregion

		        switch (componentCategory)
		        {
		            #region AssemblingSerializer (IAssemblerComponent)

		            case ComponentType.AssemblingSerializer:
		                // add another base type
		                codeTypeDeclaration.BaseTypes.Add(typeof(IAssemblerComponent));

                        // add a member variable to store the incoming message
                        cmf = new CodeMemberField(typeof(ArrayList), "_inmsgs")
                        {
                            InitExpression =
                            new CodeObjectCreateExpression(
                                typeof(ArrayList)), // no parameters

                            Attributes = MemberAttributes.Private
                        };
                        codeTypeDeclaration.Members.Add(cmf);

		                #region implement IAssemblerComponent

		                crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IAssemblerComponent members");

		                #region IAssemblerComponent.AddDocument

		                // implement the IAssemblerComponent.AddDocument member
		                codeMemberMethod = new CodeMemberMethod();

		                // add #region directive
		                codeMemberMethod.StartDirectives.Add(crDirective);

		                codeMemberMethod.Name = "AddDocument";
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
		                codeMemberMethod.ReturnType = new CodeTypeReference(typeof(void));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement("store the message for later use"));
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodInvokeExpression(
		                        new CodeMethodReferenceExpression(
		                            new CodeVariableReferenceExpression("_inmsgs"), "Add"), new CodeArgumentReferenceExpression("inmsg")));

		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("Adds the document inmsg to the list of messages that will be", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("included in the interchange.", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("<param name=\"pc\">the current pipeline context</param>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("<param name=\"inmsg\">the message to be added</param>", true));

		                codeMemberMethod.Attributes = MemberAttributes.Public;
		                codeMemberMethod.Attributes |= MemberAttributes.Final;
		                codeMemberMethod.ImplementationTypes.Add(typeof(IAssemblerComponent));
		                codeTypeDeclaration.Members.Add(codeMemberMethod);

                        #endregion

                        #region  IAssemblerComponent.Assemble

                        // implement the IAssemblerComponent.Assemble member
                        codeMemberMethod = new CodeMemberMethod
                        {
                            Name = "Assemble"
                        };
                        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                codeMemberMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement("TODO: implement assembling logic"));

		                codeMemberMethod.Statements.Add(
		                    new CodeCommentStatement(
		                        "BizTalk 2004 documentation: \"In this release of BizTalk Server 2004,"));
		                codeMemberMethod.Statements.Add(
		                    new CodeCommentStatement(
		                        "assembling functionality is not used, so BizTalk Server always passes"));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement("one document to the component input.\""));
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodReturnStatement(
		                        new CodeCastExpression(
		                            typeof(IBaseMessage),
		                            new CodeArrayIndexerExpression(
		                                new CodeVariableReferenceExpression("_inmsgs"),
		                                new CodePrimitiveExpression(0)))));

		                //new CodeSnippetExpression("return _inmsgs[0] as IBaseMessage"));

		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement(
		                    "Builds the interchange from the messages that were added by the previous method. ", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("Returns a pointer to the assembled message.", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("<param name=\"pc\">the current pipeline context</param>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<returns>the assembled message instance</returns>",
		                    true));

		                codeMemberMethod.Attributes = MemberAttributes.Public;
		                codeMemberMethod.Attributes |= MemberAttributes.Final;
		                codeMemberMethod.ImplementationTypes.Add(typeof(IAssemblerComponent));

		                // add #endregion
		                codeMemberMethod.EndDirectives.Add(crEndDirective);

		                codeTypeDeclaration.Members.Add(codeMemberMethod);

		                #endregion

		                #endregion

		                break;

		            #endregion

		            #region DisassemblingParser (IDisassemblerComponent, IProbeMessage)

		            case ComponentType.DisassemblingParser:
		                codeTypeDeclaration.BaseTypes.Add(typeof(IDisassemblerComponent));

                        // add a member variable to store the incoming message
                        cmf = new CodeMemberField(typeof(Queue), "_msgs")
                        {
                            InitExpression = new CodeObjectCreateExpression(typeof(Queue)),
                            Attributes = MemberAttributes.Private
                        };
                        cmf.Comments.Add(new CodeCommentStatement("<summary>", true));
		                cmf.Comments.Add(new CodeCommentStatement(
		                    "this variable will contain any message generated by the Disassemble method", true));
		                cmf.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeTypeDeclaration.Members.Add(cmf);

		                #region implement IDisassemblerComponent

		                crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IDisassemblerComponent members");

		                #region IDisassemblerComponent.GetNext

		                // implement the IDisassemblerComponent.GetNext member
		                codeMemberMethod = new CodeMemberMethod();

		                // add #region directive
		                codeMemberMethod.StartDirectives.Add(crDirective);

		                codeMemberMethod.Name = "GetNext";
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                codeMemberMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));

		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement(
		                    "called by the messaging engine until returned null, after disassemble has been called", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>",
		                    true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement(
		                    "<returns>an IBaseMessage instance representing the message created</returns>", true));

		                codeMemberMethod.Statements.Add(
		                    new CodeCommentStatement("get the next message from the Queue and return it"));

		                // IBaseMessage msg = null;
		                codeMemberMethod.Statements.Add(
		                    new CodeVariableDeclarationStatement(
		                        typeof(IBaseMessage), "msg", new CodePrimitiveExpression(null)));

		                // if(_msgs.Count > 0)
		                // {
		                //		msg = (IBaseMessage) _msgs.Dequeue();
		                // }
		                codeMemberMethod.Statements.Add(
		                    new CodeConditionStatement(
		                        new CodeBinaryOperatorExpression(
		                            new CodePropertyReferenceExpression(
		                                new CodeVariableReferenceExpression("_msgs"), "Count"),
		                            CodeBinaryOperatorType.GreaterThan,
		                            new CodePrimitiveExpression(0)),
		                        new CodeAssignStatement(
		                            new CodeVariableReferenceExpression("msg"),
		                            new CodeCastExpression(
		                                typeof(IBaseMessage),
		                                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("_msgs"),
		                                    "Dequeue")))));

		                // return msg;
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodReturnStatement(
		                        new CodeVariableReferenceExpression(
		                            "msg")));

		                codeMemberMethod.Attributes = MemberAttributes.Public;
		                codeMemberMethod.Attributes |= MemberAttributes.Final;
		                codeMemberMethod.ImplementationTypes.Add(typeof(IDisassemblerComponent));
		                codeTypeDeclaration.Members.Add(codeMemberMethod);

                        #endregion

                        #region IDisassemblerComponent.Disassemble

                        // implement the IDisassemblerComponent.Disassemble member
                        codeMemberMethod = new CodeMemberMethod
                        {
                            Name = "Disassemble"
                        };
                        codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
		                codeMemberMethod.ReturnType = new CodeTypeReference(typeof(void));

		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("called by the messaging engine when a new message arrives", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>",
		                    true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">the actual message</param>",
		                    true));

		                codeMemberMethod.Statements.Add(new CodeCommentStatement(""));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement("TODO: implement message retrieval logic"));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement(""));

		                // _msgs.Enqueue(inmsg)
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodInvokeExpression(
		                        new CodeVariableReferenceExpression("_msgs"), "Enqueue",
		                        new CodeArgumentReferenceExpression("inmsg")));

		                codeMemberMethod.Attributes = MemberAttributes.Public;
		                codeMemberMethod.Attributes |= MemberAttributes.Final;
		                codeMemberMethod.ImplementationTypes.Add(typeof(IDisassemblerComponent));

		                // add #endregion directive
		                codeMemberMethod.EndDirectives.Add(crEndDirective);

		                codeTypeDeclaration.Members.Add(codeMemberMethod);

		                #endregion

		                #endregion

		                // does the user wants a stub for IProbeMessage?
		                if (implementsIProbeMessage)
		                {
		                    #region IProbeMessage implementation

		                    crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IProbeMessage members");

		                    #region IProbeMessage.Probe

		                    codeTypeDeclaration.BaseTypes.Add(typeof(IProbeMessage));

		                    codeMemberMethod = new CodeMemberMethod();

		                    // add #region directive
		                    codeMemberMethod.StartDirectives.Add(crDirective);

		                    codeMemberMethod.Name = "Probe";
		                    codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                    codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
		                    codeMemberMethod.ReturnType = new CodeTypeReference(typeof(bool));

		                    codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                    codeMemberMethod.Comments.Add(
		                        new CodeCommentStatement("called by the messaging engine when a new message arrives", true));
		                    codeMemberMethod.Comments.Add(
		                        new CodeCommentStatement("checks if the incoming message is in a recognizable format", true));
		                    codeMemberMethod.Comments.Add(
		                        new CodeCommentStatement("if the message is in a recognizable format, only this component",
		                            true));
		                    codeMemberMethod.Comments.Add(
		                        new CodeCommentStatement("within this stage will be execute (FirstMatch equals true)", true));
		                    codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                    codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>",
		                        true));
		                    codeMemberMethod.Comments.Add(
		                        new CodeCommentStatement("<param name=\"inmsg\">the actual message</param>", true));

		                    codeMemberMethod.Statements.Add(new CodeCommentStatement(""));
		                    codeMemberMethod.Statements.Add(
		                        new CodeCommentStatement("TODO: check whether you're interested in the given message"));
		                    codeMemberMethod.Statements.Add(new CodeCommentStatement(""));

		                    // return true;
		                    codeMemberMethod.Statements.Add(
		                        new CodeMethodReturnStatement(
		                            new CodePrimitiveExpression(true)));

		                    codeMemberMethod.Attributes = MemberAttributes.Public;
		                    codeMemberMethod.Attributes |= MemberAttributes.Final;
		                    codeMemberMethod.ImplementationTypes.Add(typeof(IProbeMessage));

		                    // add #endregion directive
		                    codeMemberMethod.EndDirectives.Add(crEndDirective);

		                    codeTypeDeclaration.Members.Add(codeMemberMethod);

		                    #endregion

		                    #endregion
		                }

		                break;

		            #endregion

		            default:
		                codeTypeDeclaration.BaseTypes.Add(typeof(IComponent));

		                #region IComponent implementation

		                crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IComponent members");

		                #region IComponent.Execute

		                // implement IComponent.Execute member
		                codeMemberMethod = new CodeMemberMethod();

		                // add #region directive
		                codeMemberMethod.StartDirectives.Add(crDirective);

		                codeMemberMethod.Name = "Execute";
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
		                codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
		                codeMemberMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));

		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("Implements IComponent.Execute method.", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">Pipeline context</param>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">Input message</param>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<returns>Original input message</returns>", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("<remarks>", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("IComponent.Execute method is used to initiate", true));
		                codeMemberMethod.Comments.Add(
		                    new CodeCommentStatement("the processing of the message in this pipeline component.", true));
		                codeMemberMethod.Comments.Add(new CodeCommentStatement("</remarks>", true));

		                codeMemberMethod.Statements.Add(new CodeCommentStatement(""));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement("TODO: implement component logic"));
		                codeMemberMethod.Statements.Add(new CodeCommentStatement(""));

		                codeMemberMethod.Statements.Add(new CodeCommentStatement("this way, it's a passthrough pipeline component"));
		                codeMemberMethod.Statements.Add(
		                    new CodeMethodReturnStatement(
		                        new CodeArgumentReferenceExpression("inmsg")));

		                codeMemberMethod.Attributes = MemberAttributes.Public;
		                codeMemberMethod.Attributes |= MemberAttributes.Final;
		                codeMemberMethod.ImplementationTypes.Add(typeof(IComponent));

		                // add #endregion directive
		                codeMemberMethod.EndDirectives.Add(crEndDirective);

		                codeTypeDeclaration.Members.Add(codeMemberMethod);

		                #endregion

		                #endregion

		                break;
		        }

		        // add the interfaces we've implemented
		        foreach (string baseType in baseTypes)
		        {
		            codeTypeDeclaration.BaseTypes.Add(baseType);
		        }

		        // we're generating a public class
		        codeTypeDeclaration.IsClass = true;
		        codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;

		        // and add it to our namespace
		        codeNamespace.Types.Add(codeTypeDeclaration);

		        // tell the code generator to generate our sourcecode
		        codeGenerator.GenerateCodeFromNamespace(codeNamespace, sw, CodeGeneratorOptions);
		    }


		    // we're done, unless the user chose to implement the code in VB.NET...
            if (language == ImplementationLanguages.VbNet)
            {
                // remove unneeded Inherits ... code, move the ... to the Implements line
                // as we have no base class we want to use
                PostFixVbCode(fileName);
            }
		}
		#endregion

        #region postFixVbCode - does a postfix to move the "Inherits <classname>" to the interfaces
        /// <summary>
        /// this method alters the generated sourcecode (VB.NET) in order to remove the Inerits &lt;classname&gt;
        /// and move the class to the Implements row (the VB.NET) <see cref="C:ICodeGenerator"/> copies the first
        /// defined BaseType to the Inherits line, while we need it to be on the Implements line, as we don't
        /// inherit from any classes
        /// </summary>
        /// <param name="fileName">the file to modify</param>
        private static void PostFixVbCode(string fileName)
		{
            // this will hold the entire sourcecode file contents
            StringBuilder buffer = new StringBuilder();

            #region copy the content of the file into "buffer"
            // read the specified file
            using (StreamReader sr = new StreamReader(fileName))
            {
                // just a simple buffer
                string tmp;

                // read until we have no more input
                while ((tmp = sr.ReadLine()) != null)
                {
                    // and append an exact copy to our buffer instance
                    buffer.AppendLine(tmp);
                }
            }
            #endregion

            #region Replace the incorrect definition
            Regex replacer = new Regex(@"(?im)inherits\s+(?<inherits>.*)\r\n*[\t|\s]*(?<implements>implements\s+.*)\r|(?<implements>implements\s+.*)\r\n[\t|\s]*inherits\s+(?<inherits>.*)");

            MatchCollection matches = replacer.Matches(buffer.ToString());
            string interfaces = null, baseclasses = null;

            // get the implements / inherits groups
            foreach (Match match in matches)
            {
                if (interfaces == null && match.Groups["implements"].Value.Length > 0)
                {
                    interfaces = match.Groups["implements"].Value;
                }
                if (baseclasses == null && match.Groups["inherits"].Value.Length > 0)
                {
                    baseclasses = match.Groups["inherits"].Value;
                }
            }

            string replacement = null;

            foreach (Match match in matches)
            {
                if (match.Groups["implements"].Value.Length > 0)
                {
                    replacement = replacer.Replace(buffer.ToString(), $"{interfaces}, {baseclasses}");
                }
            }
            #endregion
             
            // "replacement" now contains the correct inheritance
            // write out the correct definition
            using (StreamWriter sw = new StreamWriter(fileName))
			{
				sw.Write(replacement);
			}
		}
		#endregion
	}
}