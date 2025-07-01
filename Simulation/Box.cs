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
            _shader = new UiOpenGlShader("Shaders/box.vert", "Shaders/box.frag");
            _shader.Use();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            var aPositionLocation = _shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(aPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(aPositionLocation);
        }

        public virtual void Render(Matrix4 projection, Matrix4 view)
        {
            _shader.Use();

            var color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            if (Collision)
                color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (FutureCollision)
                color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
            
            _shader.SetVector4("color", color);

            var model = Matrix4.CreateTranslation((float)PosX, (float)PosY, 0.0f);

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
    }
}