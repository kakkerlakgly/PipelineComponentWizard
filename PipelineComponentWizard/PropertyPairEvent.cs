using System;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	/// <summary>
	/// PropertyPairEvent class holds name and value for
	/// transportation in events. The remove field indicates
	/// that the value must be removed from the resultcollection
	/// </summary>
	public class PropertyPairEvent : EventArgs
	{
	    public PropertyPairEvent(string strName, object value)
		{
			Name = strName;
			Value = value;
		}

		public string Name { get; }

	    public object Value { get; }
    }
}
