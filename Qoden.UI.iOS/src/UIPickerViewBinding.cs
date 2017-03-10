using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Foundation;
using UIKit;
using ObjCRuntime;
using System.Linq;
using Qoden.Validation;
using Qoden.Binding;

namespace Qoden.UI.iOS
{
	/// <summary>
	/// Binding for UIPickerView DataSource and Delegate.
	/// </summary>
	/// 
	/// <remarks>
	/// 
	/// <para>
	/// <see cref="UIPickerViewBinding"/> manage list of columns <see cref="IPickerColumn"/>, each column binds to IList data source.
	/// <see cref="UIPickerViewBinding"/> provide helper extension methods for easier configuration. Real world example:
	/// <code>
	/// PickerColumn{EventData} titleColumn;
	/// //First create binding and configure one column. See <see cref="PickerColumnBuilder{T}"/> for details.
	/// //Title column configured to display EventData Title property
	/// Bindings.Picker(View.EventPicker)
	/// 	.Column(model.Events, out titleColumn)
	/// 	.Title((_, idx) => _.Title);
	/// //Then bind <see cref="PickerColumn{T}"/> SelectedItem property to model.Event property 
	/// Bindings.Property(model, _ => _.Event)
	/// 	.To(titleColumn.SelectedItemProperty())
	/// 	.AfterSourceUpdate((t, s) => LOG.Info("Selected event: {0}", s.Value != null ? s.Value.Title : "none"));
	/// </code>
	/// </para>
	///  
	/// <para>
	/// When <see cref="UIPickerViewBinding"/> set as DataSource in <see cref="UIPickerView"/> the view asks if bindings responds to 
	/// <see cref="UIPickerViewDataSource"/> and <see cref="UIPickerViewDelegate"/> selectors and cache (!) this information. 
	/// This means you MUST configure columns first and then set bindings as picker data source.
	/// When asked if <see cref="UIPickerViewBinding"/> responds to selector binding inspect it columns to see what 
	/// callbacks they have and based on that deside what selectors it support. All optional methods are listed in enum <see cref="UIPickerViewOption"/>.
	/// </para>
	/// </remarks>
	public class UIPickerViewBinding : NSObject, IUIPickerViewDataSource, IUIPickerViewDelegate, IBinding
	{
		//TODO implement crossplatform logging and restore this line
		//static readonly ILogger LOG = LoggerFactory.GetLogger<UIPickerViewBinding>();

		List<IPickerColumn> columns = new List<IPickerColumn>();

		/// <summary>
		/// Adds the column.
		/// </summary>
		/// <returns>The column.</returns>
		/// <param name="column">Column.</param>
		/// <param name="index">Index.</param>
		public void AddColumn(IPickerColumn column, int index = -1)
		{
			if (Bound) throw new InvalidOperationException("Cannot update bound DataSource");
			if (columns.Contains(column)) return;
			if (index == -1)
			{
				columns.Add(column);
			}
			else
			{
				columns.Insert(index, column);
			}
		}

		/// <summary>
		/// Removes the column.
		/// </summary>
		/// <returns>The column.</returns>
		/// <param name="column">Column.</param>
		public void RemoveColumn(IPickerColumn column)
		{
			if (Bound) throw new InvalidOperationException("Cannot update bound DataSource");
			if (columns.Contains(column))
			{
				columns.Remove(column);
			}
		}

		UIPickerView pickerView;
		/// <summary>
		/// Gets or sets the picker view.
		/// </summary>
		/// <value>The picker view.</value>
		public UIPickerView PickerView
		{
			get
			{
				return pickerView;
			}
			set
			{
				if (pickerView == value) return;
				bool rebind = pickerView != null && Bound;
				if (rebind) Unbind();
				pickerView = value;

				if (rebind)
				{
					Bind();
				}
			}
		}

		/// <summary>
		/// Reloads the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="pickerColumn">Picker column.</param>
		public void ReloadComponent(IPickerColumn pickerColumn)
		{
			var idx = columns.IndexOf(pickerColumn);
			if (idx >= 0 && PickerView != null)
				PickerView.ReloadComponent(idx);
		}

		/// <summary>
		/// Gets the bound.
		/// </summary>
		/// <value>The bound.</value>
		public bool Bound
		{
			get; private set;
		}

		/// <summary>
		/// Gets or sets the enabled.
		/// </summary>
		/// <value>The enabled.</value>
		public bool Enabled
		{
			get; set;
		}

		/// <summary>
		/// Bind this instance.
		/// </summary>
		public void Bind()
		{
			if (Bound) return;
			foreach (var column in columns)
				column.Bind();

			if (pickerView != null)
			{
				pickerView.DataSource = this;
				pickerView.WeakDelegate = this;
			}
		}

		/// <summary>
		/// Unbind this instance.
		/// </summary>
		public void Unbind()
		{
			if (!Bound) return;
			foreach (var column in columns)
				column.Unbind();
		}

		public void UpdateSource()
		{
			throw new NotSupportedException();
		}

		public void UpdateTarget()
		{
			if (PickerView != null) PickerView.ReloadAllComponents();
		}

