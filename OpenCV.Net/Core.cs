﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net.Native;
using System.Runtime.InteropServices;

namespace OpenCV.Net
{
    /// <summary>
    /// This class provides wrapper methods for the OpenCV C interface native functions.
    /// </summary>
    public static partial class cv
    {
        #region Array allocation, deallocation, initialization and access to elements

        /// <summary>
        /// Allocates a block of <paramref name="size"/> bytes in memory,
        /// returning a pointer to the beginning of the block.
        /// </summary>
        /// <param name="size">Size of the memory block, in bytes.</param>
        /// <returns>
        /// On success, a pointer to the memory block allocated by the function.
        /// If there is not enough memory, the function raises an error.
        /// </returns>
        public static IntPtr Alloc(UIntPtr size)
        {
            return NativeMethods.cvAlloc(size);
        }

        /// <summary>
        /// Deallocates a block of memory previously allocated by a call to
        /// <see cref="Alloc"/>.
        /// </summary>
        /// <param name="ptr">Pointer to a memory block previously allocated with <see cref="Alloc"/>.</param>
        public static void Free(ref IntPtr ptr)
        {
            NativeMethods.cvFree_(ptr);
            ptr = IntPtr.Zero;
        }

        /// <summary>
        /// Repeats the 2D source array in both horizontal and vertical directions
        /// to fill the destination array.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Repeat(CvArr src, CvArr dst)
        {
            NativeMethods.cvRepeat(src, dst);
        }

        /// <summary>
        /// Copies elements of one array to another.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void Copy(CvArr src, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvCopy(src, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Divides a multi-channel array into several single-channel arrays or extracts
        /// a single channel from the array.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst0">The first destination channel array.</param>
        /// <param name="dst1">The second destination channel array.</param>
        /// <param name="dst2">The third destination channel array.</param>
        /// <param name="dst3">The fourth destination channel array.</param>
        /// <remarks>
        /// The function divides a multi-channel array into separate single-channel arrays.
        /// Two modes are available for the operation. If the source array has N channels
        /// and the first N destination channels are not <b>null</b>, they all are
        /// extracted from the source array; if only a single destination channel of
        /// the first N is not <b>null</b>, this particular channel is extracted;
        /// otherwise an exception is raised. The rest of the destination channels
        /// (beyond the first N) must always be <b>null</b>. For an <see cref="IplImage"/>
        /// instance, <see cref="Copy"/> with COI set can also be used to extract a
        /// single channel from the image.
        /// </remarks>
        public static void Split(CvArr src, CvArr dst0, CvArr dst1, CvArr dst2, CvArr dst3)
        {
            NativeMethods.cvSplit(src, dst0 ?? CvArr.Null, dst1 ?? CvArr.Null, dst2 ?? CvArr.Null, dst3 ?? CvArr.Null);
        }

        /// <summary>
        /// Composes a multi-channel array from several single-channel arrays or
        /// inserts a single channel into the array.
        /// </summary>
        /// <param name="src0">The first input channel array.</param>
        /// <param name="src1">The second input channel array.</param>
        /// <param name="src2">The third input channel array.</param>
        /// <param name="src3">The fourth input channel array.</param>
        /// <param name="dst">The destination array.</param>
        /// <remarks>
        /// The function is the opposite to <see cref="Split"/>. If the destination
        /// array has N channels then if the first N input channels are not <b>null</b>,
        /// they all are copied to the destination array; if only a single source
        /// channel of the first N is not <b>null</b>, this particular channel is
        /// copied into the destination array; otherwise an exception is raised.
        /// The rest of the source channels (beyond the first N) must always be <b>null</b>.
        /// For an <see cref="IplImage"/> instance, <see cref="Copy"/> with COI set
        /// can also be used to insert a single channel into the image.
        /// </remarks>
        public static void Merge(CvArr src0, CvArr src1, CvArr src2, CvArr src3, CvArr dst)
        {
            NativeMethods.cvMerge(src0 ?? CvArr.Null, src1 ?? CvArr.Null, src2 ?? CvArr.Null, src3 ?? CvArr.Null, dst);
        }

        /// <summary>
        /// Copies specified channels from the input arrays to the specified channels
        /// of the output arrays. All matrices must have the same size and depth.
        /// </summary>
        /// <param name="src">The input arrays.</param>
        /// <param name="dst">The output arrays.</param>
        /// <param name="fromTo">
        /// The array of index pairs, specifying which channels are copied and where.
        /// <paramref name="fromTo"/>[k*2] is the 0-based index of the input channel
        /// in <paramref name="src"/> and <paramref name="fromTo"/>[k*2+1] is the
        /// index of the output channel in <paramref name="dst"/>. Continuous channel
        /// numbering is used, i.e. the first input channels in <paramref name="src"/>
        /// are indexed from <code>0</code> to <code>src[0].Channels-1</code>, and the
        /// second input channels are indexed from <code>src[0].Channels</code> to
        /// <code>src[0].Channels + src[1].Channels-1</code>. The same scheme is used
        /// for the output image channels. As a special case, negative output channel
        /// indices mean that the corresponding output channel is filled with zeros.
        /// </param>
        /// <remarks>
        /// This function is a generalized form of <see cref="Split"/> and
        /// <see cref="Merge"/>. It can be used to change the order of the planes,
        /// add/remove alpha channel, extract or insert a single plane or multiple
        /// planes, etc.
        /// </remarks>
        public static void MixChannels(CvArr[] src, CvArr[] dst, int[] fromTo)
        {
            var srcHandles = Array.ConvertAll(src, arr => arr.DangerousGetHandle());
            var dstHandles = Array.ConvertAll(dst, arr => arr.DangerousGetHandle());
            NativeMethods.cvMixChannels(srcHandles, srcHandles.Length, dstHandles, dstHandles.Length, fromTo, fromTo.Length / 2);
            GC.KeepAlive(src);
            GC.KeepAlive(dst);
        }

