using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;

namespace eShopOnContainers.Droid.Renderers
{
    public class BadgeView : TextView
    {
        public enum BadgePosition
        {
            PositionTopLeft = 1,
            PositionTopRight = 2,
            PositionBottomLeft = 3,
            PositionBottomRight = 4,
            PositionCenter = 5
        }

        private const int DefaultHmarginDip = -10;
        private const int DefaultVmarginDip = 2;
        private const int DefaultLrPaddingDip = 4;
        private const int DefaultCornerRadiusDip = 7;

        private static Animation _fadeInAnimation;
        private static Animation _fadeOutAnimation;

        private Context _context;
        private readonly Color _defaultBadgeColor = Color.ParseColor("#CCFF0000");
        private ShapeDrawable _backgroundShape;

        public View Target { get; private set; }
        public BadgePosition Postion { get; set; } = BadgePosition.PositionTopRight;
        public int BadgeMarginH { get; set; }
        public int BadgeMarginV { get; set; }

        public static int TextSizeDip { get; set; } = 11;

        public Color BadgeColor
        {
            get { return _backgroundShape.Paint.Color; }
            set
            {
                _backgroundShape.Paint.Color = value;

                Background.InvalidateSelf();
            }
        }

        public Color TextColor
        {
            get { return new Color(CurrentTextColor); }
            set { SetTextColor(value); }
        }

        public BadgeView(Context context, View target) : this(context, null, Android.Resource.Attribute.TextViewStyle, target)
        {
        }

        public BadgeView(Context context, IAttributeSet attrs, int defStyle, View target) : base(context, attrs, defStyle)
        {
            Init(context, target);
        }

        private void Init(Context context, View target)
        {
            _context = context;
            Target = target;

            BadgeMarginH = DipToPixels(DefaultHmarginDip);
            BadgeMarginV = DipToPixels(DefaultVmarginDip);

            Typeface = Typeface.DefaultBold;
            var paddingPixels = DipToPixels(DefaultLrPaddingDip);
            SetPadding(paddingPixels, 0, paddingPixels, 0);
            SetTextColor(Color.White);
            SetTextSize(ComplexUnitType.Dip, TextSizeDip);

            _fadeInAnimation = new AlphaAnimation(0, 1)
            {
                Interpolator = new DecelerateInterpolator(),
                Duration = 200
            };

            _fadeOutAnimation = new AlphaAnimation(1, 0)
            {
                Interpolator = new AccelerateInterpolator(),
                Duration = 200
            };

            _backgroundShape = CreateBackgroundShape();
            Background = _backgroundShape;
            BadgeColor = _defaultBadgeColor;

            if (Target != null)
            {
                ApplyTo(Target);
            }
            else
            {
                Show();
            }
        }

        private ShapeDrawable CreateBackgroundShape()
        {
            var radius = DipToPixels(DefaultCornerRadiusDip);
            var outerR = new float[] { radius, radius, radius, radius, radius, radius, radius, radius };

            return new ShapeDrawable(new RoundRectShape(outerR, null, null));
        }

        private void ApplyTo(View target)
        {
            var lp = target.LayoutParameters;
            var parent = target.Parent;

            var group = parent as ViewGroup;
            if (group == null)
            {
                Console.WriteLine("Badge target parent has to be a view group");
                return;
            }

            group.SetClipChildren(false);
            group.SetClipToPadding(false);


            var container = new FrameLayout(_context);
            var index = group.IndexOfChild(target);

            group.RemoveView(target);
            group.AddView(container, index, lp);

            container.AddView(target);
            group.Invalidate();

            Visibility = ViewStates.Gone;
            container.AddView(this);

        }

        public void Show()
        {
            Show(false, null);
        }

        public void Show(bool animate)
        {
            Show(animate, _fadeInAnimation);
        }


        public void Hide(bool animate)
        {
            Hide(animate, _fadeOutAnimation);
        }

        private void Show(bool animate, Animation anim)
        {
            ApplyLayoutParams();

            if (animate)
            {
                StartAnimation(anim);
            }

            Visibility = ViewStates.Visible;

        }

        private void Hide(bool animate, Animation anim)
        {
            Visibility = ViewStates.Gone;
            if (animate)
            {
                StartAnimation(anim);
            }
        }

        private void ApplyLayoutParams()
        {
            var layoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            switch (Postion)
            {
                case BadgePosition.PositionTopLeft:
                    layoutParameters.Gravity = GravityFlags.Left | GravityFlags.Top;
                    layoutParameters.SetMargins(BadgeMarginH, BadgeMarginV, 0, 0);
                    break;
                case BadgePosition.PositionTopRight:
                    layoutParameters.Gravity = GravityFlags.Right | GravityFlags.Top;
                    layoutParameters.SetMargins(0, BadgeMarginV, BadgeMarginH, 0);
                    break;
                case BadgePosition.PositionBottomLeft:
                    layoutParameters.Gravity = GravityFlags.Left | GravityFlags.Bottom;
                    layoutParameters.SetMargins(BadgeMarginH, 0, 0, BadgeMarginV);
                    break;
                case BadgePosition.PositionBottomRight:
                    layoutParameters.Gravity = GravityFlags.Right | GravityFlags.Bottom;
                    layoutParameters.SetMargins(0, 0, BadgeMarginH, BadgeMarginV);
                    break;
                case BadgePosition.PositionCenter:
                    layoutParameters.Gravity = GravityFlags.Center;
                    layoutParameters.SetMargins(0, 0, 0, 0);
                    break;
            }

            LayoutParameters = layoutParameters;

        }

        private int DipToPixels(int dip)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, Resources.DisplayMetrics);
        }

        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;

                if (Visibility == ViewStates.Visible && string.IsNullOrEmpty(value))
                {
                    Hide(true);
                }
                else if (Visibility == ViewStates.Gone && !string.IsNullOrEmpty(value))
                {
                    Show(true);
                }
            }
        }
    }
}