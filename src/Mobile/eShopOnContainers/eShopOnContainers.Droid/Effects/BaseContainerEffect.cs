using Xamarin.Forms.Platform.Android;

namespace eShopOnContainers.Droid.Effects
{
    public abstract class BaseContainerEffect : PlatformEffect
    {
        private bool _unloaded;
        private bool _attached;

        protected bool Attached
        {
            get { return _attached; }
        }

        protected virtual bool CanBeApplied()
        {
            return true;
        }

        protected virtual void OnAttachedInternal()
        {
        }

        protected virtual void OnDetachedInternal()
        {
        }

        protected sealed override void OnAttached()
        {
            if (CanBeApplied())
            {
                _attached = true;

                Container.ViewDetachedFromWindow -= ContainerViewDetachedFromWindow;
                Container.ViewDetachedFromWindow += ContainerViewDetachedFromWindow;
                Container.ViewAttachedToWindow -= ContainerViewAttachedToWindow;
                Container.ViewAttachedToWindow += ContainerViewAttachedToWindow;

                OnAttachedInternal();
            }
        }

        protected sealed override void OnDetached()
        {
            if (_attached && !_unloaded)
            {
                _attached = false;

                Container.ViewDetachedFromWindow -= ContainerViewDetachedFromWindow;
                Container.ViewAttachedToWindow -= ContainerViewAttachedToWindow;

                OnDetachedInternal();
            }
        }

        private void ContainerViewDetachedFromWindow(object sender, global::Android.Views.View.ViewDetachedFromWindowEventArgs e)
        {
            _unloaded = true;
        }

        private void ContainerViewAttachedToWindow(object sender, global::Android.Views.View.ViewAttachedToWindowEventArgs e)
        {
            _unloaded = false;
        }
    }
}
