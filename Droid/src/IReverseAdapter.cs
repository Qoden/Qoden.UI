using System;
namespace Qoden.UI
{
    public interface IReverseAdapter<T>
    {
        int GetPosition(T item);
    }
}
