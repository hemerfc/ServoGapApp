using System.Collections.Generic;
using OpenTK.Mathematics;

namespace ServoGapApp.Simulation
{
    public class ConveyorStatic : ConveyorBase
    {
        public WorldSimulation World { get; set; }
        public ConveyorStatic NextConv { get; set; }
        public List<Box> Boxes { get; set; }

        public ConveyorStatic(WorldSimulation world, string name, double posX, double posY, double width, double height, double speed, ConveyorStatic nextConveyor) :
            base(name, posX, posY, width, height, speed)
        {
            World = world;
            NextConv = nextConveyor;
            Boxes = new List<Box>();
        }

        public virtual void AddBox(Box box)
        {
            Boxes.Add(box);
            box.Conveyor = this;

            if (box.PosY == 0)
                box.PosY = PosY + (Height - box.Height) / 2;

            box.StartPosY = box.PosY;
            box.ChangePosY = PosY - box.PosY + (Height - box.Height) / 2;
        }

        public void AddBoxToZero(Box box)
        {
            Boxes.Add(box);
            box.Conveyor = this;

            box.PosY = PosY + (Height - box.Height) / 2;
            box.StartPosY = box.PosY;
            box.PosX = box.PosX - box.Width;
        }

        public virtual void Update(double elapsed)
        {
            var delta = elapsed * Speed;

            Position += delta;

            foreach (var box in Boxes)
            {
                box.PosX += delta;
            }
        }

        public virtual void Render(Matrix4 projection)
        {
            base.Render(projection, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
        }

        public virtual void AfterUpdate()
        {
            var boxToRemove = new List<Box>();
            foreach (var box in Boxes)
            {
                // a caixa deve estar 55% sobre a aproxima esteira para ser transferida
                if ((box.PosX + (box.Width * 0.55)) > (PosX + Width))
                {
                    boxToRemove.Add(box);

                    if (NextConv != null)
                        NextConv.AddBox(box);
                    else
                    {
                        // se é a ultima pista, e remove a caixa da lista global
                        World.RemoveBox(box);

                        // verifica se houve colisão e incremeta o total de colisoes
                        if (box.Collision)
                            World.TotalCollisionsOut += 1;

                        // incrementa o numero total de caixas na saida
                        World.CollisionsOut += 1;
                    }
                }
                else
                {
                    if (box.PosX > PosX)
                        box.PosY = box.StartPosY + easeInOutSine(box.PosX - PosX, 0, box.ChangePosY, Width * 0.9);
                }
            }
            boxToRemove.ForEach(b => Boxes.Remove(b));
        }
    }
}
