using System.Collections.Generic;
using OpenTK.Mathematics;

namespace ServoGapApp.Simulation
{
    public abstract class World
    {
        public abstract IEnumerable<ConveyorBase> Conveyors { get; }

        public abstract IEnumerable<Box> Boxes { get; }

        public abstract bool SimEnabled { get; }

        public abstract void Update(double elapsed);

        public abstract void Pause();

        public object GetObject(double worldX, double worldY)
        {
            foreach (var box in Boxes)
            {
                if (PointIsInside(box.PosX, box.PosY, box.Width, box.Height, worldX, worldY))
                    return box;
            }

            foreach (var conv in Conveyors)
            {
                if (PointIsInside(conv.PosX, conv.PosY, conv.Width, conv.Height, worldX, worldY))
                    return conv;
            }

            return null;
        }

        private bool PointIsInside(double posX, double posY, double width, double height, double px, double py)
        {
            return (px > posX) && (px < posX + width) && (py > posY) && py < (posY + height);
        }
        
        public void Render(Matrix4 projection, Matrix4 view)
        {
            foreach (var conv in Conveyors)
            {
                if (conv is ConveyorServo servo)
                    servo.Render(projection, view);
                else if (conv is ConveyorStatic staticConv)
                    staticConv.Render(projection, view);
                else
                    conv.Render(projection, view, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            }

            foreach (var box in Boxes)
            {
                box.Render(projection, view);
            }
        }
    }
}
