namespace TankWebObserver.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public decimal Score { get;set; }
        public decimal Hp { get; set; }
        public decimal MaxHp { get; set; }
        public TankCommon.Objects.Point LeftCorner { get; set; }

        public Player(int id, string nickname, decimal score, 
            decimal hp, decimal maxHp, TankCommon.Objects.Point leftCorner)
        {
            Id = id;
            Nickname = nickname;
            Score = score;
            Hp = hp;
            MaxHp = maxHp;
            LeftCorner = leftCorner;
        }

        public override string ToString()
        {
            return Id + "|" + Nickname + "|" + Score + "|" + Hp + "|" + MaxHp + "|" + LeftCorner.ToString();
        }
    }
}