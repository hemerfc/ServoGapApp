using System;
using System.Collections.Generic;
using System.Linq;

namespace ServoGapApp.Simulation
{
    public class WorldSimulation : World
    {
        public Dictionary<string, ConveyorStatic> ConvDict { get; set; }
        public List<Box> BoxesList { get; set; }
        public override IEnumerable<ConveyorBase> Conveyors { get { return ConvDict.Values.Cast<ConveyorBase>(); } }
        public override IEnumerable<Box> Boxes { get { return BoxesList; } }
        public override bool SimEnabled { get { return true; } }
        public int CollisionsOut { get; set; }
        public int TotalCollisionsOut { get; set; }
        public double TotalTime { get; set; }
        public double StatisticUpdateTimer { get; set; }
        public double GenPos1 { get; set; }
        public double GenPos2 { get; set; }
        public Random Rand { get; set; }
        public double[] BoxSizes { get; private set; }
        public int GapTotalWidth { get; private set; }
        public double BoxTotalWidth { get; private set; }
        public int BoxTotalCount { get; private set; }
        public Action PauseFunc { get; private set; }

        public double Tolerance = 20;
        public double GapRequired = 100;
        public double GapInputSpeed = 666;
        public double GapSpeed = 2000;
        public double GapOutputSpeed = 1333;
        public double GapCorrSpeed = 1100;
        public double MinimunCorrection = -400.0;
        public double MaximumCorrection = 300.0;

