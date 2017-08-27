using System;
using Qoden.Validation;

namespace Qoden.Binding
{
    public delegate void PropertyBindingAction(IPropertyBinding binding, ChangeSource changeSource);

    public enum ChangeSource
    {
        Source, Target
    };
	/// <summary>
	/// Binding between two properties (see <see cref="IProperty"/>). 
	/// </summary>
	public interface IPropertyBinding : IBinding
	{
		/// <summary>
		/// Get source object property. Usually this is model property.
		/// </summary>
		IProperty Source { get; set; }

		/// <summary>
		/// Gets target object property. Usually this is view property.
		/// </summary>
		IProperty Target { get; set; }

		/// <summary>
		/// Action to move data from Source property to Target. By default it perform 
		/// <code>
		/// Target.FieldValue = Source.FieldValue
		/// </code>
		/// </summary>
		PropertyBindingAction UpdateTargetAction { get; set; }

		/// <summary>
		/// Action to move data from Target property to Source. By default it perform 
		/// <code>
		/// Source.FieldValue = Target.FieldValue
		/// </code>
		/// </summary>
		/// <value>The target to source action.</value>
		PropertyBindingAction UpdateSourceAction { get; set; }
	}

	public class PropertyBinding : IPropertyBinding
	{
		private IDisposable _sourceSubscription;
		private IDisposable _targetSubscription;

		public PropertyBinding ()
		{
			Enabled = true;
			_updateSource = DefaultUpdateSource;
			_updateTarget = DefaultUpdateTarget;
		}

		static void DefaultUpdateSource (IPropertyBinding binding, ChangeSource change)
		{
			if (!binding.Source.IsReadOnly) {
				binding.Source.Value = binding.Target.Value;
			}
		}

		static void DefaultUpdateTarget (IPropertyBinding binding, ChangeSource change)
		{
			if (!binding.Target.IsReadOnly) {
				binding.Target.Value = binding.Source.Value;
			}
		}

		public void Bind ()
		{
			if (Bound)
				return;
			Assert.State (Source != null).IsTrue ("Source is not set");
			_sourceSubscription = Source.OnPropertyChange (Source_Change);
            if (Target != null && this.UpdatesSource())
                _targetSubscription = Target.OnPropertyChange(Target_Change);
		}

		void Source_Change (IProperty _)
		{
			UpdateTarget ();
		}

		void Target_Change (IProperty _)
		{
			UpdateSource ();
		}

		public void Unbind ()
		{
			if (!Bound)
				return;
			_sourceSubscription.Dispose ();
			_sourceSubscription = null;
			if (_targetSubscription != null) {
				_targetSubscription.Dispose ();
				_targetSubscription = null;
			}
		}

		public bool Bound => _sourceSubscription != null;

		public void UpdateTarget ()
		{	
            if (this.UpdatesTarget ())
				PerformAction (UpdateTargetAction);
		}

		public void UpdateSource ()
		{				
			if (this.UpdatesSource ())
				PerformAction (UpdateSourceAction);
		}

		bool _performingAction;

		void PerformAction (PropertyBindingAction action)
		{
			if (Enabled && !_performingAction) {
				Assert.State (Source != null).IsTrue ("Source is not set");
				try {
					_performingAction = true;
                    action (this, action == UpdateSourceAction ? ChangeSource.Source : ChangeSource.Target);
				} finally {
					_performingAction = false;
				}
			}
		}

		private IProperty _source;

		public IProperty Source {
			get => _source;
			set { 
				Assert.Argument (value, "value").NotNull ();
				Assert.State (Bound).IsFalse ();
				_source = value;
			}
		}

		private IProperty _target;

		public IProperty Target {
			get => _target;
			set {
				Assert.Argument (value, "value").NotNull ();
				Assert.State (Bound).IsFalse ();
				_target = value;
			}
		}

		private PropertyBindingAction _updateTarget;

		public PropertyBindingAction UpdateTargetAction {
			get => _updateTarget;
			set { 
				Assert.Argument (value, "value").NotNull ();
				_updateTarget = value;
			}
		}

		private PropertyBindingAction _updateSource;

		public PropertyBindingAction UpdateSourceAction {
			get => _updateSource;
			set { 
				Assert.Argument (value, "value").NotNull ();
				_updateSource = value;
			}
		}

		public bool Enabled { get; set; }
	}
	
	public static class PropertyBindingExtensions
	{
		static void DoNotUpdate (IPropertyBinding binding, ChangeSource source)
		{
		}

		public static void DontUpdateTarget (this IPropertyBinding binding)
		{
			binding.UpdateTargetAction = DoNotUpdate;
		}

		public static void DontUpdateSource (this IPropertyBinding binding)
		{
			binding.UpdateSourceAction = DoNotUpdate;
		}

		public static bool UpdatesSource (this IPropertyBinding binding)
		{
			return binding.UpdateSourceAction != DoNotUpdate;
		}

		public static bool UpdatesTarget (this IPropertyBinding binding)
		{
			return binding.UpdateTargetAction != DoNotUpdate;
		}
	}
}