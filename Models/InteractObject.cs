namespace TankWebObserver.Models
{
    public class InteractObject
    {
        public string Type { get; set; }
        public TankCommon.Objects.Point LeftCorner { get; set; }

        public InteractObject(string type, TankCommon.Objects.Point leftCorner)
        {
            Type = type;
            LeftCorner = leftCorner;
        }

        public override string ToString()
        {
            return Type + "|" + LeftCorner;
        }
    }
}