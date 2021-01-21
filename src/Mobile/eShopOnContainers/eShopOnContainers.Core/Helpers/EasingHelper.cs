using eShopOnContainers.Core.Animations.Base;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Helpers
{
    public static class EasingHelper
    {
        public static Easing GetEasing(EasingType type)
        {
            switch (type)
            {
                case EasingType.BounceIn:
                    return Easing.BounceIn;
                case EasingType.BounceOut:
                    return Easing.BounceOut;
                case EasingType.CubicIn:
                    return Easing.CubicIn;
                case EasingType.CubicInOut:
                    return Easing.CubicInOut;
                case EasingType.CubicOut:
                    return Easing.CubicOut;
                case EasingType.Linear:
                    return Easing.Linear;
                case EasingType.SinIn:
                    return Easing.SinIn;
                case EasingType.SinInOut:
                    return Easing.SinInOut;
                case EasingType.SinOut:
                    return Easing.SinOut;
                case EasingType.SpringIn:
                    return Easing.SpringIn;
                case EasingType.SpringOut:
                    return Easing.SpringOut;
            }

            return null;
        }
    }
}
