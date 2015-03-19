﻿using KGP.Direct3D11.Descriptors;
using Microsoft.Kinect;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KGP.Direct3D11.Buffers
{
    /// <summary>
    /// Represent a Structured Buffer holding joint orientations
    /// </summary>
    public unsafe class BodyJointOrientationBuffer : IDisposable
    {
        private SharpDX.Direct3D11.Buffer buffer;
        private ShaderResourceView shaderView;

        /// <summary>
        /// Shader view
        /// </summary>
        public ShaderResourceView ShaderView
        {
            get { return this.shaderView; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">Direct3D11 Device</param>
        public BodyJointOrientationBuffer(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            this.buffer = new SharpDX.Direct3D11.Buffer(device, JointBufferDescriptor.DynamicBuffer(BufferStride.From<Vector4>()));
            this.shaderView = new ShaderResourceView(device, this.buffer);
        }

        /// <summary>
        /// Copies color space joints to gpu
        /// </summary>
        /// <param name="context"></param>
        /// <param name="joints"></param>
        public void Copy(DeviceContext context, IEnumerable<KinectBody> joints)
        {
            var jointArray = joints.SelectMany(cpj => cpj.JointOrientations.Values.Select(j => j.Orientation).ToArray()).ToArray();

            if (jointArray.Length == 0)
                return;

            fixed (Vector4* cptr = & jointArray[0])
            {
                this.buffer.Upload(context, new IntPtr(cptr), Marshal.SizeOf(typeof(Vector4)) * jointArray.Length);
            }
        }

        /// <summary>
        /// Dispose GPU resources
        /// </summary>
        public void Dispose()
        {
            this.shaderView.Dispose();
            this.buffer.Dispose();
        }
    }
}
