using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qoden.UI.Test
{
    [TestClass]
    public class ViewHierarchyTest
    {
        public class TestView1 : FakeView
        {
            [View]
            public FakeView View1 { get; private set; }
            [View]
            public QView View2 { get; private set; }
        }

        [TestMethod]
        public void ViewBuilding()
        {
            var view = new TestView1();
            var subviews = new ViewHierarchy(view, new FakeViewHierarchyBuilder());
            Assert.AreEqual(2, view.Subviews.Count, "View marked with View attribute added into parent view");
            Assert.AreEqual(2, subviews.Subviews.Count(), "View hierarchy contains all views marked with View attribute");

            Assert.IsNotNull(view.View1, "Child views created and set");
            Assert.IsNotNull(view.View2, "View wrappers created and set");
            Assert.IsNotNull(view.View2.PlatformView, "Platform views created in view wrappers");
        }

        public class TestView2 : FakeView
        {
            [TestTheme("Style1", "Style2")]
            public FakeView View1 { get; private set; } = new FakeView();

            [TestTheme("Style1", "Style3")]
            public QView View2 { get; private set; } = new QView(new FakeView());
        }

        [TestMethod]
        public void Decorating()
        {
            var view = new TestView2();
            var subviews = new ViewHierarchy(view, new FakeViewHierarchyBuilder());

            CollectionAssert.AreEqual(new []{"Applied", "Applied"}, new []{view.View1.Info["Style1"], view.View1.Info["Style2"] },
                                     "Multiple styles applied");

            Assert.IsFalse(view.View2.PlatformView.Info.ContainsKey("Style1"),
                           "Style1 not applied to view wrapper due to type mismatch");
            Assert.AreEqual("Applied", view.View2.PlatformView.Info["Style3"], 
                            "Stype3 applied to view wrapper");
        }

        public class TestTheme
        {
            public static void Style1(FakeView v)
            {
                v.Info["Style1"] = "Applied";
            }

            public static void Style2(FakeView v)
            {
                v.Info["Style2"] = "Applied";
            }

            public static void Style3(QView v)
            {
                v.PlatformView.Info["Style3"] = "Applied";
            }
        }

        public class TestThemeAttribute : DecoratorAttribute
        {
            public TestThemeAttribute(params string[] methods) : base(typeof(TestTheme), methods)
            {
            }
        }
    }
}
