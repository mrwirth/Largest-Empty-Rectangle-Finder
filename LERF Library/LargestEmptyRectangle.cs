using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LERF_Library
{
    public static class LargestEmptyRectangle
    {
        public static (ICollection<Rectangle> LERs, ICollection<Rectangle> Extents) Find(Rectangle baseRectangle, IEnumerable<Rectangle> overlaidRectangles)
        {
            var baseRectangleSet = SubtractOverlays(baseRectangle, overlaidRectangles);
            var largestEmptyExtents = FindLargestEmptyRectangles(baseRectangleSet);
            var largestEmptyRectangles = largestEmptyExtents.Select(e => e.Rectangle);
            return (largestEmptyRectangles.ToList(), baseRectangleSet.Select(e => e.Rectangle).ToList());
        }

        private static IEnumerable<Extent> SubtractOverlays(Rectangle baseRectangle, IEnumerable<Rectangle> overlaidRectangles)
        {
            IEnumerable<Extent> baseExtentSet = new HashSet<Extent>() { new Extent(baseRectangle) };
            foreach (var rectangle in overlaidRectangles)
            {
                baseExtentSet = SplitOnRectangleEdges(baseExtentSet, rectangle)
                    .Remove(rectangle);
            }

            return baseExtentSet;
        }

        private static IEnumerable<Extent> Remove(this IEnumerable<Extent> extents, Rectangle rectangle)
        {
            extents = extents.ToHashSet();
            var extentsToRemove = extents
                    .Where(e => e.Left >= rectangle.Left && e.Right <= rectangle.Right && e.Top >= rectangle.Top && e.Bottom <= rectangle.Bottom)
                    .ToHashSet();
            foreach (var extent in extentsToRemove)
            {
                if (extent.LeftExtent != null)
                {
                    extent.LeftExtent.RightExtent = null;
                }
                if (extent.RightExtent != null)
                {
                    extent.RightExtent.LeftExtent = null;
                }
                if (extent.TopExtent != null)
                {
                    extent.TopExtent.BottomExtent = null;
                }
                if (extent.BottomExtent != null)
                {
                    extent.BottomExtent.TopExtent = null;
                }
            }
            return extents.Except(extentsToRemove);
        }

        private static IEnumerable<Extent> SplitOnRectangleEdges(IEnumerable<Extent> baseExtentSet, Rectangle overlaidRectangle)
        {
            baseExtentSet = SplitOnVerticalEdge(baseExtentSet, overlaidRectangle.Left);
            baseExtentSet = SplitOnVerticalEdge(baseExtentSet, overlaidRectangle.Right);
            baseExtentSet = SplitOnHorizontalEdge(baseExtentSet, overlaidRectangle.Top);
            baseExtentSet = SplitOnHorizontalEdge(baseExtentSet, overlaidRectangle.Bottom);

            return baseExtentSet;
        }

        private static IEnumerable<Extent> SplitOnVerticalEdge(IEnumerable<Extent> baseExtentSet, int axis)
        {
            var baseExtents = baseExtentSet.ToHashSet();
            var extentsToMerge = new HashSet<Extent>();
            var extentsToSplit = baseExtentSet
                .Where(e => e.Left < axis && e.Right > axis);
            foreach (var extent in extentsToSplit)
            {
                baseExtents.Remove(extent);
                var leftExtent = new Extent(extent.Left, extent.Top, axis - extent.Left, extent.Height);
                var rightExtent = new Extent(axis, extent.Top, extent.Right - axis, extent.Height);
                baseExtents.Add(leftExtent);
                baseExtents.Add(rightExtent);
            }

            return baseExtents;
        }

        private static IEnumerable<Extent> SplitOnHorizontalEdge(IEnumerable<Extent> baseExtentSet, int axis)
        {
            var baseExtents = baseExtentSet.ToHashSet();
            var extentsToMerge = new HashSet<Extent>();
            var extentsToSplit = baseExtentSet
                .Where(e => e.Top < axis && e.Bottom > axis);
            foreach (var extent in extentsToSplit)
            {
                baseExtents.Remove(extent);
                var topExtent = new Extent(extent.Left, extent.Top, extent.Width, axis - extent.Top);
                var bottomExtent = new Extent(extent.Left, axis, extent.Width, extent.Bottom - axis);
                baseExtents.Add(topExtent);
                baseExtents.Add(bottomExtent);
            }

            return baseExtents;
        }

        private static IEnumerable<Extent> FindLargestEmptyRectangles(IEnumerable<Extent> baseExtentSet)
        {
            baseExtentSet.ConnectExtents();
            var LERs = new ConcurrentBag<Extent>();
            Parallel.ForEach(baseExtentSet, (extent) =>
            {
                var extentLERs = GetMaxExtents(extent);
                foreach (var LER in extentLERs)
                {
                    LERs.Add(LER);
                }
            });
            var maxArea = LERs.Max(e => e.Area);
            return LERs.Where(e => e.Area == maxArea);
        }

        private static void ConnectExtents(this IEnumerable<Extent> extents)
        {
            foreach (var extent in extents)
            {
                extent.LeftExtent = extents
                    .Where(e => e.Right == extent.Left && e.Top == extent.Top)
                    .FirstOrDefault();
                extent.RightExtent = extents
                    .Where(e => e.Left == extent.Right && e.Top == extent.Top)
                    .FirstOrDefault();
                extent.TopExtent = extents
                    .Where(e => e.Bottom == extent.Top && e.Left == extent.Left)
                    .FirstOrDefault();
                extent.BottomExtent = extents
                    .Where(e => e.Top == extent.Bottom && e.Left == extent.Left)
                    .FirstOrDefault();
            }
        }

        private static HashSet<Extent> GetMaxExtents(Extent origin)
        {
            var maxExtents = new HashSet<Extent>() { origin };
            var lastColumn = origin;
            var height = 0;
            while (lastColumn != null)
            {
                var width = lastColumn.Right - origin.Left;
                var lastRow = lastColumn;
                while (lastRow != null)
                {
                    var columnHeight = lastRow.Bottom - origin.Top;
                    if (height == 0 || columnHeight < height)
                    {
                        if (lastRow.BottomExtent != null)
                        {
                            lastRow = lastRow.BottomExtent;
                            continue;
                        }
                        height = columnHeight;
                    }
                    if ((width * height) >= maxExtents.Max(e => e.Area))
                    {
                        var newExtent = new Extent(origin, width, height);
                        if(newExtent.Area > maxExtents.Max(e => e.Area))
                        {
                            maxExtents.Clear();
                        }
                        maxExtents.Add(newExtent);
                    }
                    break;
                }
                lastColumn = lastColumn.RightExtent;
            }

            return maxExtents;
        }
    }
}
