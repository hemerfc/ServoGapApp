using System;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.Simulation
{
    public class ConveyorBase : IDisposable
    {
        private UiOpenGlShader? _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        public string Name { get; set; }
        public double PosX { set; get; }
        public double PosY { set; get; }
        public double Width { get; set; }
        public double Height { get; set; }

        [Category("Conveyor")]
        public double Speed { get; set; }

        [Category("Conveyor")]
        public double Position { get; set; }

        private readonly float[] _vertices;
        private readonly uint[] _indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        public ConveyorBase(string name, double posX, double posY, double width, double height, double speed)
        {
            Name = name;
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            Speed = speed;
            Position = 0;

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

        public double noEasing(double t, double b, double c, double d)
        {
            return (c * (t / d)) + b;
        }

        public double easeInOutSine(double t, double b, double c, double d)
        {
            return -c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b;
        }
    }
}