using System.ComponentModel;

namespace ServoGapApp.Simulation
{
    public class ConveyorMonitored : ConveyorBase
    {


        public ConveyorMonitored(string name, double posX, double posY, double width, double height, double speed) :
            base(name, posX, posY, width, height, speed)
        {
        }

        [Category("Conveyor")]
        public string _Name { get { return "CONV_" + Name; } }
        [Category("Conveyor")]
        public float Correction { get; internal set; }
        [Category("Conveyor")]
        public float NegCorr { get; internal set; }
        [Category("Conveyor")]
        public float PosCorr { get; internal set; }
    }
}
