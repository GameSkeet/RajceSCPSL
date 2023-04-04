namespace RajceInternal.UI
{
    internal abstract class UIElement<TRes>
    {
        public delegate void OnInteractionDelegate(out TRes res);
        public OnInteractionDelegate onInteraction;

        public abstract void OnDraw();
    }
}
