using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Qoden.UI;

namespace Example
{
    public partial class ProfileForm : QodenView
    {
        [View]
        public QGroupedListView Form { get; private set; }

        [View(addToParent: false)]
        [Theme("Headline")]
        public QTextView Title { get; private set; }
        [View(addToParent: false)]
        [Theme("TextField")]
        public TextField FirstName { get; private set; }
        [View(addToParent: false)]
        [Theme("TextField")]
        public TextField LastName { get; private set; }
        [View(addToParent: false)]
        [Theme("TextField")]
        public TextField BirthDate { get; private set; }
        [View(addToParent: false)]
        [Theme("TextField")]
        public TextField Email { get; private set; }
        [View(addToParent: false)]
        [Theme("TextField")]
        public TextField PhoneNumber { get; private set; }
        [View(addToParent: false)]
        [Theme("ButtonField")]
        public TextViewWithIcon NotificationSettings { get; private set; }
        [View(addToParent: false)]
        [Theme("ButtonField")]
        public TextViewWithIcon ChangePassword { get; private set; }
        [View(addToParent: false)]
        [Theme("ButtonField")]
        public TextViewWithIcon LogOut { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();

#if __ANDROID__
            FirstName.LayoutParameters.Width = LayoutParams.MatchParent;
            LastName.LayoutParameters.Width = LayoutParams.MatchParent;
            BirthDate.LayoutParameters.Width = LayoutParams.MatchParent;
            Email.LayoutParameters.Width = LayoutParams.MatchParent;
            PhoneNumber.LayoutParameters.Width = LayoutParams.MatchParent;
            NotificationSettings.LayoutParameters.Width = LayoutParams.MatchParent;
            ChangePassword.LayoutParameters.Width = LayoutParams.MatchParent;
            LogOut.LayoutParameters.Width = LayoutParams.MatchParent;
#endif

            Title.SetText(Theme.HeadlineText("Profile"));
            Title.LayoutBox().AutoSize().Layout();
            this.SetBackgroundColor(new RGB(17, 17, 17));
            FirstName.Label.SetText("First Name");
            LastName.Label.SetText("Last Name");
            BirthDate.Label.SetText("Birth Date");
            Email.Label.SetText("Email");
            PhoneNumber.Label.SetText("Phone Number");
            NotificationSettings.Title.SetText("Notification Settings");
            ChangePassword.Title.SetText("Change Password");
            LogOut.Title.SetText("Log Out");

            var views = new List<List<QView>>
            {
                new List<QView> {new QView(FirstName), new QView(LastName), new QView(BirthDate)},
                new List<QView> {new QView(Email), new QView(PhoneNumber)},
                new List<QView> {new QView(NotificationSettings), new QView(ChangePassword), new QView(LogOut)},
            };
            Form.SetContent(new FormContent(views, Builder));

#if __IOS__
            var scroller = new KeyboardScroller();
            scroller.ScrollView = Form.PlatformView;
            Form.PlatformView.TableHeaderView = Title.PlatformView;
            Form.SetBackgroundColor(new RGB(17, 17, 17));
            Form.PlatformView.SeparatorStyle = UIKit.UITableViewCellSeparatorStyle.SingleLine;
            Form.PlatformView.SeparatorColor = UIKit.UIColor.Red;
#endif

            var textFields = new[]
            {
                FirstName, LastName, BirthDate, Email, PhoneNumber
            };
            AlignTextFieldLabelsWidth(new SizeF(10000, 10000), textFields);
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            layout.View(Form)
                .Left(0).Right(0).Top(30).Bottom(0);
        }

        void AlignTextFieldLabelsWidth(SizeF bounds, TextField[] textFields)
        {
            var maxLabelWidth = float.MinValue;
            foreach (var view in textFields)
            {
                var labelSize = view.Label.SizeThatFits(bounds);
                maxLabelWidth = Math.Max(maxLabelWidth, labelSize.Width);
            }

            foreach (var view in textFields)
            {
                view.LabelWidth = maxLabelWidth;
            }
        }

        class FormContent : GroupedListContent
        {
            public FormContent(IList<List<QView>> content, IViewHierarchyBuilder builder) : base(builder)
            {
                DataSource = content;
            }

#if __ANDROID__
            public override int ChildTypeCount {
                get {
                    return DataSource.Sum(x => x.Count);
                }
            }
            public override int GroupTypeCount => 1;
            public override long GetChildId(int groupPosition, int childPosition)
            {
                return GetChildType(groupPosition, childPosition);
            }
            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return false;
            }
            public override long GetGroupId(int groupPosition)
            {
                return groupPosition;
            }
#endif

#if __IOS__
            [Foundation.Export("tableView:heightForHeaderInSection:")]
            public nfloat EstimatedHeightForHeader(UIKit.UITableView tableView, nint section)
            {
                return Theme.FormHeaderHeight;
            }

	        [Foundation.Export("tableView:heightForFooterInSection:")]
	        public virtual nfloat HeightForFooter(UIKit.UITableView tableView, nint section)
	        {
	            return (nfloat)0.01f;
	        }
#endif

            public IList<List<QView>> DataSource { get; set; }

            public override int GroupCount => DataSource.Count;

            public override QView CreateChildView(int group, int child, IViewHierarchyBuilder builder)
            {
                return DataSource[group][child];
            }

            public override QView CreateGroupView(int group, IViewHierarchyBuilder builder)
            {
                var header = builder.MakeView<TextViewWithIcon>();
                Theme.FormHeader(header);
#if __ANDROID__
                header.LayoutParameters.Width = LayoutParams.MatchParent;
#endif
                return new QView(header);
            }

            public override void FillChildView(int group, int child, QView item)
            {
            }

            public override void FillGroupView(int group, QView section)
            {
                var view = section.PlatformView as TextViewWithIcon;
                switch (group)
                {
                    case 0:
                        view.Title.SetText("Personal Information");
                        break;
                    case 1:
                        view.Title.SetText("Contact Information");
                        break;
                    case 2:
                        view.Title.SetText("Actions");
                        break;
                }
            }

            public override int GetChildrenCount(int groupPosition)
            {
                return DataSource[groupPosition].Count;
            }

            public override int GetChildType(int groupPosition, int childPosition)
            {
                //All cells are unique and has different ids
                //Return child view index in DataSource as id otherwise it breaks in Android
                int before = 0;
                for (int i = 0; i < groupPosition - 1; ++i)
                {
                    before += DataSource[i].Count;
                }
                return before + childPosition;
            }

            public override int GetGroupType(int groupPosition)
            {
                return 0;
            }
        }
    }
}
