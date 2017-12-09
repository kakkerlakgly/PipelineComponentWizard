using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    public class DesignerVariable
    {
        public readonly string Name;
        public readonly Type Type;

        public DesignerVariable(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return Name + " (" + Type.Name + ')';
        }
    }
}
