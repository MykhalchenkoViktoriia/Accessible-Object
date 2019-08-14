using ManagedWinapi.Accessibility;
using ManagedWinapi.Windows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibleObjects
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		SynchronizationContext uiContext = SynchronizationContext.Current;
		string role, name, value;

		public MainWindow()
		{
			InitializeComponent();
			this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(HandleEsc);

			MouseHook.Start();
			MouseHook.MouseAction += new EventHandler(Event);
		}

		// Обработка данных после нажатия левой клавиши мыши
		private void Event(object sender, EventArgs e)
		{
			if (CrossCursor)
			{
				ChangeCursor(@"C:\Windows\Cursors\aero_arrow.cur");
				CrossCursor = false;

				if (NativeMethods.GetCursorPos(out NativeMethods.POINT pt))
				{
					IntPtr hWnd = NativeMethods.WindowFromPoint(pt);
					AccessibleObjectID S = new AccessibleObjectID();
					SystemWindow window = new SystemWindow(hWnd);
					SystemAccessibleObject win = SystemAccessibleObject.FromWindow(window, S);
					SystemAccessibleObject smallAcc = null;

					smallAcc = ChildObj(win, pt.X, pt.Y);

					if (smallAcc != null)
						win = smallAcc;

					try
					{
						role = win.RoleString;
						if (role == null) role = "null";
					}
					catch
					{
						role = "null";
					}

					try
					{
						name = win.Name;
						if (name == null) name = "null";
					}
					catch
					{
						name = "null";
					}

					try
					{
						value = win.Value;
						if (value == null) value = "null";
					}
					catch
					{
						value = "null";
					}

					if ((role != "null") || (name != "null") || (value != "null"))
					{
						Thread t = new Thread(Run);
						t.Start(uiContext);
					}
				}
			}
		}

		/// <summary>
		/// This method runs in the main UI thread
		/// Creates element Control and adds in the treeView
		/// </summary>
		private void CreateControl(object state)
		{
			if (treeView.SelectedItem != null)
			{
				try

				{
					TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(treeView.SelectedItem) as TreeViewItem;

					ObservableCollection<Control> CNew = new ObservableCollection<Control>
					{
					new Control { Name = "Control ", AccName = name, Rolue = role, Value = value}
					};

					tvi.Items.Add(CNew);
				}
				catch { }
			}
		}

		private void Run(object state)
		{
			// достает контекст синхронизации из state'а
			SynchronizationContext uiContext = state as SynchronizationContext;
			uiContext.Post(CreateControl, null);
		}

		// Iterating over child elements. Returns an object under the mouse
		public SystemAccessibleObject ChildObj(SystemAccessibleObject accessibleObject, int X, int Y)
		{

			SystemAccessibleObject acc = null;
			SystemAccessibleObject[] childObject = accessibleObject.Children;

			foreach (SystemAccessibleObject child in childObject)
			{
				try
				{
					Rectangle loc = child.Location;

					if ((X <= (loc.X + loc.Width)) && (X >= loc.X) && (Y <= (loc.Y + loc.Height)) && (Y >= loc.Y))
					{
						acc = child;
						SystemAccessibleObject smallAcc = ChildObj(acc, X, Y);

						if (smallAcc != null) return smallAcc;

					}

				}
				catch { }
			}

			return acc;
		}

		private void Add_group_Click(object sender, RoutedEventArgs e)
		{
			ObservableCollection<Group> GNew = new ObservableCollection<Group>
			{
				new Group { Name = "Group " }
			};
			treeView.Items.Add(GNew);

		}

		private void Add_control_Click(object sender, RoutedEventArgs e)
		{
			ChangeCursor(@"C:\Windows\Cursors\cross_il.cur");
			CrossCursor = true;

		}

		private void Remove_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem target = GetTreeViewItemFromObject(treeView.ItemContainerGenerator, treeView.SelectedItem);
			ItemsControl parent = GetSelectedTreeViewItemParent(target);
			parent.Items.Remove(treeView.SelectedItem);
		}

		// Defines an item TreeViewItem
		public TreeViewItem GetTreeViewItemFromObject(ItemContainerGenerator container, object targetObject)
		{
			if (container.ContainerFromItem(targetObject) is TreeViewItem target) return target;
			for (int i = 0; i < container.Items.Count; i++)
				if ((container.ContainerFromIndex(i) as TreeViewItem)?.ItemContainerGenerator is ItemContainerGenerator childContainer)
					if (GetTreeViewItemFromObject(childContainer, targetObject) is TreeViewItem childTarget) return childTarget;
			return null;
		}

		private void HandleEsc(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				ChangeCursor(@"C:\Windows\Cursors\aero_arrow.cur");
				CrossCursor = false;
			}

		}

		// Changes the Cursor
		private static bool CrossCursor = false;
		private void ChangeCursor(string curFile)
		{
			Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors\", "Arrow", curFile);
			SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
		}

		const uint SPI_SETCURSORS = 0x0057;
		const uint SPIF_UPDATEINIFILE = 0x01;
		const uint SPIF_SENDCHANGE = 0x02;

		[DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
		}

		// Get TreeViewItem's Parent element
		public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(item);
			while (!(parent is TreeViewItem || parent is System.Windows.Controls.TreeView))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}

			return parent as ItemsControl;
		}
	}
}
