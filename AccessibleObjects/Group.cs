using System.Collections.ObjectModel;

namespace AccessibleObjects
{
	class Group
	{
		public static int group_num = 0;
		public ObservableCollection<Group> Groups { get; set; }
		string name;


		public Group()
		{
			this.Groups = new ObservableCollection<Group>();
		}

		public virtual string Name
		{
			get { return name + ++group_num; }
			set { name = value; }
		}
	}
}
