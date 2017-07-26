﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Qoden.UI;

namespace Example
{
    public partial class ProfileForm : QodenView
    {
        public ILabeledField[] TextFields { get; private set; }

        [View]
        [Theme("FormList")]
        public QGroupedListView Form { get; private set; }

        [View]
        [Theme("Headline")]
        public FormHeaderView Title { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "TextField")]
        public TextField FirstName { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "TextField")]
        public TextField LastName { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "TextField")]
        public DateTimeTextField BirthDate { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "TextField")]
        public TextField Email { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "TextField")]
        public PhoneNumberField PhoneNumber { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "ButtonField")]
        public TextViewWithIcon NotificationSettings { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "ButtonField")]
        public TextViewWithIcon ChangePassword { get; private set; }
        [View(addToParent: false)]
        [Theme("Field", "ButtonField")]
        public TextViewWithIcon LogOut { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            this.SetBackgroundColor(Theme.Colors.BG);

            Title.Title.SetText(Theme.HeadlineText("Profile"));

            FirstName.Label.SetText("First Name");
            FirstName.Text.SetHintText(Theme.TableTextFieldPlaceholderText("Add your name"));

            LastName.Label.SetText("Last Name");
            LastName.Text.SetHintText(Theme.TableTextFieldPlaceholderText("Add your name"));

            BirthDate.Label.SetText("Birth Date");
            BirthDate.Text.SetHintText(Theme.TableTextFieldPlaceholderText("Choose your birthdate"));

            Email.Label.SetText("Email");
            Email.Text.SetHintText(Theme.TableTextFieldPlaceholderText("Add your email"));

            PhoneNumber.Label.SetText("Phone Number");
            PhoneNumber.Text.SetHintText(Theme.TableTextFieldPlaceholderText("Add your phone number"));


            NotificationSettings.Title.SetText("Notification Settings");
            NotificationSettings.Icon.SetText(Icons.Notification);

            ChangePassword.Title.SetText("Change Password");
            ChangePassword.Icon.SetText(Icons.ChangePassword);

            LogOut.Title.SetText("Log Out");
            LogOut.Icon.SetText(Icons.LogOut);

            var views = new List<List<QView>>
            {
                new List<QView> {new QView(FirstName), new QView(LastName), new QView(BirthDate)},
                new List<QView> {new QView(Email), new QView(PhoneNumber)},
                new List<QView> {new QView(NotificationSettings), new QView(ChangePassword)},
                new List<QView> {new QView(LogOut)}
            };
            Form.SetContent(new FormContent(views, this, Builder));

            TextFields = new ILabeledField[]
            {
                FirstName, LastName, BirthDate, Email, PhoneNumber
            };
            AlignTextFieldLabelsWidth(new SizeF(10000, 10000), TextFields);

            PlatformCreate();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            PlatformDispose();
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            var title = layout.View(Title)
                  .Top(TopInset).Left(0).Right(0).Height(44);
            layout.View(Form)
                .Below(title.LayoutBounds).Left(0).Right(0).Bottom(0);
        }

        void AlignTextFieldLabelsWidth(SizeF bounds, ILabeledField[] textFields)
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
            ProfileForm _form;
            public FormContent(IList<List<QView>> content, ProfileForm form, IViewHierarchyBuilder builder) : base(builder)
            {
                DataSource = content;
                _form = form;
            }

#if __ANDROID__
            public override int ChildTypeCount
            {
                get
                {
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
                return section > 1 ? Theme.FormEmptyHeaderHeight : Theme.FormHeaderHeight;
            }

            [Foundation.Export("tableView:heightForFooterInSection:")]
            public virtual nfloat HeightForFooter(UIKit.UITableView tableView, nint section)
            {
                return (nfloat)0.01f;
            }

            [Foundation.Export("scrollViewDidScroll:")]
            public virtual void ScrollViewDidScroll(UIKit.UIScrollView scrollView)
            {
                _form.Form_Scrolled();
            }
#endif

            public IList<List<QView>> DataSource { get; set; }

            public override int GroupCount => DataSource.Count;

            public override QView CreateChildView(int groupPosition, int childPosition, IViewHierarchyBuilder builder)
            {
                return DataSource[groupPosition][childPosition];
            }

            public override QView CreateGroupView(int groupPosition, IViewHierarchyBuilder builder)
            {
                var header = builder.MakeView<TextViewWithIcon>();
                Theme.FormHeader(header);
#if __ANDROID__
                header.LayoutParameters.Width = LayoutParams.MatchParent;
#endif
                return new QView(header);
            }

            public override void FillChildView(int groupPosition, int childPosition, QView item)
            {
            }

            public override void FillGroupView(int groupPosition, QView section)
            {
                var view = section.PlatformView as TextViewWithIcon;
                switch (groupPosition)
                {
                    case 0:
                        view.Title.SetText("PERSONAL INFORMATION");
                        view.Icon.SetText(Icons.PersonalInfo);
                        break;
                    case 1:
                        view.Title.SetText("CONTACT INFORMATION");
                        view.Icon.SetText(Icons.ContactInfo);
                        break;
                    case 2:
                        view.Title.SetText("");
                        view.Icon.SetText("");
                        break;
                    case 3:
                        view.Title.SetText("");
                        view.Icon.SetText("");
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
