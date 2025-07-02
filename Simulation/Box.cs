using System;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.Simulation
{
    public class Box : IDisposable
    {
        private UiOpenGlShader? _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        public ConveyorStatic Conveyor { get; internal set; }
        public string ConveyorName => Conveyor?.Name ?? "";

        public string Name { get; set; }
        public int Idx { get; set; }

        [Category("Box")]
        public double PosX { get; set; }
        public double PosY { get; set; }

        [Category("Box")]
        public double Width { get; set; }
        public double Height { get; set; }
        public double StartPosY { get; set; }
        public double ChangePosY { get; set; }
        public bool Collision { get; set; }
        public bool FutureCollision { get; set; }
        public int Origin { get; set; }

        [Category("Box")]
        public double Correction { get; internal set; }

        [Category("Box")]
        public double FrontGap { get; internal set; }
        public double BackGap { get; internal set; }
        public double CorrectionRemaining { get; internal set; }

        private readonly float[] _vertices;
        private readonly uint[] _indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        public Box(double width, int origin)
        {
            Width = width;
            Origin = origin;
            Height = 200;

            _vertices = new[]
            {
                0.0f, 0.0f, 0.0f,
                (float)Width, 0.0f, 0.0f,
                (float)Width, (float)Height, 0.0f,
                0.0f, (float)Height, 0.0f
            };
        }

        public void Init()
        {
        }

        public void Dispose()
        {
        }
    }
}