		public static readonly IntPtr GetAttributedTitleSelector = Selector.GetHandle("pickerView:attributedTitleForRow:forComponent:");
		[Export("pickerView:attributedTitleForRow:forComponent:")]
		public NSAttributedString GetAttributedTitle(UIPickerView pickerView, nint row, nint component)
		{
			PickerView = pickerView;
			return columns[(int)component].GetAttributedTitle((int)row);
		}

		[Export("numberOfComponentsInPickerView:")]
		public nint GetComponentCount(UIPickerView pickerView)
		{
			PickerView = pickerView;
			return columns.Count;
		}

		public static readonly IntPtr GetComponentWidthSelector = Selector.GetHandle("pickerView:widthForComponent:");
		[Export("pickerView:widthForComponent:")]
		public nfloat GetComponentWidth(UIPickerView pickerView, nint component)
		{
			PickerView = pickerView;
			return columns[(int)component].GetWidth();
		}

		public static readonly IntPtr GetRowHeightSelector = Selector.GetHandle("pickerView:rowHeightForComponent:");
		[Export("pickerView:rowHeightForComponent:")]
		public nfloat GetRowHeight(UIPickerView pickerView, nint component)
		{
			PickerView = pickerView;
			return columns[(int)component].GetRowHeight();
		}

		[Export("pickerView:numberOfRowsInComponent:")]
		public nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			PickerView = pickerView;
			return columns[(int)component].GetRowsCount();
		}

