using System;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.Simulation
{
    public class BoxMonitored : Box, IDisposable
    {
        private UiOpenGlShader _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        
        private readonly float[] _vertices;
        private readonly uint[] _indices =
        {
            0, 1, 2,
            2, 3, 0
        };
        
        [Category("Box")]
        public string _Name { get { return "BOX_" + Idx; } }

        [Category("Box")]
        public int ConveyorId { get; internal set; }

        public BoxMonitored(double width, int origin) : base(width, origin)
        {
            ConveyorId = 0;
            
            _vertices = new[]
            {
                0.0f, 0.0f, 0.0f,
                (float)Width, 0.0f, 0.0f,
                (float)Width, (float)Height, 0.0f,
                0.0f, (float)Height, 0.0f
            };
            
            _shader = new UiOpenGlShader("Shaders/monitored_box.vert", "Shaders/monitored_box.frag");
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

        public override void Render(Matrix4 projection, Matrix4 view)
        {
            if (ConveyorId >= 0 && Width > 0)
            {
                PosY = (ConveyorId < 6) ? 150.0 : -150.0;

                var color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                if (Collision)
                    color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                else if (FutureCollision)
                    color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
                
                _shader.Use();
                _shader.SetVector4("color", color);

                var model = Matrix4.CreateTranslation((float)PosX, (float)PosY, 0.0f);

                _shader.SetMatrix4("model", model);
                _shader.SetMatrix4("view", view);
                _shader.SetMatrix4("projection", projection);

                GL.BindVertexArray(_vertexArrayObject);
                GL.DrawElements(PrimitiveType.Quads, _indices.Length, DrawElementsType.UnsignedInt, 0);
                
                _shader.SetVector4("color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                GL.DrawElements(PrimitiveType.LineStrip, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }
        
        public new void Dispose()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader?.Dispose();
            base.Dispose();
        }
    }
}
