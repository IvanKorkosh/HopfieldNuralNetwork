using System;

namespace HopfieldNetworkCore
{
    public class ImageNotFoundException : Exception
    {
        public ImageNotFoundException() : base("Image not found")
        {
        }
    }
}
