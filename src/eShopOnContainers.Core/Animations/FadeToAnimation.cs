using eShopOnContainers.Core.Animations.Base;
using eShopOnContainers.Core.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Animations
{
    public class FadeToAnimation : AnimationBase
    {
        public static readonly BindableProperty OpacityProperty =
         BindableProperty.Create("Opacity", typeof(double), typeof(FadeToAnimation), 0.0d,
         propertyChanged: (bindable, oldValue, newValue) =>
         ((FadeToAnimation)bindable).Opacity = (double)newValue);

        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Target.FadeTo(Opacity, Convert.ToUInt32(Duration), EasingHelper.GetEasing(Easing));
        }

        protected override Task ResetAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Target.FadeTo(0, 0, null);
        }
    }

    public class FadeInAnimation : AnimationBase
    {
        public enum FadeDirection
        {
            Up,
            Down
        }

        public static readonly BindableProperty DirectionProperty =
         BindableProperty.Create("Direction", typeof(FadeDirection), typeof(FadeInAnimation), FadeDirection.Up,
         propertyChanged: (bindable, oldValue, newValue) =>
         ((FadeInAnimation)bindable).Direction = (FadeDirection)newValue);

        public FadeDirection Direction
        {
            get { return (FadeDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Target.Animate("FadeIn", FadeIn(), 16, Convert.ToUInt32(Duration));
                });
            });
        }

        protected override Task ResetAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Target.FadeTo(0, 0, null);
        }

        internal Animation FadeIn()
        {
            var animation = new Animation();

            animation.WithConcurrent((f) => Target.Opacity = f, 0, 1, Xamarin.Forms.Easing.CubicOut);

            animation.WithConcurrent(
              (f) => Target.TranslationY = f,
              Target.TranslationY + ((Direction == FadeDirection.Up) ? 50 : -50), Target.TranslationY,
              Xamarin.Forms.Easing.CubicOut, 0, 1);

            return animation;
        }
    }

    public class FadeOutAnimation : AnimationBase
    {
        public enum FadeDirection
        {
            Up,
            Down
        }

        public static readonly BindableProperty DirectionProperty =
         BindableProperty.Create("Direction", typeof(FadeDirection), typeof(FadeOutAnimation), FadeDirection.Up,
         propertyChanged: (bindable, oldValue, newValue) =>
         ((FadeOutAnimation)bindable).Direction = (FadeDirection)newValue);

        public FadeDirection Direction
        {
            get { return (FadeDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Target.Animate("FadeOut", FadeOut(), 16, Convert.ToUInt32(Duration));
                });
            });
        }

        protected override Task ResetAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Target.FadeTo(0, 0, null);
        }

        internal Animation FadeOut()
        {
            var animation = new Animation();

            animation.WithConcurrent(
                 (f) => Target.Opacity = f,
                 1, 0);

            animation.WithConcurrent(
                  (f) => Target.TranslationY = f,
                  Target.TranslationY, Target.TranslationY + ((Direction == FadeDirection.Up) ? 50 : -50));

            return animation;
        }
    }
}
