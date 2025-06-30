using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

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

        public override void Render(Matrix4 projection)
        {
            if (ConveyorId >= 0 && Width > 0)
            {

                PosY = (ConveyorId < 6) ? 150.0 : -150.0;

                if (Collision)
                    GL.Color3(1.0, 0.0, 0.0);
                else
                {
                    if (FutureCollision)
                        GL.Color3(1.0, 1.0, 0.0);
                    else
                        GL.Color3(0.0, 1.0, 0.0);
                }

                GL.LoadName(Idx);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(PosX, PosY);
                GL.Vertex2(PosX + Width, PosY);
                GL.Vertex2(PosX + Width, PosY + Height);
                GL.Vertex2(PosX, PosY + Height);
                GL.End();


                GL.Color3(1.0, 1.0, 1.0);
                GL.Begin(PrimitiveType.LineStrip);
                GL.Vertex2(PosX, PosY);
                GL.Vertex2(PosX + Width, PosY);
                GL.Vertex2(PosX + Width, PosY + Height);
                GL.Vertex2(PosX, PosY + Height);
                GL.End();
            }
        }
    }
}
