using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace eShopOnContainers.Windows.Extensions
{
    public static class VisualTreeExtensions
    {
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null!");
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }

        internal static IEnumerable<FrameworkElement> GetLogicalChildrenBreathFirst(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null!");
            var queue =
                new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());
            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                yield return element;
                foreach (var visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }
        }

        internal static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            var parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        internal static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return VisualTreeHelper.GetParent(node) as FrameworkElement;
        }

        internal static T GetParentByType<T>(this DependencyObject element)
            where T : FrameworkElement
        {
            Debug.Assert(element != null, "The element cannot be null!");
            var parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                var result = parent as T;
                if (result != null)
                {
                    return result;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static T GetFirstDescendantOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendantsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendants().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        {
            if (start == null)
            {
                yield break;
            }

            var queue = new Queue<DependencyObject>();

            var popup = start as Popup;

            if (popup != null)
            {
                if (popup.Child != null)
                {
                    queue.Enqueue(popup.Child);
                    yield return popup.Child;
                }
            }
            else
            {
                var count = VisualTreeHelper.GetChildrenCount(start);

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(start, i);
                    queue.Enqueue(child);
                    yield return child;
                }
            }

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();

                popup = parent as Popup;

                if (popup != null)
                {
                    if (popup.Child != null)
                    {
                        queue.Enqueue(popup.Child);
                        yield return popup.Child;
                    }
                }
                else
                {
                    var count = VisualTreeHelper.GetChildrenCount(parent);

                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        yield return child;
                        queue.Enqueue(child);
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject parent)
        {
            var popup = parent as Popup;

            if (popup?.Child != null)
            {
                yield return popup.Child;
                yield break;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                yield return child;
            }
        }

        public static IEnumerable<DependencyObject> GetChildrenByZIndex(
            this DependencyObject parent)
        {
            int i = 0;
            var indexedChildren =
                parent.GetChildren().Cast<FrameworkElement>().Select(
                child => new { Index = i++, ZIndex = Canvas.GetZIndex(child), Child = child });

            return
                from indexedChild in indexedChildren
                orderby indexedChild.ZIndex, indexedChild.Index
                select indexedChild.Child;
        }

        public static T GetFirstAncestorOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestorsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetAncestorsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestors().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject start)
        {
            var parent = VisualTreeHelper.GetParent(start);

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable<DependencyObject> GetSiblings(this DependencyObject start)
        {
            var parent = VisualTreeHelper.GetParent(start);

            if (parent == null)
            {
                yield return start;
            }
            else
            {
                var count = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    yield return child;
                }
            }
        }

        public static bool IsInVisualTree(this DependencyObject dob)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return false;
            }

            //TODO: consider making it work with Popups too.
            if (Window.Current == null)
            {
                // This may happen when a picker or CameraCaptureUI etc. is open.
                return false;
            }

            return Window.Current.Content != null && dob.GetAncestors().Contains(Window.Current.Content);
        }

        public static Point GetPosition(this FrameworkElement dob, Point origin = new Point(), FrameworkElement relativeTo = null)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return new Point();
            }

            if (relativeTo == null)
            {
                relativeTo = Window.Current.Content as FrameworkElement;
            }

            if (relativeTo == null)
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            var absoluteOrigin = new Point(relativeTo.ActualWidth * origin.X, relativeTo.ActualHeight * origin.X);

            if (dob == relativeTo)
            {
                return absoluteOrigin;
            }

            var ancestors = dob.GetAncestors().ToArray();

            if (!ancestors.Contains(relativeTo))
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            return
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(absoluteOrigin);
        }

        public static Rect GetBoundingRect(this FrameworkElement dob, FrameworkElement relativeTo = null)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return Rect.Empty;
            }

            if (relativeTo == null)
            {
                relativeTo = Window.Current.Content as FrameworkElement;
            }

            if (relativeTo == null)
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            if (dob == relativeTo)
            {
                return new Rect(0, 0, relativeTo.ActualWidth, relativeTo.ActualHeight);
            }

            var ancestors = dob.GetAncestors().ToArray();

            if (!ancestors.Contains(relativeTo))
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            var topLeft =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(new Point());
            var topRight =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            dob.ActualWidth,
                            0));
            var bottomLeft =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            0,
                            dob.ActualHeight));
            var bottomRight =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            dob.ActualWidth,
                            dob.ActualHeight));

            var minX = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Min();
            var maxX = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Max();
            var minY = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Min();
            var maxY = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Max();

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private static void ExploreTree(List<DependencyObject> list, DependencyObject obj)
        {
            if (list != null && obj != null)
            {
                int childrens = VisualTreeHelper.GetChildrenCount(obj);
                for (int x = 0; x < childrens; x++)
                {
                    DependencyObject tmp = VisualTreeHelper.GetChild(obj, x);
                    list.Add(tmp);
                    ExploreTree(list, tmp);
                }
            }
        }

        public static List<DependencyObject> GetVisualTree(DependencyObject obj)
        {
            var tmp = new List<DependencyObject>();
            ExploreTree(tmp, obj);
            return tmp;
        }

        public static T FindNameInVisualTree<T>(DependencyObject root, string name)
        {
            T res = default(T);
            List<DependencyObject> tree = null;
            tree = GetVisualTree(root);
            for (int x = 0; x < tree.Count; x++)
            {
                if (tree[x] is FrameworkElement)
                {
                    if (((FrameworkElement)tree[x]).Name == name)
                    {
                        res = (T)((object)tree[x]);
                        break;
                    }
                }
            }

            return res;
        }
    }
}