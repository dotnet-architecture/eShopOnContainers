using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Animations.Base
{
    public abstract class AnimationBase : BindableObject
    {
        private bool _isRunning = false;

        public static readonly BindableProperty TargetProperty = BindableProperty.Create("Target", typeof(VisualElement), typeof(AnimationBase), null,
            propertyChanged: (bindable, oldValue, newValue) => ((AnimationBase)bindable).Target = (VisualElement)newValue);

        public VisualElement Target
        {
            get { return (VisualElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly BindableProperty DurationProperty = BindableProperty.Create("Duration", typeof(string), typeof(AnimationBase), "1000",
            propertyChanged: (bindable, oldValue, newValue) => ((AnimationBase)bindable).Duration = (string)newValue);

        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly BindableProperty EasingProperty = BindableProperty.Create("Easing", typeof(EasingType), typeof(AnimationBase), EasingType.Linear,
            propertyChanged: (bindable, oldValue, newValue) => ((AnimationBase)bindable).Easing = (EasingType)newValue);

        public EasingType Easing
        {
            get { return (EasingType)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        public static readonly BindableProperty RepeatForeverProperty = BindableProperty.Create("RepeatForever", typeof(bool), typeof(AnimationBase), false,
            propertyChanged: (bindable, oldValue, newValue) => ((AnimationBase)bindable).RepeatForever = (bool)newValue);

        public bool RepeatForever
        {
            get { return (bool)GetValue(RepeatForeverProperty); }
            set { SetValue(RepeatForeverProperty, value); }
        }

        public static readonly BindableProperty DelayProperty = BindableProperty.Create("Delay", typeof(int), typeof(AnimationBase), 0,
            propertyChanged: (bindable, oldValue, newValue) => ((AnimationBase)bindable).Delay = (int)newValue);

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        protected abstract Task BeginAnimation();

        public async Task Begin()
        {
            try
            {
                if (!_isRunning)
                {
                    _isRunning = true;

                    await InternalBegin()
                        .ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted)
                        .ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in animation {ex}");
            }
        }

        protected abstract Task ResetAnimation();

        public async Task Reset()
        {
            _isRunning = false;
            await ResetAnimation();
        }

        private async Task InternalBegin()
        {
            if (Delay > 0)
            {
                await Task.Delay(Delay);
            }

            if (!RepeatForever)
            {
                await BeginAnimation();
            }
            else
            {
                do
                {
                    await BeginAnimation();
                    await ResetAnimation();
                } while (RepeatForever);
            }
        }
    }
}