		public static readonly IntPtr GetTitleSelector = Selector.GetHandle("pickerView:titleForRow:forComponent:");
		[Export("pickerView:titleForRow:forComponent:")]
		public string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			PickerView = pickerView;
			return columns[(int)component].GetTitle((int)row);
		}

		public static readonly IntPtr GetViewSelector = Selector.GetHandle("pickerView:viewForRow:forComponent:reusingView:");
		[Export("pickerView:viewForRow:forComponent:reusingView:")]
		public virtual UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			PickerView = pickerView;
			return columns[(int)component].GetView((int)row, view);
		}

		[Export("pickerView:didSelectRow:inComponent:")]
		public void Selected(UIPickerView pickerView, nint row, nint component)
		{
			PickerView = pickerView;
			columns[(int)component].SelectedIndex = (int)row;
		}

		UIPickerViewOption SelectorToOption(Selector sel)
		{
			if (sel.Handle == GetAttributedTitleSelector)
			{
				return UIPickerViewOption.AttributedTitle;
			}

			if (sel.Handle == GetRowHeightSelector)
			{
				return UIPickerViewOption.RowHeight;
			}

			if (sel.Handle == GetTitleSelector)
			{
				return UIPickerViewOption.Title;
			}

			if (sel.Handle == GetViewSelector)
			{
				return UIPickerViewOption.View;
			}

			if (sel.Handle == GetComponentWidthSelector)
			{
				return UIPickerViewOption.Width;
			}
			return UIPickerViewOption.None;
		}

		public override bool RespondsToSelector(Selector sel)
		{
			var option = SelectorToOption(sel);
			if (option != UIPickerViewOption.None)
			{
				if (columns.Count == 0)
				{
					//TODO implement crossplatform logging and restore this line
					//LOG.Error("UIPickerView inspect binding options but there are not binding columns. You likely will get unexpected results. " +
					          //"Consider configuring columns first and then setting binding as UIPickerView data source and delegate.");
					Console.WriteLine("UIPickerView inspect binding options but there are not binding columns. You likely will get unexpected results. " +
							  "Consider configuring columns first and then setting binding as UIPickerView data source and delegate.");
				}
				var supported = columns.Where(c => (c.SupportedOptions & option) != 0).Any();
				return supported;
			}
			else
			{
				return base.RespondsToSelector(sel);
			}
		}
	}

	[Flags]
	public enum UIPickerViewOption : int
	{
		None = 0x00,
		AttributedTitle = 0x01,
		Title = 0x01 << 1,
		Width = 0x01 << 2,
		RowHeight = 0x01 << 3,
		View = 0x01 << 4,
	}

	public interface IPickerColumn : INotifyPropertyChanged
	{
		void Bind();
		void Unbind();
		int SelectedIndex { get; set; }

		UIPickerViewOption SupportedOptions { get; }

		NSAttributedString GetAttributedTitle(int row);
		NSString GetTitle(int row);
		int GetWidth();
		int GetRowHeight();
		int GetRowsCount();
		UIView GetView(int row, UIView reusableView);
	}

	public class PickerColumn<T> : IPickerColumn
	{
		IList<T> dataSource;
		int selectedIndex = -1;

		public event PropertyChangedEventHandler PropertyChanged;

		public PickerColumn(UIPickerViewBinding owner)
		{
			Assert.Argument(owner, "owner").NotNull();
			Owner = owner;
		}

		public int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				if (selectedIndex != value)
				{
					selectedIndex = value;
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
					}
				}
			}
		}

		public UIPickerViewBinding Owner { get; private set; }

		public UIPickerViewOption SupportedOptions
		{
			get
			{
				var callbacks = UIPickerViewOption.None;
				if (AttributedTitle != null)
					callbacks |= UIPickerViewOption.AttributedTitle;
				if (Title != null)
					callbacks |= UIPickerViewOption.Title;
				if (Width != null)
					callbacks |= UIPickerViewOption.Width;
				if (RowHeight != null)
					callbacks |= UIPickerViewOption.RowHeight;
				if (View != null)
					callbacks |= UIPickerViewOption.View;
				return callbacks;
			}
		}

		public Func<T, int, NSAttributedString> AttributedTitle;
		public NSAttributedString GetAttributedTitle(int row)
		{
			if (AttributedTitle != null)
			{
				return AttributedTitle(dataSource[row], row);
			}
			return null;
		}

		public Func<T, int, string> Title;
		public NSString GetTitle(int row)
		{
			if (Title != null)
				return new NSString(Title(dataSource[row], row));
			return null;
		}

		public Func<int> Width;
		public int GetWidth()
		{
			if (Width != null)
				return Width();
			return -1;
		}

		public Func<int> RowHeight;
		public int GetRowHeight()
		{
			if (RowHeight != null)
				return RowHeight();
			return -1;
		}

		public int GetRowsCount()
		{
			return dataSource != null ? dataSource.Count : 0;
		}

		public Func<T, int, UIView, UIView> View;
		public UIView GetView(int row, UIView reusableView)
		{
			if (View != null)
				return View(dataSource[row], row, reusableView);
			return null;
		}

		public T SelectedItem
		{
			get { return SelectedIndex >= 0 && dataSource != null ? dataSource[SelectedIndex] : default(T); }
		}

		public IList<T> DataSource
		{
			get
			{
				return dataSource;
			}

			set
			{
				if (Owner.Bound) throw new InvalidOperationException("Cannot update bound data source");

				if (Equals(dataSource, value))
				{
					return;
				}

				dataSource = value;
			}
		}

		public void Bind()
		{
			if (dataSource is INotifyCollectionChanged)
				((INotifyCollectionChanged)dataSource).CollectionChanged += HandleCollectionChanged;
		}

		public void Unbind()
		{
			if (dataSource is INotifyCollectionChanged)
				((INotifyCollectionChanged)dataSource).CollectionChanged -= HandleCollectionChanged;
		}

		void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!Owner.Enabled) return;

			Owner.ReloadComponent(this);
		}
	}

	public class PickerColumnBuilder<T>
	{
		UIPickerViewBinding binding;
		PickerColumn<T> column;

		public PickerColumnBuilder(UIPickerViewBinding binding, IList<T> dataSource)
		{
			this.binding = binding;
			column = new PickerColumn<T>(binding);
			column.DataSource = dataSource;
			binding.AddColumn(column);
		}

		public PickerColumnBuilder<T> Title(Func<T, int, string> title)
		{
			column.Title = title;
			return this;
		}

		public PickerColumnBuilder<T> AttributedTitle(Func<T, int, NSAttributedString> title)
		{
			column.AttributedTitle = title;
			return this;
		}

		public PickerColumnBuilder<T> RowHeight(Func<int> rowHeight)
		{
			column.RowHeight = rowHeight;
			return this;
		}

		public PickerColumnBuilder<T> Width(Func<int> width)
		{
			column.Width = width;
			return this;
		}

		public PickerColumnBuilder<T> View(Func<T, int, UIView, UIView> view)
		{
			column.View = view;
			return this;
		}

		public PickerColumnBuilder<U> OtherColumn<U>(IList<U> dataSource)
		{
			return binding.Column(dataSource);
		}

		public PickerColumnBuilder<U> OtherColumn<U>(out PickerColumn<U> column, IList<U> dataSource)
		{
			return binding.Column(dataSource, out column);
		}

#pragma warning disable RECS0146 // Member hides static member from outer class
		public PickerColumn<T> Column
#pragma warning restore RECS0146 // Member hides static member from outer class
		{
			get
			{
				return column;
			}
		}

		public UIPickerViewBinding Binding { get { return binding; } }
	}

	public static class IPickerColumnBinding
	{
		public static IProperty<int> SelectedIndexProperty(this IPickerColumn column)
		{
			return column.Property(_ => _.SelectedIndex);
		}

		public static IProperty<T> SelectedItemProperty<T>(this PickerColumn<T> column)
		{
			return column.Property(_ => _.SelectedItem);
		}

		public static PickerColumnBuilder<T> Column<T>(this UIPickerViewBinding binding, IList<T> dataSource)
		{
			return new PickerColumnBuilder<T>(binding, dataSource);
		}

		public static PickerColumnBuilder<T> Column<T>(this UIPickerViewBinding binding, IList<T> dataSource, out PickerColumn<T> column)
		{
			var builder = new PickerColumnBuilder<T>(binding, dataSource);
			column = builder.Column;
			return builder;
		}

		public static UIPickerViewBinding Picker(this IBindingList bindings, UIPickerView view)
		{
			var binding = new UIPickerViewBinding();
			binding.PickerView = view;
			bindings.Add(binding);
			return binding;
		}
	}
}

