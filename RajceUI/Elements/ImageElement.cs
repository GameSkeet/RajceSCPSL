using RajceUI.Binder;

using UnityEngine;

namespace RajceUI.Elements
{
    public class ImageElement : ReadOnlyValueElement<Texture>
    {
        public ImageElement(IGetter<Texture> getter) : base(getter)
        {
        }
    }
}
