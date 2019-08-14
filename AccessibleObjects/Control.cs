using System.Collections.ObjectModel;

namespace AccessibleObjects
{
	class Control : Group
	{
		public static int control_num = 0;
		string name, accname, rolue, val;


		public ObservableCollection<Control> Controls { get; set; }

		public Control()
		{
			this.Controls = new ObservableCollection<Control>();
		}

		public override string Name
		{
			get { return name + ++control_num; }
			set { name = value; }
		}
		public string AccName
		{
			get { return accname; }
			set { accname = value; }
		}
		public string Rolue
		{
			get { return rolue; }
			set { rolue = value; }
		}
		public string Value
		{
			get { return val; }
			set { val = value; }
		}

	}
}
