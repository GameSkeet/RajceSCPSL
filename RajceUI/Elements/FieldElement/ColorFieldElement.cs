using RajceUI.Binder;

using UnityEngine;

namespace RajceUI.Elements
{
    public class ColorFieldElement : FieldBaseElement<Color>
    {
        public ColorFieldElement(LabelElement label, IBinder<Color> binder) : base(label, binder) { }
    }
}
