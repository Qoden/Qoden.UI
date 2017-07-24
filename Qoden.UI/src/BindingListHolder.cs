using System;
using Qoden.Binding;
using Qoden.Validation;

namespace Qoden.UI
{
    public struct BindingListHolder
    {
        BindingList bindings;
        public BindingList Value
        {
            get
            {
                if (bindings == null)
                {
                    bindings = new BindingList();
                }
                return bindings;
            }
            set
            {
                Assert.Argument(value, "value").NotNull();
                var rebind = bindings != null && bindings.Bound;
                if (rebind)
                {
                    bindings.Unbind();
                }
                bindings = value;
                if (rebind)
                {
                    bindings.Bind();
                }
            }
        }
    }
}
