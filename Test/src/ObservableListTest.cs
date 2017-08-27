/**
 * This test is adopted from Mono project.
 **/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Qoden.Binding;
using Xunit;

#pragma warning disable RECS0026 // Possible unassigned object created by 'new'			
namespace Qoden.Binding.Test
{
	public class ObservableListTest
	{
		[Fact]
		public void Constructor()
		{
			var list = new List<int> { 3 };
			var col = new ObservableList<int>(list);
			col.Add(5);
			Assert.Equal(1, list.Count);

			col = new ObservableList<int>((IEnumerable<int>)list);
			col.Add(5);
			Assert.Equal(1, list.Count);
		}

		[Fact]
		public void Constructor_Invalid()
		{

			try
			{
#pragma warning disable IDE0004 // Remove Unnecessary Cast
				new ObservableList<int>((List<int>)null);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
				Assert.True(false, "#1");
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new ObservableList<int>((IEnumerable<int>)null);
				Assert.True(false, "#2");
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Fact]
		public void Insert()
		{
			bool reached = false;
			ObservableList<int> col = new ObservableList<int>();
			col.CollectionChanged += (sender, e) =>
			{
				reached = true;
				Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
				Assert.Equal(0, e.NewStartingIndex);
				Assert.Equal(-1, e.OldStartingIndex);
				Assert.Equal(1, e.NewItems.Count);
				Assert.Equal(5, (int)e.NewItems[0]);
				Assert.Equal(null, e.OldItems);
			};
			col.Insert(0, 5);
			Assert.True(reached, "INS_5");
		}

		[Fact]
		public void RemoveAt()
		{
			bool reached = false;
			ObservableList<int> col = new ObservableList<int>();
			col.Insert(0, 5);
			col.CollectionChanged += (sender, e) =>
			{
				reached = true;
				Assert.Equal(NotifyCollectionChangedAction.Remove, e.Action);
				Assert.Equal(-1, e.NewStartingIndex);
				Assert.Equal(0, e.OldStartingIndex);
				Assert.Equal(null, e.NewItems);
				Assert.Equal(1, e.OldItems.Count);
				Assert.Equal(5, (int)e.OldItems[0]);
			};
			col.RemoveAt(0);
			Assert.True(reached, "REMAT_7");
		}

		[Fact]
		public void Move()
		{
			bool reached = false;
			ObservableList<int> col = new ObservableList<int>();
			col.Insert(0, 0);
			col.Insert(1, 1);
			col.Insert(2, 2);
			col.Insert(3, 3);
			col.CollectionChanged += (sender, e) =>
			{
				reached = true;
				Assert.Equal(NotifyCollectionChangedAction.Move, e.Action);
				Assert.Equal(3, e.NewStartingIndex);
				Assert.Equal(1, e.OldStartingIndex);
				Assert.Equal(1, e.NewItems.Count);
				Assert.Equal(1, e.NewItems[0]);
				Assert.Equal(1, e.OldItems.Count);
				Assert.Equal(1, e.OldItems[0]);
			};
			col.Move(1, 3);
			Assert.True(reached, "MOVE_8");
		}

		[Fact]
		public void Add()
		{
			ObservableList<char> collection = new ObservableList<char>();
			bool propertyChanged = false;
			List<string> changedProps = new List<string>();
			NotifyCollectionChangedEventArgs args = null;

			((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
			{
				propertyChanged = true;
				changedProps.Add(e.PropertyName);
			};

			collection.CollectionChanged += (sender, e) =>
			{
				args = e;
			};

			collection.Add('A');

			Assert.True(propertyChanged, "ADD_1");
			Assert.True(changedProps.Contains("Count"), "ADD_2");
			Assert.True(changedProps.Contains("Item[]"), "ADD_3");

			CollectionChangedEventValidators.ValidateAddOperation(args, new char[] { 'A' }, 0, "ADD_4");
		}

		[Fact]
		public void Remove()
		{
			ObservableList<char> collection = new ObservableList<char>();
			bool propertyChanged = false;
			List<string> changedProps = new List<string>();
			NotifyCollectionChangedEventArgs args = null;

			collection.Add('A');
			collection.Add('B');
			collection.Add('C');

			((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
			{
				propertyChanged = true;
				changedProps.Add(e.PropertyName);
			};

			collection.CollectionChanged += (sender, e) =>
			{
				args = e;
			};

			collection.Remove('B');

			Assert.True(propertyChanged, "REM_1");
			Assert.True(changedProps.Contains("Count"), "REM_2");
			Assert.True(changedProps.Contains("Item[]"), "REM_3");

			CollectionChangedEventValidators.ValidateRemoveOperation(args, new char[] { 'B' }, 1, "REM_4");
		}

		[Fact]
		public void Set()
		{
			ObservableList<char> collection = new ObservableList<char>();
			bool propertyChanged = false;
			List<string> changedProps = new List<string>();
			NotifyCollectionChangedEventArgs args = null;

			collection.Add('A');
			collection.Add('B');
			collection.Add('C');

			((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
			{
				propertyChanged = true;
				changedProps.Add(e.PropertyName);
			};

			collection.CollectionChanged += (sender, e) =>
			{
				args = e;
			};

			collection[2] = 'I';

			Assert.True(propertyChanged, "SET_1");
			Assert.True(changedProps.Contains("Item[]"), "SET_2");

			CollectionChangedEventValidators.ValidateReplaceOperation(args, new char[] { 'C' }, new char[] { 'I' }, 2, "SET_3");
		}

		[Fact]
		public void Reentrant()
		{
			ObservableList<char> collection = new ObservableList<char>();
			bool propertyChanged = false;
			List<string> changedProps = new List<string>();
			NotifyCollectionChangedEventArgs args = null;

			collection.Add('A');
			collection.Add('B');
			collection.Add('C');

			PropertyChangedEventHandler pceh = (sender, e) =>
			{
				propertyChanged = true;
				changedProps.Add(e.PropertyName);
			};

			// Adding a PropertyChanged event handler
			((INotifyPropertyChanged)collection).PropertyChanged += pceh;

			collection.CollectionChanged += (sender, e) =>
			{
				args = e;
			};

			collection.CollectionChanged += (sender, e) =>
			{
				// This one will attempt to break reentrancy
				try
				{
					collection.Add('X');
					Assert.True(false, "Reentrancy should not be allowed.");
				}
				catch (InvalidOperationException)
				{
				}
			};

			collection[2] = 'I';

			Assert.True(propertyChanged, "REENT_1");
			Assert.True(changedProps.Contains("Item[]"), "REENT_2");

			CollectionChangedEventValidators.ValidateReplaceOperation(args, new char[] { 'C' }, new char[] { 'I' }, 2, "REENT_3");

			// Removing the PropertyChanged event handler should work as well:
			((INotifyPropertyChanged)collection).PropertyChanged -= pceh;
		}

		//Private test class for protected members of ObservableList
		private class ObservableListTestHelper : ObservableList<char>
		{
			internal void DoubleEnterReentrant()
			{
				IDisposable object1 = BlockReentrancy();
				IDisposable object2 = BlockReentrancy();

				Assert.Same(object1, object2);

				//With double block, try the reentrant:
				NotifyCollectionChangedEventArgs args = null;

				CollectionChanged += (sender, e) =>
				{
					args = e;
				};

				// We need a second callback for reentrancy to matter
				CollectionChanged += (sender, e) =>
				{
					// Doesn't need to do anything; just needs more than one callback registered.
				};

				// Try adding - this should cause reentrancy, and fail
				try
				{
					Add('I');
					Assert.True(false, "Reentrancy should not be allowed. -- #2");
				}
				catch (InvalidOperationException)
				{
				}

				// Release the first reentrant
				object1.Dispose();

				// Try adding again - this should cause reentrancy, and fail again
				try
				{
					Add('J');
					Assert.True(false, "Reentrancy should not be allowed. -- #3");
				}
				catch (InvalidOperationException)
				{
				}

				// Release the reentrant a second time
				object1.Dispose();

				// This last add should work fine.
				Add('K');
				CollectionChangedEventValidators.ValidateAddOperation(args, new char[] { 'K' }, 0, "REENTHELP_1");
			}
		}

		[Fact]
		public void ReentrantReuseObject()
		{
			ObservableListTestHelper helper = new ObservableListTestHelper();

			helper.DoubleEnterReentrant();
		}

		[Fact]
		public void Clear()
		{
			List<char> initial = new List<char>();

			initial.Add('A');
			initial.Add('B');
			initial.Add('C');

			ObservableList<char> collection = new ObservableList<char>(initial);
			bool propertyChanged = false;
			List<string> changedProps = new List<string>();
			NotifyCollectionChangedEventArgs args = null;

			((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
			{
				propertyChanged = true;
				changedProps.Add(e.PropertyName);
			};

			collection.CollectionChanged += (sender, e) =>
			{
				args = e;
			};

			collection.Clear();

			Assert.True(propertyChanged, "CLEAR_1");
			Assert.True(changedProps.Contains("Count"), "CLEAR_2");
			Assert.True(changedProps.Contains("Item[]"), "CLEAR_3");

			CollectionChangedEventValidators.ValidateResetOperation(args, "CLEAR_4");
		}
	}

	internal static class CollectionChangedEventValidators
	{
		#region Validators

		internal static void AssertEquivalentLists(IList expected, IList actual)
		{
			if (expected == null)
			{
				Assert.Null(actual);
				return;
			}
			else
				Assert.NotNull(actual);

			Assert.Equal(expected.Count, actual.Count);

			for (int i = 0; i < expected.Count; i++)
				Assert.Equal(expected[i], actual[i]);
		}

		private static void ValidateCommon(NotifyCollectionChangedEventArgs args, NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newIndex, int oldIndex, string message)
		{
			Assert.NotNull(args);

			Assert.Equal(action, args.Action);

			AssertEquivalentLists(newItems, args.NewItems);
			AssertEquivalentLists(oldItems, args.OldItems);

			Assert.Equal(newIndex, args.NewStartingIndex);
			Assert.Equal(oldIndex, args.OldStartingIndex);
		}

		internal static void ValidateResetOperation(NotifyCollectionChangedEventArgs args, string message)
		{
			ValidateCommon(args, NotifyCollectionChangedAction.Reset, null, null, -1, -1, message);
		}

		internal static void ValidateAddOperation(NotifyCollectionChangedEventArgs args, IList newItems, string message)
		{
			ValidateAddOperation(args, newItems, -1, message);
		}

		internal static void ValidateAddOperation(NotifyCollectionChangedEventArgs args, IList newItems, int startIndex, string message)
		{
			ValidateCommon(args, NotifyCollectionChangedAction.Add, newItems, null, startIndex, -1, message);
		}

		internal static void ValidateRemoveOperation(NotifyCollectionChangedEventArgs args, IList oldItems, string message)
		{
			ValidateRemoveOperation(args, oldItems, -1, message);
		}

		internal static void ValidateRemoveOperation(NotifyCollectionChangedEventArgs args, IList oldItems, int startIndex, string message)
		{
			ValidateCommon(args, NotifyCollectionChangedAction.Remove, null, oldItems, -1, startIndex, message);
		}

		internal static void ValidateReplaceOperation(NotifyCollectionChangedEventArgs args, IList oldItems, IList newItems, string message)
		{
			ValidateReplaceOperation(args, oldItems, newItems, -1, message);
		}

		internal static void ValidateReplaceOperation(NotifyCollectionChangedEventArgs args, IList oldItems, IList newItems, int startIndex, string message)
		{
			ValidateCommon(args, NotifyCollectionChangedAction.Replace, newItems, oldItems, startIndex, startIndex, message);
		}

		internal static void ValidateMoveOperation(NotifyCollectionChangedEventArgs args, IList changedItems, int newIndex, int oldIndex, string message)
		{
			ValidateCommon(args, NotifyCollectionChangedAction.Move, changedItems, changedItems, newIndex, oldIndex, message);
		}

		#endregion
	}
}
