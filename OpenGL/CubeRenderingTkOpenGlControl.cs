using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ServoGapApp.Shaders;
using ServoGapApp.Simulation;

namespace ServoGapApp.OpenGL
{
    public class CubeRenderingTkOpenGlControl : BaseTkOpenGlControl
    {
        public WorldRenderer WorldRenderer;
        public WorldSimulation? WorldSimulation;
        
        public CubeRenderingTkOpenGlControl()
        {
            Console.WriteLine("UI: Creating OpenGLControl");
        }

        protected override void OpenTkInit()
        {
            WorldSimulation = new WorldSimulation(() => { });
            WorldRenderer = new WorldRenderer();
        }

        protected override void OpenTkRender()
        {
            DoUpdate();
            DoRender();
        }

        protected override void OpenTkTeardown()
        {
            WorldRenderer.Dispose();
        }

        private void DoUpdate()
        {
        }

        private void DoRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            WorldRenderer.Render(WorldSimulation, Bounds.Width, Bounds.Height);
        }
    }
}