        /// <summary>
        /// Converts one array to another with optional linear transformation.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="scale">
        /// The optional scale factor applied independently to all element channels.
        /// </param>
        /// <param name="shift">The optional value added to scaled array elements.</param>
        public static void ConvertScale(CvArr src, CvArr dst, double scale = 1, double shift = 0)
        {
            NativeMethods.cvConvertScale(src, dst, scale, shift);
        }

        /// <summary>
        /// Converts input array elements to an 8-bit unsigned integer array
        /// with optional linear transformation.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array (must have U8 depth).</param>
        /// <param name="scale">
        /// The optional scale factor applied independently to all element channels.
        /// </param>
        /// <param name="shift">The optional value added to scaled array elements.</param>
        /// <remarks>
        /// This function is similar to <see cref="ConvertScale"/>, but stores absolute
        /// values after the conversion results.
        /// </remarks>
        public static void ConvertScaleAbs(CvArr src, CvArr dst, double scale = 1, double shift = 0)
        {
            NativeMethods.cvConvertScaleAbs(src, dst, scale, shift);
        }

        #endregion

        #region Arithmetic, logic and comparison operations

        /// <summary>
        /// Calculates the per-element sum of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void Add(CvArr src1, CvArr src2, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvAdd(src1, src2, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates the per-element sum of an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void AddS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvAddS(src, value, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates the per-element difference between two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void Sub(CvArr src1, CvArr src2, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvSub(src1, src2, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates the per-element difference between an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void SubS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            AddS(src, new CvScalar(-value.Val0, -value.Val1, -value.Val2, -value.Val3), dst, mask);
        }

        /// <summary>
        /// Calculates the per-element difference between a scalar and an array.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void SubRS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvSubRS(src, value, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates the per-element product of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="scale">An optional scale factor.</param>
        public static void Mul(CvArr src1, CvArr src2, CvArr dst, double scale = 1)
        {
            NativeMethods.cvMul(src1, src2, dst, scale);
        }

        /// <summary>
        /// Calculates the per-element division of two arrays.
        /// </summary>
        /// <param name="src1">The first input array. If the reference is <b>null</b>, the array is assumed to be all ones.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="scale">An optional scale factor.</param>
        public static void Div(CvArr src1, CvArr src2, CvArr dst, double scale = 1)
        {
            NativeMethods.cvDiv(src1 ?? CvArr.Null, src2, dst, scale);
        }

        /// <summary>
        /// Calculates the sum of a scaled array and another array.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="scale">The scale factor for the first array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void ScaleAdd(CvArr src1, CvScalar scale, CvArr src2, CvArr dst)
        {
            NativeMethods.cvScaleAdd(src1, scale, src2, dst);
        }

        /// <summary>
        /// Calculates the sum of a multiple of an array with another array.
        /// </summary>
        /// <param name="A">The first input array.</param>
        /// <param name="real_scalar">The scale factor for the first array.</param>
        /// <param name="B">The second input array.</param>
        /// <param name="C">The destination array.</param>
        public static void AXPY(CvArr A, double real_scalar, CvArr B, CvArr C)
        {
            ScaleAdd(A, CvScalar.Real(real_scalar), B, C);
        }

        /// <summary>
        /// Computes the weighted sum of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="alpha">The weight for the first array elements.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="beta">The weight for the second array elements.</param>
        /// <param name="gamma">A scalar that is added to each sum.</param>
        /// <param name="dst">The destination array.</param>
        public static void AddWeighted(CvArr src1, double alpha, CvArr src2, double beta, double gamma, CvArr dst)
        {
            NativeMethods.cvAddWeighted(src1, alpha, src2, beta, gamma, dst);
        }

        /// <summary>
        /// Calculates the dot product of two arrays in Euclidian metrics.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <returns>
        /// The Euclidean dot product of the two arrays. In the case of multiple
        /// channel arrays, the results for all channels are accumulated.
        /// </returns>
        public static double DotProduct(CvArr src1, CvArr src2)
        {
            return NativeMethods.cvDotProduct(src1, src2);
        }

