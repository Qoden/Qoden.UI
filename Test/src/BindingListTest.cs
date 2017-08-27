using System;
using Qoden.Binding;
using Qoden.UI.Test;
using Xunit;

namespace Qoden.Binding.Test
{
	public class BindingListTest
	{
        FakeUiControl ui = new FakeUiControl();
        FakeModel model = new FakeModel();
        BindingList bl = new BindingList();

        public BindingListTest()
        {
            model.Name = "This is my name";
        }

        [Fact]
        public void CanBindOneWay()
        {
            bl.Property(model, x => x.Name)
              .To(ui.Property(x => x.Text))
              .OneWay();
            bl.Bind();
            bl.UpdateTarget();
            Assert.Equal(model.Name, ui.Text);

            model.Name = "Updated value";
            Assert.Equal(model.Name, ui.Text);
        }


		[Fact]
		public void Constructor()
		{
			var bl = new BindingList();
			//Add
			//Remove
			//Clear
			//Count
			//UpdateTarget
			//UpdateSource
			//Bind
			//Unbind
			//Enabled
			//Bound
		}

		[Fact]
		public void Add()
		{
			
		}

		[Fact]
		public void Remove()
		{
			
		}

		[Fact]
		public void Clear()
		{
			
		}

	}
}
