using eShopOnContainers.Core.Animations.Base;
using System;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Extensions
{
    public static class AnimationExtension
    {
        public static async void Animate(this VisualElement visualElement, AnimationBase animation, Action onFinishedCallback = null)
        {
            animation.Target = visualElement;

            await animation.Begin();

            onFinishedCallback?.Invoke();
        }
    }
}