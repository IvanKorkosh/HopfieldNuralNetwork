using System;
using System.Collections.Generic;
using System.Linq;

namespace HopfieldNetworkCore
{
    public class PatternRecognizer
    {
        private readonly int imageSize;
        private readonly List<BinaryState[]> images = new List<BinaryState[]>();
        private readonly double[] weights;

        public PatternRecognizer(int imageSize)
        {
            if (imageSize <= 0)
            {
                throw new ArgumentException("Less zero", nameof(imageSize));
            }

            this.imageSize = imageSize;
            weights = new double[imageSize * imageSize];
        }

        public int Count => images.Count;

        public BinaryState[] GetImage(int index)
        {
            if (index < 0 || index > imageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return images[index];
        }

        public void EraseAllImages()
        {
            ResetWeights();
            images.Clear();
        }

        public void LoadNewImage(IEnumerable<BinaryState> image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (image.Count() != imageSize)
            {
                throw new ArgumentException("Invalid length", nameof(image));
            }

            images.Add(image.ToArray());
            CalculateWeights();
        }

        public List<BinaryState> FindImage(IEnumerable<BinaryState> image, int attemptsNumber)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (image.Count() != imageSize)
            {
                throw new ArgumentException("Invalid length", nameof(image));
            }

            if (attemptsNumber < 1)
            {
                throw new ArgumentException("Less zero", nameof(image));
            }

            var source = image.ToList();
            for (int attempt = 0; attempt < attemptsNumber; ++attempt)
            {
                var result = ImageCorrection(source);
                if (Contains(result))
                {
                    return result;
                }

                source = result;
            }

            throw new ImageNotFoundException();
        }

        private void CalculateWeights()
        {
            for (int y = 0; y < imageSize; ++y)
            {
                for (int x = 0; x < imageSize; ++x)
                {
                    if (y == x)
                    {
                        continue;
                    }

                    foreach (var image in images)
                    {
                        var parsed = Parse(image);
                        weights[y * imageSize + x] += parsed[x] * parsed[y];
                    }

                    weights[y * imageSize + x] /= imageSize;
                }
            }
        }

        private void ResetWeights()
        {
            for (int y = 0; y < imageSize; ++y)
            {
                for (int x = 0; x < imageSize; ++x)
                {
                    weights[y * imageSize + x] = 0;
                }
            }
        }

        private List<BinaryState> ImageCorrection(IEnumerable<BinaryState> image)
        {
            var source = Parse(image);
            var result = new BinaryState[imageSize];
            for (int y = 0; y < imageSize; ++y)
            {
                double imageСomponent = 0;
                for (int x = 0; x < imageSize; ++x)
                {
                    imageСomponent += source[x] * weights[y * imageSize + x];
                }

                result[y] = Sign(imageСomponent);
            }

            return result.ToList();
        }

        private bool Contains(IEnumerable<BinaryState> source)
            => images.Any(image => Compare(source, image));

        private bool Compare(IEnumerable<BinaryState> left, IEnumerable<BinaryState> right)
        {
            var leftArray = left.ToArray();
            var rightArray = right.ToArray();
            for (int i = 0; i < imageSize; ++i)
            {
                if (leftArray[i] != rightArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        private BinaryState Sign(double x)
            => x < 0 ? BinaryState.Low : BinaryState.High;

        private List<double> Parse(IEnumerable<BinaryState> source)
            => source.Select(s => s == BinaryState.High ? 1.0 : -1.0).ToList();
    }
}
