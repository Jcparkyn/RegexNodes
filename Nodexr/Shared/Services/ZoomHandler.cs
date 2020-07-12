﻿using Microsoft.JSInterop;

namespace Nodexr.Shared.Services
{
    public class ZoomHandler
    {
        public static double Zoom { get; set; } = 1d;

        public static string ZoomCSS { get => Zoom.ToString(); }

        public static void ZoomBy(double factor)
        {
            Zoom *= factor;
        }

        [JSInvokable]
        public static void SetZoom(float zoom)
        {
            Zoom = zoom;
        }
    }
}
