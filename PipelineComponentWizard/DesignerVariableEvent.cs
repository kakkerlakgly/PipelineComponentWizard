using System;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    /// <summary>
    /// DesignerVariableEvent class holds name and type for
    /// transportation in events.
    /// </summary>
    public class DesignerVariableEvent : EventArgs
	{
	    public readonly DesignerVariable Variable;

	    public DesignerVariableEvent(DesignerVariable variable)
	    {
	        Variable = variable;
	    }

    }
}
