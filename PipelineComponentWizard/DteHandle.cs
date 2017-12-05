using System;
using System.Runtime.InteropServices;
using EnvDTE;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    public class DteHandle
    {
        //EnvDTE.Project proj;
        //EnvDTE.Configuration config;
        //EnvDTE.Properties configProps;
        //EnvDTE.Property prop;
        readonly DTE _dte = Marshal.GetActiveObject("VisualStudio.DTE.14.0") as DTE;
        public Project GetProject(String name)
        {
            foreach (Project item in _dte.Solution.Projects)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}