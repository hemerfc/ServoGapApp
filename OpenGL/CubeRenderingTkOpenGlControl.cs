using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.OpenGL
{
    public class CubeRenderingTkOpenGlControl : BaseTkOpenGlControl
    {
        private UiOpenGlShader? _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        
        private float _modelRotationDegrees = 0f;
        
        private readonly float[] _vertices =
        {
            // Position
            500f, 500f, 0.0f,  // top right
            500f, -500f, 0.0f, // bottom right
            -500f, -500f, 0.0f,// bottom left
            -500f, 500f, 0.0f, // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3 // second triangle
        };

        public CubeRenderingTkOpenGlControl()
        {
            Console.WriteLine("UI: Creating OpenGLControl");
        }

        protected override void OpenTkInit()
        {
            _shader = new("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(_shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(_shader.GetAttribLocation("aPosition"));

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        }

        protected override void OpenTkRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DoUpdate();
            DoRender();
        }

        protected override void OpenTkTeardown()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader?.Dispose();
            GL.UseProgram(0);
        }

        private void DoUpdate()
        {
            // _modelRotationDegrees += 1f;
        }

        private void DoRender()
        {
            _shader!.Use();

            var model = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_modelRotationDegrees));
            var view = Matrix4.Identity;
            
            var aspectRatio = (float)(Bounds.Width / Bounds.Height);
            var projection = Matrix4.CreateOrthographic(2000 * aspectRatio, 2000, -1.0f, 1.0f);
            
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}