using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ServoGapApp.Simulation
{
    public class ConveyorServo : ConveyorStatic
    {
        public double MinCorrSpeed { get; set; }
        public double MaxCorrSpeed { get; set; }
        public double Dec { get; set; }
        public double Acc { get; set; }
        public double Correction { get; private set; }
        public double CorrectionSpeed { get; private set; }
        public double CorrectionSum { get; private set; }
        public bool CorrectionDone { get; private set; }
        public bool NewBox { get; private set; }
        public Box CorrectionBox { get; private set; }
        public bool LastCorrection { get; private set; }

        public ConveyorServo(WorldSimulation world, string name, double posX, double posY, 
            double width, double height, double speed, ConveyorStatic nextConveyor, 
            double minimumCorrectionSpeed, double maximumCorrectionSpeed, double deceleration, 
            double acceleration, bool lastCorrection):
            base(world, name, posX, posY, width, height, speed, nextConveyor)
        {
            MinCorrSpeed = minimumCorrectionSpeed;
            MaxCorrSpeed = maximumCorrectionSpeed;
            Dec = deceleration;
            Acc = acceleration;
            CorrectionDone = true;
            LastCorrection = lastCorrection;
        }

        public override void Update(double elapsed)
        {
            var delta = elapsed * Speed;

            Position += delta;

            if (CorrectionBox != null)
            {
                if (Correction < 0)
                    CorrectionSpeed -= Dec * elapsed;
                else
                    CorrectionSpeed += Acc * elapsed;

                // speed limits
                CorrectionSpeed = CorrectionSpeed > MaxCorrSpeed ? MaxCorrSpeed : CorrectionSpeed;
                CorrectionSpeed = CorrectionSpeed < -MinCorrSpeed ? -MinCorrSpeed : CorrectionSpeed;
                
                var corrDelta = CorrectionSpeed * elapsed;
                CorrectionSum += corrDelta;
                CorrectionBox.CorrectionRemaining -= corrDelta;
                delta += corrDelta;

                if (Math.Abs(CorrectionSum) > Math.Abs(Correction))
                {
                    CorrectionBox = null;
                    CorrectionSum = 0;
                    Correction = 0;
                    CorrectionSpeed = 0;
                }
            }

            foreach (var box in Boxes)
            {
                box.PosX += delta;
            }
        }


        public override void AfterUpdate()
        {
            base.AfterUpdate();

            // se não esta corrigindo 
            if (Correction == 0 && NewBox)
            {
                // verifica se tem alguma caixa na esteira
                if (Boxes.Count > 0)
                {
                    // correcao disponivel e inicia a nova correção
                    if (Boxes.Count == 1)
                    {
                        CorrectionBox = Boxes[0];
                        Boxes[0].CorrectionRemaining = Boxes[0].Correction;
                        Correction = Boxes[0].Correction;
                    }
                    else if (Boxes.Count == 2)
                    {
                    }
                }

                NewBox = false;
            }
        }

        public override void AddBox(Box box)
        {
            base.AddBox(box);
            NewBox = true;

            if (box.FutureCollision && (box.ConveyorName == "c1_5" || box.ConveyorName == "c2_5" || box.ConveyorName == "merg"))
            {
                World.Pause();
            }
        }

        public new void Render(Matrix4 projection)
        {
            var color = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            if (CorrectionSpeed < 0)
                color = new Vector4(0.6f, 0.0f, 0.6f, 1.0f);
            else if (CorrectionSpeed > 0)
                color = new Vector4(0.0f, 0.6f, 0.6f, 1.0f);
            
            base.Render(projection, color);
        }
    }
}
