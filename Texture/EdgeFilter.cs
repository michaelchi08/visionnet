﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisionNET.Texture
{
    /// <summary>
    /// A filter based on the first derivative of a Gaussian.
    /// </summary>
    [Serializable]
    public class EdgeFilter : Filter
    {
        private float _stddev, _orientation;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stddev">The standard deviation of the mean</param>
        /// <param name="orientation">Direction of the filter</param>
        /// <param name="channel">Channel to apply filter to</param>
        public EdgeFilter(float stddev, float orientation, int channel)
            : base(ComputeFilter(stddev, orientation), channel)
        {
            _stddev = stddev;
            _orientation = orientation;
        }

        /// <summary>
        /// Computes a 2-dimensional convolution filter for the provided sigma.
        /// </summary>
        /// <param name="stddev">Standard deviation of the Gaussian second derivative</param>
        /// <param name="orientation">Direction of the filter</param>
        /// <returns>Convolution filter</returns>
        public static float[,] ComputeFilter(float stddev, float orientation)
        {
            int halfsize = (int)Math.Ceiling(stddev * 7);
            int size = halfsize * 2 + 1;
            float center = size * .5f;
            Gaussian gauss = new Gaussian(0, stddev * 3);
            GaussianFirstDerivative fd = new GaussianFirstDerivative(0, stddev);
            float[,] filter = new float[size, size];
            float cos = (float)Math.Cos(orientation);
            float sin = (float)Math.Sin(orientation);
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    float rTmp = r + .5f - center;
                    float cTmp = c + .5f - center;
                    float rr = sin * cTmp + cos * rTmp;
                    float cc = cos * cTmp - sin * rTmp;
                    float val1 = gauss.Compute(cc);
                    float val2 = fd.Compute(rr);
                    float value = val1 * val2;
                    filter[r, c] = value;
                }
            }

            return filter;
        }

        /// <summary>
        /// Generates a string that describes the filter.
        /// </summary>
        /// <returns>A useful description</returns>
        public override string ToString()
        {
            return string.Format("{0} edge s={1:f4} o={2:f4}", base.ToString(), _stddev, _orientation);
        }
    }
}
