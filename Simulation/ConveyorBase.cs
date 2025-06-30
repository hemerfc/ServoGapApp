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
            _shader = new UiOpenGlShader("Shaders/conveyor.vert", "Shaders/conveyor.frag");
            _shader.Use();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            var aPositionLocation = _shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(aPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(aPositionLocation);
        }

        public virtual void Render(Matrix4 projection, Vector4 color)
        {
            _shader.Use();
            
            _shader.SetVector4("color", color);

            var model = Matrix4.CreateTranslation((float)PosX, (float)PosY, 0.0f);
            var view = Matrix4.LookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.UnitY);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader?.Dispose();
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