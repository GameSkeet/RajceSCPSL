using System.Collections.Generic;

namespace RajceUI.Binder
{
    public interface IBinderSet<T>
    {
        IEnumerable<IBinder<T>> Binders { get; }
    }
}
