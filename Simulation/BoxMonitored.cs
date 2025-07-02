using System;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.Simulation
{
    public class BoxMonitored : Box
    {
        [Category("Box")]
        public string _Name { get { return "BOX_" + Idx; } }

        [Category("Box")]
        public int ConveyorId { get; internal set; }

        public BoxMonitored(double width, int origin) : base(width, origin)
        {
            ConveyorId = 0;
        }
    }
}
