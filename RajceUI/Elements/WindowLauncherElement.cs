namespace RajceUI.Elements
{
    public class WindowLauncherElement : ToggleElement
    {
        public readonly WindowElement window;

        public WindowLauncherElement(LabelElement label, WindowElement window) :
            base(label, Binder.Binder.Create(() => window.IsOpen, v => window.IsOpen = v))
        {
            this.window = window;
            this.window.IsOpen = false;
        }
    }
}