        public WorldSimulation(Action pauseFunc)
        {
            BoxesList = new List<Box>();
            BoxSizes = new[] { 250.0, 300.0, 350.0, 450.0, 700.0 };

            ConvDict = new Dictionary<string, ConveyorStatic>();
            ConvDict.Add("c3_1", new ConveyorStatic(this, "c3_1", 8000, 0, 3000, 200, GapOutputSpeed, null));
            ConvDict.Add("merg", new ConveyorStatic(this, "merg", 6000, -150, 2000, 500, GapOutputSpeed, ConvDict["c3_1"]));

            ConvDict.Add("c1_5", new ConveyorServo(this, "c1_5", 4800, -150, 1200, 200, GapSpeed, ConvDict["merg"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, true));
            ConvDict.Add("c1_4", new ConveyorServo(this, "c1_4", 3600, -150, 1200, 200, GapSpeed, ConvDict["c1_5"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));
            ConvDict.Add("c1_3", new ConveyorServo(this, "c1_3", 2400, -150, 1200, 200, GapSpeed, ConvDict["c1_4"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));
            ConvDict.Add("c1_2", new ConveyorServo(this, "c1_2", 1200, -150, 1200, 200, GapSpeed, ConvDict["c1_3"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));

            ConvDict.Add("c2_5", new ConveyorServo(this, "c2_5", 4800, 150, 1200, 200, GapSpeed, ConvDict["merg"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, true));
            ConvDict.Add("c2_4", new ConveyorServo(this, "c2_4", 3600, 150, 1200, 200, GapSpeed, ConvDict["c2_5"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));
            ConvDict.Add("c2_3", new ConveyorServo(this, "c2_3", 2400, 150, 1200, 200, GapSpeed, ConvDict["c2_4"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));
            ConvDict.Add("c2_2", new ConveyorServo(this, "c2_2", 1200, 150, 1200, 200, GapSpeed, ConvDict["c2_3"], GapCorrSpeed, GapCorrSpeed, 6000, 6000, false));

            ConvDict.Add("c1_1", new ConveyorStatic(this, "c1_1", 0, -150, 1200, 200, GapInputSpeed, ConvDict["c1_2"]));
            ConvDict.Add("c2_1", new ConveyorStatic(this, "c2_1", 0, 150, 1200, 200, GapInputSpeed, ConvDict["c2_2"]));

            Rand = new Random(100);

            GenPos1 = Rand.Next(200, 280);
            GenPos2 = Rand.Next(500, 730);

            PauseFunc = pauseFunc;

            foreach (var conv in ConvDict.Values)
            {
                conv.Init();
            }
        }


        public override void Update(double elapsed)
        {
            foreach (var conv in ConvDict.Values)
            {
                conv.Update(elapsed);
            }

            foreach (var conv in ConvDict.Values)
            {
                conv.AfterUpdate();
            }

            BoxCreation();

            // ordena a lista usando PosX
            BoxesList.Sort((box1, box2) => box1.PosX.CompareTo(box2.PosX));

            CollisionsDetection();

            CorrectionLogic();

            TotalTime += elapsed;
            StatisticUpdateTimer += elapsed;
        }


        private void CorrectionLogic()
        {
            Box frontBox = null;
            Box backBox = null;

            // este loop percorre a lista no sentido contrario
            // atualiza cada caixa, com os gaps
            for (int i = BoxesList.Count - 1; i > 0; i--)
            {
                var cBox = BoxesList[i];
                cBox.Idx = i;

                // se é a primeira caixa
                if (frontBox == null)
                    cBox.FrontGap = 99999.0;
                else
                    cBox.FrontGap = frontBox.PosX - (cBox.PosX + cBox.Width);

                // se não é a ultima caixa
                if (i > 0)
                {
                    backBox = BoxesList[i - 1];
                    cBox.BackGap = cBox.PosX - (backBox.PosX + backBox.Width);
                }
                else
                    cBox.BackGap = 0.0;

                frontBox = cBox;
            }

            frontBox = null;
            backBox = null;
            // este loop percorre a lista no sentido contrario
            // atualiza cada caixa, com sua correcao
            for (int i = BoxesList.Count - 1; i > 0; i--)
            {
                var cBox = BoxesList[i];
                cBox.Correction = 0;
                if (i < (BoxesList.Count - 1))
                    frontBox = BoxesList[i + 1];
                if (i > 0)
                    backBox = BoxesList[i - 1];

                if (frontBox != null)
                {
                    var P1 = (frontBox.Width * 0.55);
                    var P2 = (cBox.Width * 0.55);
                    var Delta = 1 - (GapOutputSpeed / GapSpeed);
                    var GapComPercaFront = (GapRequired + P1 * Delta + P2 * Delta) / (1 - Delta);

                    var frontBoxCorrection = 0.0;
                    if (frontBox.Conveyor is ConveyorServo && !((ConveyorServo)frontBox.Conveyor).LastCorrection)
                        frontBoxCorrection = frontBox.Correction;

                    // se precisa afastar da caixa da frente
                    if ((cBox.FrontGap + frontBoxCorrection) != GapComPercaFront)
                    {
                        // segura a cBox para complementar o gap
                        var corr = (cBox.FrontGap + frontBoxCorrection) - GapComPercaFront;

                        if (Math.Abs(corr) > Tolerance)
                            cBox.Correction = corr;
                        else // não faz nenhuma correcao
                            cBox.Correction = 0;
                    }
                }
                else
                {
                    if (backBox != null)
                    {
                        var P1 = (backBox.Width * 0.55);
                        var P2 = (cBox.Width * 0.55);
                        var Delta = 1 - (GapOutputSpeed / GapSpeed);
                        var GapComPercaBack = (GapRequired + P1 * Delta + P2 * Delta) / (1 - Delta);

                        // se precisa afastar da caixa de traz
                        if (cBox.BackGap < GapComPercaBack)
                        {
                            var corr = GapComPercaBack - cBox.BackGap;

                            if (Math.Abs(corr) > Tolerance)
                                cBox.Correction = corr;
                            else // não faz nenhuma correcao
                                cBox.Correction = 0;
                        }
                    }
                }
                
                // A CORRECAO MAXIMA NÃO DEVE PASSAR DA POSICAO EM QUE A CAIXA É TRANSFERIDA PARA A PROXIMA ESTEIRA
                // EX: SE A CAIXA (POS=800, WIDTH=300) 
                // ESTA NAPRIMEIRA ESTEIRA (POS=0, WIDTH=1200)
                // TRANSF_BOX_PERCENT = .55 TRANSF_BOX_FIT = 100
                // CORR_END = C.POS + C.WIDTH + (B.WIDTH * TRANSF_BOX_PERCENT) - R00_BOX_TRANSF_FIT 
                //          =     0 +    1200 + (    300 *               0.55) -                100 = 1265
                // CORR_LIMIT = LIMITE DE TRANSF - B.POS - B.WIDTH 
                //            =             1265 -   800 -     300 = 165         

                var conveyorEnd = cBox.Conveyor.PosX + cBox.Conveyor.Width;
                var transfLimit = cBox.Width * 0.55;
                var corr_end = conveyorEnd + transfLimit - 200;
                var corr_limit = corr_end - cBox.PosX - cBox.Width;

                cBox.Correction = Math.Min(corr_limit, Math.Max(-corr_limit, cBox.Correction));
                cBox.Correction = Math.Min(MaximumCorrection, Math.Max(MinimunCorrection, cBox.Correction));
            }
        }


        private void BoxCreation()
        {
            if (GenPos1 < ConvDict["c1_1"].Position)
            {
                var BoxWidth = BoxSizes[Rand.Next(1, BoxSizes.Length)];
                var GapWidth = Rand.Next(100, 280);

                var box = new Box(BoxWidth, 1);
                box.Init();
                BoxesList.Insert(0, box);
                ConvDict["c1_1"].AddBoxToZero(box);

                GenPos1 = ConvDict["c1_1"].Position + box.Width + GapWidth;

                GapTotalWidth += GapWidth;
                BoxTotalWidth += BoxWidth;
                BoxTotalCount += 1;
            }

            if (GenPos2 < ConvDict["c2_1"].Position)
            {
                var BoxWidth = BoxSizes[Rand.Next(1, BoxSizes.Length)];
                var GapWidth = Rand.Next(100, 280);

                var box = new Box(BoxWidth, 2);
                box.Init();
                BoxesList.Insert(0, box);
                ConvDict["c2_1"].AddBoxToZero(box);

                GenPos2 = ConvDict["c2_1"].Position + box.Width + GapWidth;

                GapTotalWidth += GapWidth;
                BoxTotalWidth += BoxWidth;
                BoxTotalCount += 1;
            }
        }

        private void CollisionsDetection()
        {
            foreach (var box in Boxes)
            {
                box.Collision = false;
                box.FutureCollision = false;
            }

            // from Boxes.Count to 1 by -1
            for (int i = BoxesList.Count - 1; i > 0; i--)
            {
                var cBox = BoxesList[i];
                var nBox = BoxesList[i - 1];

                /*if (cBox.Origin != nBox.Origin)
                {*/
                if (checkXCollision(cBox, nBox))
                {
                    cBox.FutureCollision = true;
                    nBox.FutureCollision = true;

                    if (checkYCollision(cBox, nBox))
                    {
                        cBox.Collision = true;
                        nBox.Collision = true;
                    }
                }
                //}
            }
        }

        private bool checkXCollision(Box cBox, Box nBox)
        {
            var x1 = cBox.PosX;
            var x2 = cBox.PosX + cBox.Width;
            var x3 = nBox.PosX;
            var x4 = nBox.PosX + nBox.Width;

            return (x1 >= x3 && x1 <= x4) || (x2 >= x3 && x2 <= x4) || (x3 >= x1 && x3 <= x2);
        }

        private bool checkYCollision(Box cBox, Box nBox)
        {
            var y1 = cBox.PosY;
            var y2 = cBox.PosY + cBox.Height;
            var y3 = nBox.PosY;
            var y4 = nBox.PosY + nBox.Height;

            return (y1 >= y3 && y1 <= y4) || (y2 >= y3 && y2 <= y4) || (y3 >= y1 && y3 <= y2);
        }

        internal void RemoveBox(Box box)
        {
            box.Dispose();
            BoxesList.Remove(box);
        }

        public override void Pause()
        {
            PauseFunc();
        }

    }
}

