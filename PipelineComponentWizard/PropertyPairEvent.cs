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
	    public PropertyPairEvent(string strName, object Value)
		{
			Name = strName;
			this.Value = Value;
		}
		
		public PropertyPairEvent(string strName, object Value, bool remove)
		{
			Name = strName;
			this.Value = Value;
			Remove = remove;
		}

		public string Name { set; get; }

	    public object Value { set; get; }

	    public bool Remove { get; set; }
	}
}
