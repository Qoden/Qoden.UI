using System;
using Qoden.Binding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qoden.Binding.Test
{
	[TestClass]
	public class BindingListTest
	{
        FakeUIControl ui = new FakeUIControl();
        FakeModel model = new FakeModel();
        BindingList bl = new BindingList();

        public BindingListTest()
        {
            model.Name = "This is my name";
        }

        [TestMethod]
        public void CanBindOneWay()
        {
            bl.Property(model, x => x.Name)
              .To(ui.Property(x => x.Text))
              .OneWay();
            bl.Bind();
            bl.UpdateTarget();
            Assert.AreEqual(model.Name, ui.Text);

            model.Name = "Updated value";
            Assert.AreEqual(model.Name, ui.Text);
        }


		[TestMethod]
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

		[TestMethod]
		public void Add()
		{
			
		}

		[TestMethod]
		public void Remove()
		{
			
		}

		[TestMethod]
		public void Clear()
		{
			
		}

	}
}
