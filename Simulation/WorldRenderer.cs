using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;

namespace ServoGapApp.Simulation
{
    public class WorldRenderer : IDisposable
    {
        private float _modelRotationDegrees = 0f;
        private readonly UiOpenGlShader _shader;
        private readonly int _vertexArrayObject;
        private readonly int _elementBufferObject;
        
        private readonly Dictionary<string, int> _vboCache = new();
        private readonly Dictionary<string, int> _vaoCache = new();
        private readonly int _vertexBufferObject;

        public WorldRenderer()
        {
            // setup the background color
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // load and compile the shaders
            _shader = new("Shaders/shader.vert", "Shaders/shader.frag");
        }

        public void Render(WorldSimulation simulation, double width, double height)
        {
            _shader!.Use();

            var model = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_modelRotationDegrees));
            var view = Matrix4.Identity;
            
            var aspectRatio = (float)( height / width) / 2f;                                  
            var limit = 11000f;
            var limitLeft = - (limit * aspectRatio);
            var limitRight = limit * aspectRatio;
            var projection = Matrix4.CreateOrthographicOffCenter(0, limit, limitLeft, limitRight, -1.0f, 1.0f);
    
            foreach (var conveyor in simulation.Conveyors)
            {
                RenderConveyor(conveyor, projection, view);
            }

            foreach (var box in simulation.Boxes)
            {
                //RenderBox(box, projection, view);
            }
        }

        private void RenderConveyor(ConveyorBase conveyor, Matrix4 projection, Matrix4 view)
        {
            var color = new Vector4(1f, 1f, 1f, 1.0f);
            /*if (conveyor is ConveyorServo servo)
            {
                if (servo.CorrectionSpeed < 0)
                    color = new Vector4(0.6f, 0.0f, 0.6f, 1.0f);
                else if (servo.CorrectionSpeed > 0)
                    color = new Vector4(0.0f, 0.6f, 0.6f, 1.0f);
            }*/
            
            if (!_vboCache.TryGetValue(conveyor.Name, out var vbo))
            {
                // create the vertex array object, vertex buffer object
                var vertexBufferObject = GL.GenBuffer();
                var vertexArrayObject = GL.GenVertexArray();
                _vboCache[conveyor.Name] = vertexBufferObject;
                _vaoCache[conveyor.Name] = vertexArrayObject;
                
                CreateVboForConveyor(conveyor, vertexArrayObject, vertexBufferObject);
                
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            }
            
            var model = Matrix4.CreateTranslation((float)conveyor.PosX, (float)conveyor.PosY, 0.0f);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetVector4("color", color);
            
            GL.DrawArrays( PrimitiveType.Triangles, 0, 6);
            
            _shader.SetVector4("color", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 6);
        }

        private void CreateVboForConveyor(ConveyorBase conveyor, int vertexArrayObject, int vertexBufferObject)
        {
            // bind the vertex array object
            GL.BindVertexArray(vertexArrayObject);
            // bind the vertex buffer object
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            // copy the vertex data into the _vertexArrayObject
               
            float[] vertices =
            {
                // First triangle (bottom-left, top-left, bottom-right)
                0, 0, 0,
                0, (float)conveyor.Height, 0,
                (float)conveyor.Width, 0, 0,
                // Second triangle (top-left, top-right, bottom-right)
                0,  (float)conveyor.Height, 0.0f,
                (float)conveyor.Width,  (float)conveyor.Height, 0.0f,
                (float)conveyor.Width, 0, 0
            };
            
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            // setup the vertex attribute position attribute
            var aPositionLocation = _shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(aPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(aPositionLocation);
        }

        private void RenderBox(Box box, Matrix4 projection, Matrix4 view)
        {
            var color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            if (box.Collision)
                color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (box.FutureCollision)
                color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);

            if (!_vboCache.TryGetValue(box.Name, out var vbo))
            {
                vbo = GL.GenBuffer();
                _vboCache[box.Name] = vbo; 
                
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            }

            var model = Matrix4.CreateTranslation((float)box.PosX, (float)box.PosY, 0.0f);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetVector4("color", color);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            _shader.SetVector4("color", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
        }
        
        public void Dispose()
        {
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader?.Dispose();
            GL.UseProgram(0);
            
            foreach (var vbo in _vboCache.Values)
            {
                GL.DeleteBuffer(vbo);
            }
            foreach (var vao in _vaoCache.Values)
            {
                GL.DeleteVertexArray(vao);
            }
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader?.Dispose();
            
        }
    }
}
