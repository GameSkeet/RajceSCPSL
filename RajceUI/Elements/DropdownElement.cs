using RajceUI.Binder;

using System.Linq;
using System.Collections.Generic;

namespace RajceUI.Elements
{
    public class DropdownElement : FieldBaseElement<int>
    {
        public readonly List<string> options;

        public DropdownElement(LabelElement label, IBinder<int> binder, IEnumerable<string> options) : base(label, binder)
        {
            this.options = options.ToList();
        }
    }
}