        /// <summary>
        /// Performs per-element bit-wise conjunction of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void And(CvArr src1, CvArr src2, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvAnd(src1, src2, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise conjunction of an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void AndS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvAndS(src, value, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise disjunction of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void Or(CvArr src1, CvArr src2, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvOr(src1, src2, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise disjunction of an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void OrS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvOrS(src, value, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise "exclusive or" operation on two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void Xor(CvArr src1, CvArr src2, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvXor(src1, src2, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise “exclusive or” operation on an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="mask">
        /// Optional operation mask, 8-bit single-channel array specifying the
        /// elements that should be changed on <paramref name="dst"/>.
        /// </param>
        public static void XorS(CvArr src, CvScalar value, CvArr dst, CvArr mask = null)
        {
            NativeMethods.cvXorS(src, value, dst, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Performs per-element bit-wise inversion of array elements.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Not(CvArr src, CvArr dst)
        {
            NativeMethods.cvNot(src, dst);
        }

        /// <summary>
        /// Checks that array elements lie between the elements of two other arrays.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="lower">The inclusive lower boundary array.</param>
        /// <param name="upper">The exclusive upper boundary array.</param>
        /// <param name="dst">The destination array. It must have U8 or S8 type.</param>
        public static void InRange(CvArr src, CvArr lower, CvArr upper, CvArr dst)
        {
            NativeMethods.cvInRange(src, lower, upper, dst);
        }

        /// <summary>
        /// Checks that array elements lie between two scalars.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="lower">The inclusive lower boundary.</param>
        /// <param name="upper">The exclusive upper boundary.</param>
        /// <param name="dst">The destination array. It must have U8 or S8 type.</param>
        public static void InRangeS(CvArr src, CvScalar lower, CvScalar upper, CvArr dst)
        {
            NativeMethods.cvInRangeS(src, lower, upper, dst);
        }

        /// <summary>
        /// Performs per-element comparison of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array. Both input arrays must have a single channel.</param>
        /// <param name="dst">The destination array. It must have U8 or S8 type.</param>
        /// <param name="cmp_op">
        /// The comparison operation used to test the relation between the elements to be checked.
        /// </param>
        public static void Cmp(CvArr src1, CvArr src2, CvArr dst, ComparisonOperation cmp_op)
        {
            NativeMethods.cvCmp(src2, src2, dst, cmp_op);
        }

        /// <summary>
        /// Performs per-element comparison of an array and a scalar.
        /// </summary>
        /// <param name="src">The input array, must have a single channel.</param>
        /// <param name="value">The scalar value with which to compare each array element.</param>
        /// <param name="dst">The destination array. It must have U8 or S8 type.</param>
        /// <param name="cmpOp">
        /// The comparison operation used to test the relation between the elements to be checked.
        /// </param>
        public static void CmpS(CvArr src, double value, CvArr dst, ComparisonOperation cmpOp)
        {
            NativeMethods.cvCmpS(src, value, dst, cmpOp);
        }

        /// <summary>
        /// Finds per-element minimum of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Min(CvArr src1, CvArr src2, CvArr dst)
        {
            NativeMethods.cvMin(src1, src2, dst);
        }

        /// <summary>
        /// Finds per-element maximum of two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Max(CvArr src1, CvArr src2, CvArr dst)
        {
            NativeMethods.cvMax(src1, src2, dst);
        }

        /// <summary>
        /// Finds per-element minimum of an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        public static void MinS(CvArr src, double value, CvArr dst)
        {
            NativeMethods.cvMinS(src, value, dst);
        }

        /// <summary>
        /// Finds per-element maximum of array and scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        public static void MaxS(CvArr src, double value, CvArr dst)
        {
            NativeMethods.cvMaxS(src, value, dst);
        }

        /// <summary>
        /// Calculates the absolute difference between two arrays.
        /// </summary>
        /// <param name="src1">The first input array.</param>
        /// <param name="src2">The second input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void AbsDiff(CvArr src1, CvArr src2, CvArr dst)
        {
            NativeMethods.cvAbsDiff(src1, src2, dst);
        }

        /// <summary>
        /// Calculates the absolute difference between an array and a scalar.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="value">The scalar input value.</param>
        /// <param name="dst">The destination array.</param>
        public static void AbsDiffS(CvArr src, CvArr dst, CvScalar value)
        {
            NativeMethods.cvAbsDiffS(src, dst, value);
        }

        /// <summary>
        /// Calculates the per-element absolute value of an array.
        /// </summary>
        /// <param name="src">The input array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Abs(CvArr src, CvArr dst)
        {
            AbsDiffS(src, dst, CvScalar.All(0));
        }

        #endregion

        #region Math operations

        /// <summary>
        /// Calculates the magnitude and/or angle of 2d vectors.
        /// </summary>
        /// <param name="x">The array of x-coordinates.</param>
        /// <param name="y">The array of y-coordinates.</param>
        /// <param name="magnitude">
        /// The destination array of magnitudes, may be set to <b>null</b> if it is not needed.
        /// </param>
        /// <param name="angle">
        /// The destination array of angles, may be set to <b>null</b> if it is not needed.
        /// The angles are measured in radians (0 to 2pi) or in degrees (0 to 360 degrees).
        /// </param>
        /// <param name="angleInDegrees">
        /// A value indicating whether the angles are measured in radians, which is the
        /// default mode, or in degrees.
        /// </param>
        public static void CartToPolar(CvArr x, CvArr y, CvArr magnitude, CvArr angle = null, bool angleInDegrees = false)
        {
            NativeMethods.cvCartToPolar(x, y, magnitude ?? CvArr.Null, angle ?? CvArr.Null, angleInDegrees ? 1 : 0);
        }

        /// <summary>
        /// Calculates Cartesian coordinates of 2d vectors represented in polar form.
        /// </summary>
        /// <param name="magnitude">
        /// The array of magnitudes. If it is <b>null</b>, the magnitudes are assumed to be all ones.
        /// </param>
        /// <param name="angle">The array of angles, whether in radians or degrees.</param>
        /// <param name="x">The destination array of x-coordinates, may be set to <b>null</b> if it is not needed.</param>
        /// <param name="y">The destination array of y-coordinates, may be set to <b>null</b> if it is not needed.</param>
        /// <param name="angleInDegrees">
        /// A value indicating whether the angles are measured in radians, which is the
        /// default mode, or in degrees.
        /// </param>
        public static void PolarToCart(CvArr magnitude, CvArr angle, CvArr x, CvArr y, bool angleInDegrees = false)
        {
            NativeMethods.cvPolarToCart(magnitude ?? CvArr.Null, angle, x ?? CvArr.Null, y ?? CvArr.Null, angleInDegrees ? 1 : 0);
        }

        /// <summary>
        /// Raises every array element to a power.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array, should be the same type as the source.</param>
        /// <param name="power">The exponent of power.</param>
        public static void Pow(CvArr src, CvArr dst, double power)
        {
            NativeMethods.cvPow(src, dst, power);
        }

        /// <summary>
        /// Calculates the exponent of every array element.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">
        /// The destination array, it should have double type or the same type
        /// as <paramref name="src"/>.
        /// </param>
        public static void Exp(CvArr src, CvArr dst)
        {
            NativeMethods.cvExp(src, dst);
        }

        /// <summary>
        /// Calculates the natural logarithm of every array element’s absolute value.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">
        /// The destination array, it should have double type or the same type
        /// as <paramref name="src"/>.
        /// </param>
        public static void Log(CvArr src, CvArr dst)
        {
            NativeMethods.cvLog(src, dst);
        }

        /// <summary>
        /// Calculates the angle of a 2D vector.
        /// </summary>
        /// <param name="y">The y-coordinate of the 2D vector.</param>
        /// <param name="x">The x-coordinate of the 2D vector.</param>
        /// <returns>
        /// The full-range angle of an input 2D vector. The angle is measured in
        /// degrees and varies from 0 degrees to 360 degrees.
        /// The accuracy is about 0.1 degrees.
        /// </returns>
        public static float FastArctan(float y, float x)
        {
            return NativeMethods.cvFastArctan(y, x);
        }

        /// <summary>
        /// Calculates the cubic root.
        /// </summary>
        /// <param name="value">The input floating-point value.</param>
        /// <returns>The cubic root of <paramref name="value"/>.</returns>
        public static float Cbrt(float value)
        {
            return NativeMethods.cvCbrt(value);
        }

        /// <summary>
        /// Checks that every array element is neither NaN nor Infinity. It can also check
        /// whether the elements are within a specified range.
        /// </summary>
        /// <param name="arr">The source array to check.</param>
        /// <param name="flags">
        /// The operation flags. A combination of <see cref="CheckArrayFlags.CheckNanInfinity"/>
        /// and <see cref="CheckArrayFlags.CheckRange"/>. If the latter is set, the function
        /// checks whether every value of the array is greater than or equal to <paramref name="min_val"/>
        /// and less than <paramref name="max_val"/>. If <see cref="CheckArrayFlags.CheckQuiet"/> is set,
        /// the function does not raise an error if an element is invalid or out of range.
        /// </param>
        /// <param name="min_val">The inclusive lower boundary of valid values range.</param>
        /// <param name="max_val">The exclusive upper boundary of valid values range.</param>
        /// <returns><b>true</b> if all array elements are valid and within range; <b>false</b> otherwise.</returns>
        public static bool CheckArr(CvArr arr, CheckArrayFlags flags, double min_val, double max_val)
        {
            return NativeMethods.cvCheckArr(arr, flags, min_val, max_val) != 0;
        }

        /// <summary>
        /// Initializes a random number generator state.
        /// </summary>
        /// <param name="seed">A 64-bit value used to initiate a random sequence.</param>
        /// <returns>The initialized random number generator state.</returns>
        public static ulong Rng(long seed)
        {
            unchecked
            {
                return seed > 0 ? (ulong)seed : (ulong)(long)-1;
            }
        }

        /// <summary>
        /// Returns a random 32-bit unsigned integer and updates the generator state.
        /// </summary>
        /// <param name="rng">The random number generator state initialized by <see cref="Rng"/>.</param>
        /// <returns>A uniformly-distributed random 32-bit unsigned integer.</returns>
        public static uint RandInt(ref ulong rng)
        {
            const ulong RngCoeff = 4164903690U;
            var temp = rng;
            temp = (ulong)(uint)temp * RngCoeff + (temp >> 32);
            rng = temp;
            return (uint)temp;
        }

        /// <summary>
        /// Returns a floating-point random number and updates the generator state.
        /// </summary>
        /// <param name="rng">The random number generator state initialized by <see cref="Rng"/>.</param>
        /// <returns>
        /// A uniformly-distributed random floating-point number between 0
        /// inclusive and 1 exclusive.
        /// </returns>
        public static double RandReal(ref ulong rng)
        {
            return RandInt(ref rng) * 2.3283064365386962890625e-10 /* 2^-32 */;
        }

        /// <summary>
        /// Fills an array with random numbers and updates the generator state.
        /// </summary>
        /// <param name="rng">The random number generator state initialized by <see cref="Rng"/>.</param>
        /// <param name="arr">The destination array.</param>
        /// <param name="distType">The type of distribution used to generate random numbers.</param>
        /// <param name="param1">
        /// The first parameter of the distribution. In the case of a uniform distribution it is
        /// the inclusive lower boundary of the random numbers range. In the case of a normal
        /// distribution it is the mean value of the random numbers.
        /// </param>
        /// <param name="param2">
        /// The second parameter of the distribution. In the case of a uniform distribution it is
        /// the exclusive upper boundary of the random numbers range. In the case of a normal
        /// distribution it is the standard deviation of the random numbers.
        /// </param>
        public static void RandArr(ref ulong rng, CvArr arr, CvRandDistribution distType, CvScalar param1, CvScalar param2)
        {
            NativeMethods.cvRandArr(ref rng, arr, distType, param1, param2);
        }

        /// <summary>
        /// Shuffles the array elements randomly and updates the generator state.
        /// </summary>
        /// <param name="mat">The source and destination array. The array is shuffled in place.</param>
        /// <param name="rng">The random number generator state initialized by <see cref="Rng"/>.</param>
        /// <param name="iterFactor">The scale factor that determines the number of random swap operations.</param>
        public static void RandShuffle(CvArr mat, ref ulong rng, double iterFactor = 1)
        {
            NativeMethods.cvRandShuffle(mat, ref rng, iterFactor);
        }

        /// <summary>
        /// Sorts each row or each column of a matrix.
        /// </summary>
        /// <param name="src">The source single-channel array.</param>
        /// <param name="dst">The destination array of the same size and the same type as <paramref name="src"/>.</param>
        /// <param name="indices">The array on which to store the sorted indices.</param>
        /// <param name="flags">
        /// The operation flags indicating whether to sort rows or columns and whether to use ascending
        /// or descending order.
        /// </param>
        public static void Sort(CvArr src, CvArr dst, CvArr indices, SortFlags flags)
        {
            NativeMethods.cvSort(src, dst, indices, flags);
        }

        /// <summary>
        /// Finds the real roots of a cubic equation.
        /// </summary>
        /// <param name="coeffs">The equation coefficients, an array of 3 or 4 elements.</param>
        /// <param name="roots">The output array of real roots which should have 3 elements.</param>
        /// <returns>The number of real roots found.</returns>
        public static int SolveCubic(CvMat coeffs, CvMat roots)
        {
            return NativeMethods.cvSolveCubic(coeffs, roots);
        }

        /// <summary>
        /// Finds the real or complex roots of a polynomial equation.
        /// </summary>
        /// <param name="coeffs">The array of polynomial coefficients.</param>
        /// <param name="roots2">The destination (complex) array of roots.</param>
        /// <param name="maxIter">The maximum number of iterations to perform.</param>
        /// <param name="fig">The required figures of precision.</param>
        public static void SolvePoly(CvMat coeffs, CvMat roots2, int maxIter = 20, int fig = 100)
        {
            NativeMethods.cvSolvePoly(coeffs, roots2, maxIter, fig);
        }

        #endregion

        #region Matrix operations

        /// <summary>
        /// Calculates the cross product of two 3D vectors.
        /// </summary>
        /// <param name="src1">The first source vector.</param>
        /// <param name="src2">The second source vector.</param>
        /// <param name="dst">The destination vector.</param>
        public static void CrossProduct(CvArr src1, CvArr src2, CvArr dst)
        {
            NativeMethods.cvCrossProduct(src1, src2, dst);
        }

        /// <summary>
        /// Performs generalized matrix multiplication.
        /// </summary>
        /// <param name="src1">The first source array.</param>
        /// <param name="src2">The second source array.</param>
        /// <param name="src3">
        /// The third source array (shift). Can be <b>null</b>, if there is no shift.
        /// </param>
        /// <param name="dst">The destination array.</param>
        public static void MatMulAdd(CvArr src1, CvArr src2, CvArr src3, CvArr dst)
        {
            GEMM(src1, src2, 1, src3, 1, dst, 0);
        }

        /// <summary>
        /// Performs generalized matrix multiplication.
        /// </summary>
        /// <param name="src1">The first source array.</param>
        /// <param name="src2">The second source array.</param>
        /// <param name="dst">The destination array.</param>
        public static void MatMul(CvArr src1, CvArr src2, CvArr dst)
        {
            MatMulAdd(src1, src2, CvArr.Null, dst);
        }

        /// <summary>
        /// Performs generalized matrix multiplication.
        /// </summary>
        /// <param name="src1">The first source array.</param>
        /// <param name="src2">The second source array.</param>
        /// <param name="alpha">A scale factor for the multiplication.</param>
        /// <param name="src3">
        /// The third source array (shift). Can be <b>null</b>, if there is no shift.
        /// </param>
        /// <param name="beta">A scale factor for the shift.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="tABC">
        /// The operation flags, used to indicate whether any of the inputs should be transposed.
        /// </param>
        public static void GEMM(CvArr src1, CvArr src2, double alpha, CvArr src3, double beta, CvArr dst, GemmFlags tABC = 0)
        {
            NativeMethods.cvGEMM(src1, src2, alpha, src3 ?? CvArr.Null, beta, dst, tABC);
        }

        /// <summary>
        /// Transforms each element of source array and stores resultant vectors in destination array.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="transmat">The transformation matrix to apply to elements of the source array.</param>
        /// <param name="shiftvec">The optional shift vector.</param>
        public static void Transform(CvArr src, CvArr dst, CvMat transmat, CvMat shiftvec = null)
        {
            NativeMethods.cvTransform(src, dst, transmat, shiftvec ?? CvMat.Null);
        }

        /// <summary>
        /// Performs perspective matrix transformation of a vector array.
        /// </summary>
        /// <param name="src">The source three-channel floating-point array.</param>
        /// <param name="dst">The destination three-channel floating-point array.</param>
        /// <param name="mat">The 3x3 or 4x4 transformation matrix.</param>
        public static void PerspectiveTransform(CvArr src, CvArr dst, CvMat mat)
        {
            NativeMethods.cvPerspectiveTransform(src, dst, mat);
        }

        /// <summary>
        /// Calculates the product of an array and a transposed array.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array. Must be of type F32 or F64.</param>
        /// <param name="order">The order of multipliers.</param>
        /// <param name="delta">An optional array, subtracted from <paramref name="src"/> before multiplication.</param>
        /// <param name="scale">An optional scale factor.</param>
        public static void MulTransposed(CvArr src, CvArr dst, int order, CvArr delta = null, double scale = 1)
        {
            NativeMethods.cvMulTransposed(src, dst, order, delta ?? CvArr.Null, scale);
        }

        /// <summary>
        /// Transposes a matrix.
        /// </summary>
        /// <param name="src">The source matrix.</param>
        /// <param name="dst">The destination matrix.</param>
        public static void Transpose(CvArr src, CvArr dst)
        {
            NativeMethods.cvTranspose(src, dst);
        }

        /// <summary>
        /// Copies the lower or the upper half of a square matrix to another half.
        /// </summary>
        /// <param name="matrix">The input-output floating point square matrix.</param>
        /// <param name="lowerToUpper">
        /// If <b>true</b>, the lower half is copied to the upper half, otherwise
        /// the upper half is copied to the lower half.
        /// </param>
        public static void CompleteSymm(CvMat matrix, bool lowerToUpper = false)
        {
            NativeMethods.cvCompleteSymm(matrix, lowerToUpper ? 1 : 0);
        }

        /// <summary>
        /// Flips a 2D array around vertical, horizontal or both axes.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array. If it is <b>null</b>, the flipping is done in place.</param>
        /// <param name="flipMode">A value that specifies how to flip the array.</param>
        public static void Flip(CvArr src, CvArr dst = null, FlipMode flipMode = FlipMode.Vertical)
        {
            NativeMethods.cvFlip(src, dst ?? CvArr.Null, flipMode);
        }

        /// <summary>
        /// Performs singular value decomposition of a real floating-point matrix.
        /// </summary>
        /// <param name="A">The source MxN matrix.</param>
        /// <param name="W">The resulting singular value diagonal matrix or vector of singular values.</param>
        /// <param name="U">The optional left orthogonal matrix.</param>
        /// <param name="V">The optional right orthogonal matrix.</param>
        /// <param name="flags">The operation flags that can be used to speed up the processing.</param>
        public static void SVD(CvArr A, CvArr W, CvArr U = null, CvArr V = null, SvdFlags flags = 0)
        {
            NativeMethods.cvSVD(A, W, U ?? CvArr.Null, V ?? CvArr.Null, flags);
        }

        /// <summary>
        /// Performs singular value back substitution.
        /// </summary>
        /// <param name="W">The matrix or vector of singular values.</param>
        /// <param name="U">The left orthogonal matrix (may be transposed).</param>
        /// <param name="V">The right orthogonal matrix (may be transposed).</param>
        /// <param name="B">
        /// The matrix to multiply the pseudo-inverse of the original matrix A by. If it is <b>null</b>
        /// it will be assumed to be an identity matrix of an appropriate size.
        /// </param>
        /// <param name="X">The destination matrix for the result of back substitution.</param>
        /// <param name="flags">The operation flags that were used to compute the singular values.</param>
        public static void SVBkSb(CvArr W, CvArr U, CvArr V, CvArr B, CvArr X, SvdFlags flags)
        {
            NativeMethods.cvSVBkSb(W, U, V, B ?? CvArr.Null, X, flags);
        }

        /// <summary>
        /// Finds the inverse or pseudo-inverse of a matrix.
        /// </summary>
        /// <param name="src">The source matrix.</param>
        /// <param name="dst">The destination matrix.</param>
        /// <param name="method">The inversion method.</param>
        /// <returns>
        /// The inversed condition of <paramref name="src"/> (ratio of the smallest singular value
        /// to the largest singular value) or 0 if <paramref name="src"/> is all zeros.
        /// </returns>
        public static double Invert(CvArr src, CvArr dst, InversionMethod method = InversionMethod.LU)
        {
            return NativeMethods.cvInvert(src, dst, method);
        }

        /// <summary>
        /// Solves a linear system or least-squares problem.
        /// </summary>
        /// <param name="src1">The source matrix.</param>
        /// <param name="src2">The right-hand part of the linear system.</param>
        /// <param name="dst">The output solution.</param>
        /// <param name="method">The matrix inversion method.</param>
        /// <returns>
        /// If <see cref="InversionMethod.LU"/> method is used, returns <b>true</b> if <paramref name="src1"/>
        /// is non-singular and <b>false</b> otherwise; in the latter case, <paramref name="dst"/> is not
        /// valid.
        /// </returns>
        public static bool Solve(CvArr src1, CvArr src2, CvArr dst, InversionMethod method = InversionMethod.LU)
        {
            return NativeMethods.cvSolve(src1, src2, dst, method) != 0;
        }

        /// <summary>
        /// Returns the determinant of a matrix.
        /// </summary>
        /// <param name="mat">The source matrix.</param>
        /// <returns>The determinant of the square matrix <paramref name="mat"/>.</returns>
        public static double Det(CvArr mat)
        {
            return NativeMethods.cvDet(mat);
        }

        /// <summary>
        /// Returns the trace of a matrix.
        /// </summary>
        /// <param name="mat">The source matrix.</param>
        /// <returns>The sum of the diagonal elements of the matrix <paramref name="mat"/>.</returns>
        public static CvScalar Trace(CvArr mat)
        {
            return NativeMethods.cvTrace(mat);
        }

        /// <summary>
        /// Computes eigenvalues and eigenvectors of a symmetric matrix.
        /// </summary>
        /// <param name="mat">The input symmetric square matrix, modified during the processing.</param>
        /// <param name="evects">The output matrix of eigenvectors, stored as subsequent rows.</param>
        /// <param name="evals">The output vector of eigenvalues, stored in the descending order.</param>
        /// <param name="eps">Accuracy of diagonalization.</param>
        /// <param name="lowindex">Optional index of largest eigenvalue/-vector to calculate.</param>
        /// <param name="highindex">Optional index of smallest eigenvalue/-vector to calculate.</param>
        public static void EigenVV(CvArr mat, CvArr evects, CvArr evals, double eps = 0, int lowindex = -1, int highindex = -1)
        {
            NativeMethods.cvEigenVV(mat, evects, evals, eps, lowindex, highindex);
        }

        /// <summary>
        /// Initializes a scaled identity matrix.
        /// </summary>
        /// <param name="mat">The matrix to initialize (not necessarily square).</param>
        public static void SetIdentity(CvArr mat)
        {
            SetIdentity(mat, CvScalar.Real(1));
        }

        /// <summary>
        /// Initializes a scaled identity matrix.
        /// </summary>
        /// <param name="mat">The matrix to initialize (not necessarily square).</param>
        /// <param name="value">The value to assign to the diagonal elements.</param>
        public static void SetIdentity(CvArr mat, CvScalar value)
        {
            NativeMethods.cvSetIdentity(mat, value);
        }

        /// <summary>
        /// Fills a matrix with the given range of numbers.
        /// </summary>
        /// <param name="mat">
        /// The matrix to initialize. It should be single-channel, 32-bit
        /// integer or floating-point.
        /// </param>
        /// <param name="start">The lower inclusive boundary of the range.</param>
        /// <param name="end">The upper exclusive boundary of the range.</param>
        /// <returns>The source matrix if the operation was successful, <b>null</b> otherwise.</returns>
        public static CvArr Range(CvArr mat, double start, double end)
        {
            return NativeMethods.cvRange(mat, start, end) != IntPtr.Zero ? mat : null;
        }

        /// <summary>
        /// Calculates covariance matrix of a set of vectors.
        /// </summary>
        /// <param name="vects">
        /// The input vectors, all of which must have the same type and the same size.
        /// The vectors do not have to be 1D, they can be 2D (e.g., images) and so forth.
        /// </param>
        /// <param name="covMat">The output covariance matrix that should be floating-point and square.</param>
        /// <param name="avg">
        /// The input or output (depending on the flags) array containing the mean (average)
        /// vector of the input vectors.
        /// </param>
        /// <param name="flags">A value specifying various operation flags.</param>
        public static void CalcCovarMatrix(CvArr[] vects, CvArr covMat, CvArr avg, CovarianceFlags flags)
        {
            var pImages = new IntPtr[vects.Length];
            for (int i = 0; i < vects.Length; i++)
            {
                pImages[i] = vects[i].DangerousGetHandle();
            }

            NativeMethods.cvCalcCovarMatrix(pImages, pImages.Length, covMat, avg, flags);
        }

        /// <summary>
        /// Performs PCA analysis of the vector set.
        /// </summary>
        /// <param name="data">The input data array; each vector is either a single row or a single column.</param>
        /// <param name="mean">The mean (average) vector.</param>
        /// <param name="eigenvals">The output eigenvalues of covariance matrix.</param>
        /// <param name="eigenvects">
        /// The output eigenvectors of covariance matrix (i.e. principal components); one vector per row.
        /// </param>
        /// <param name="flags">A value specifying various operation flags.</param>
        public static void CalcPCA(CvArr data, CvArr mean, CvArr eigenvals, CvArr eigenvects, PcaFlags flags)
        {
            NativeMethods.cvCalcPCA(data, mean, eigenvals, eigenvects, flags);
        }

        /// <summary>
        /// Projects vectors to the specified subspace.
        /// </summary>
        /// <param name="data">The input data array; each vector is either a single row or a single column.</param>
        /// <param name="mean">
        /// The mean (average) vector. If it is a single-row vector, then inputs are stored as rows;
        /// otherwise, it should be a single-column vector and inputs will be stored as columns.
        /// </param>
        /// <param name="eigenvects">The eigenvectors (principal components). One vector per row.</param>
        /// <param name="result">The output matrix containing the projected vectors.</param>
        public static void ProjectPCA(CvArr data, CvArr mean, CvArr eigenvects, CvArr result)
        {
            NativeMethods.cvProjectPCA(data, mean, eigenvects, result);
        }

        /// <summary>
        /// Back projects vectors from the specified subspace.
        /// </summary>
        /// <param name="proj">The input data array; each vector is either a single row or a single column.</param>
        /// <param name="mean">
        /// The mean (average) vector. If it is a single-row vector, then inputs are stored as rows;
        /// otherwise, it should be a single-column vector and inputs will be stored as columns.
        /// </param>
        /// <param name="eigenvects">The eigenvectors (principal components). One vector per row.</param>
        /// <param name="result">The output matrix containing the back projected vectors.</param>
        public static void BackProjectPCA(CvArr proj, CvArr mean, CvArr eigenvects, CvArr result)
        {
            NativeMethods.cvBackProjectPCA(proj, mean, eigenvects, result);
        }

        /// <summary>
        /// Calculates the Mahalonobis distance between two vectors.
        /// </summary>
        /// <param name="vec1">The first 1D source vector.</param>
        /// <param name="vec2">The second 1D source vector.</param>
        /// <param name="mat">The inverse covariance matrix.</param>
        /// <returns>The weighted Mahalanobis distance between two vectors.</returns>
        public static double Mahalanobis(CvArr vec1, CvArr vec2, CvArr mat)
        {
            return NativeMethods.cvMahalanobis(vec1, vec2, mat);
        }

        #endregion

        #region Array Statistics

        /// <summary>
        /// Adds up array elements.
        /// </summary>
        /// <param name="arr">The source array.</param>
        /// <returns>The sum of array elements, independently for each channel.</returns>
        public static CvScalar Sum(CvArr arr)
        {
            return NativeMethods.cvSum(arr);
        }

        /// <summary>
        /// Counts non-zero array elements.
        /// </summary>
        /// <param name="arr">
        /// The source array. Must be a single-channel array or a multi-channel
        /// image with COI set.
        /// </param>
        /// <returns>The number of non-zero elements in <paramref name="arr"/>.</returns>
        public static int CountNonZero(CvArr arr)
        {
            return NativeMethods.cvCountNonZero(arr);
        }

        /// <summary>
        /// Calculates average (mean) of array elements.
        /// </summary>
        /// <param name="arr">The source array.</param>
        /// <param name="mask">The optional operation mask.</param>
        /// <returns>The average value of array elements, independently for each channel.</returns>
        public static CvScalar Avg(CvArr arr, CvArr mask = null)
        {
            return NativeMethods.cvAvg(arr, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates average (mean) and standard deviation of array elements.
        /// </summary>
        /// <param name="arr">The source array.</param>
        /// <param name="mean">The output average of array elements, independently for each channel.</param>
        /// <param name="stdDev">The output standard deviation, independently for each channel.</param>
        /// <param name="mask">The optional operation mask.</param>
        public static void AvgSdv(CvArr arr, out CvScalar mean, out CvScalar stdDev, CvArr mask = null)
        {
            NativeMethods.cvAvgSdv(arr, out mean, out stdDev, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Finds global minimum and maximum in array or subarray.
        /// </summary>
        /// <param name="arr">The source array, single-channel or multi-channel with COI set.</param>
        /// <param name="minValue">The returned minimum value.</param>
        /// <param name="maxValue">The returned maximum value.</param>
        public static void MinMaxLoc(
            CvArr arr,
            out double minValue,
            out double maxValue)
        {
            CvPoint minLocation;
            CvPoint maxLocation;
            NativeMethods.cvMinMaxLoc(arr, out minValue, out maxValue, out minLocation, out maxLocation, CvArr.Null);
        }

        /// <summary>
        /// Finds global minimum and maximum in array or subarray.
        /// </summary>
        /// <param name="arr">The source array, single-channel or multi-channel with COI set.</param>
        /// <param name="minValue">The returned minimum value.</param>
        /// <param name="maxValue">The returned maximum value.</param>
        /// <param name="minLocation">The returned minimum location.</param>
        public static void MinMaxLoc(
            CvArr arr,
            out double minValue,
            out double maxValue,
            out CvPoint minLocation)
        {
            CvPoint maxLocation;
            NativeMethods.cvMinMaxLoc(arr, out minValue, out maxValue, out minLocation, out maxLocation, CvArr.Null);
        }

        /// <summary>
        /// Finds global minimum and maximum in array or subarray.
        /// </summary>
        /// <param name="arr">The source array, single-channel or multi-channel with COI set.</param>
        /// <param name="minValue">The returned minimum value.</param>
        /// <param name="maxValue">The returned maximum value.</param>
        /// <param name="minLocation">The returned minimum location.</param>
        /// <param name="maxLocation">The returned maximum location.</param>
        /// <param name="mask">The optional mask used to select a subarray.</param>
        public static void MinMaxLoc(
            CvArr arr,
            out double minValue,
            out double maxValue,
            out CvPoint minLocation,
            out CvPoint maxLocation,
            CvArr mask = null)
        {
            NativeMethods.cvMinMaxLoc(arr, out minValue, out maxValue, out minLocation, out maxLocation, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Calculates absolute array norm, absolute difference norm, or relative difference norm.
        /// </summary>
        /// <param name="arr1">The first source image.</param>
        /// <param name="arr2">
        /// The second source image. If it is <b>null</b>, the absolute norm of <paramref name="arr1"/>
        /// is calculated, otherwise the absolute or relative norm of
        /// <paramref name="arr1"/>-<paramref name="arr2"/> is calculated.
        /// </param>
        /// <param name="normType">The type of array norm.</param>
        /// <param name="mask">The optional operation mask.</param>
        /// <returns>The absolute or relative array norm.</returns>
        public static double Norm(CvArr arr1, CvArr arr2 = null, NormTypes normType = NormTypes.L2, CvArr mask = null)
        {
            return NativeMethods.cvNorm(arr1, arr2 ?? CvArr.Null, normType, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Normalizes the array norm or the range.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array. Must have the same size as <paramref name="src"/>.</param>
        /// <param name="a">The norm value to normalize to or the lower range boundary in the case of range normalization.</param>
        /// <param name="b">The upper range boundary in the case of range normalization; not used for norm normalization.</param>
        /// <param name="normType">The normalization type.</param>
        /// <param name="mask">The optional operation mask.</param>
        public static void Normalize(CvArr src, CvArr dst, double a = 1, double b = 0, NormTypes normType = NormTypes.L2, CvArr mask = null)
        {
            NativeMethods.cvNormalize(src, dst, a, b, normType, mask ?? CvArr.Null);
        }

        /// <summary>
        /// Reduces a matrix to a vector.
        /// </summary>
        /// <param name="src">The input matrix.</param>
        /// <param name="dst">The output single-row/single-column vector that accumulates all the matrix rows/columns.</param>
        /// <param name="dim">
        /// The dimension index along which the matrix is reduced. 0 means that the matrix is reduced
        /// to a single row, 1 means that the matrix is reduced to a single column and -1 means that the
        /// dimension is chosen automatically by analysing the <paramref name="dst"/> size.
        /// </param>
        /// <param name="op">The reduction operation.</param>
        public static void Reduce(CvArr src, CvArr dst, int dim = -1, ReduceOperation op = ReduceOperation.Sum)
        {
            NativeMethods.cvReduce(src, dst, dim, op);
        }

        #endregion

        #region Discrete Linear Transforms and Related Functions

        /// <summary>
        /// Performs a forward or inverse Discrete Fourier transform of a 1D or 2D floating-point array.
        /// </summary>
        /// <param name="src">The source array, containing real or complex values.</param>
        /// <param name="dst">The destination array of the same size and type as <paramref name="src"/>.</param>
        /// <param name="flags">The transformation flags specifying the operation of the DFT.</param>
        /// <param name="nonzeroRows">
        /// The number of nonzero rows in the source array (in the case of a forward 2d transform),
        /// or a number of rows of interest in the destination array (in the case of an inverse
        /// 2d transform).
        /// </param>
        public static void DFT(CvArr src, CvArr dst, DiscreteTransformFlags flags, int nonzeroRows)
        {
            NativeMethods.cvDFT(src, dst, flags, nonzeroRows);
        }

        /// <summary>
        /// Performs per-element multiplication of two Fourier spectrums.
        /// </summary>
        /// <param name="src1">The first source array.</param>
        /// <param name="src2">The second source array.</param>
        /// <param name="dst">The destination array of the same type and size as the source arrays.</param>
        /// <param name="flags">
        /// A combination of <see cref="DiscreteTransformFlags.Rows"/> and
        /// <see cref="DiscreteTransformFlags.MultiplyConjugate"/>.
        /// </param>
        public static void MulSpectrums(CvArr src1, CvArr src2, CvArr dst, DiscreteTransformFlags flags)
        {
            NativeMethods.cvMulSpectrums(src1, src2, dst, flags);
        }

        /// <summary>
        /// Returns optimal DFT size for a given vector size.
        /// </summary>
        /// <param name="size0">The vector size.</param>
        /// <returns>
        /// The minimum number N that is greater than or equal to <paramref name="size0"/>, such that
        /// the DFT of a vector of size N can be computed fast.
        /// </returns>
        public static int GetOptimalDFTSize(int size0)
        {
            return NativeMethods.cvGetOptimalDFTSize(size0);
        }

        /// <summary>
        /// Performs a forward or inverse Discrete Cosine transform of a 1D or 2D floating-point array.
        /// </summary>
        /// <param name="src">The source array, real 1D or 2D array.</param>
        /// <param name="dst">Destination array of the same size and type as <paramref name="src"/>.</param>
        /// <param name="flags">The transformation flags specifying the operation of the DCT.</param>
        public static void DCT(CvArr src, CvArr dst, DiscreteTransformFlags flags)
        {
            NativeMethods.cvDCT(src, dst, flags);
        }

        #endregion
    }
}
