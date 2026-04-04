using System.Net.Mail;

namespace GameWithUsingInterface
{
    public interface ICombat
    {
        int Attack();
    }
    public interface ISupport
    {
        string Heal(int amount);
    }
    class Weapon: ICombat
    {
        public string Name { get; }
        public int BaseDamage { get; }
        public int BonusDamage { get; }
        public Weapon(string Name, int BonusDamage)
        {
            this.Name = Name;
            this.BonusDamage = BonusDamage;
        }
        public int Attack()
        {
            return 10 + BonusDamage;
        }
    }

    abstract class GameCharapter
    {
        public string Name { get; }
        public int HP { get; protected set; }
        public int Level { get; protected set; }
        public bool IsAlive { get => HP > 0; }

        public Weapon? EquippedWeapon { get; set; }
        protected GameCharapter(string Name, int HP, int Level)
        {
            this.Name = Name;
            this.HP = HP;
            this.Level = Level;
        }
        //                                Вывести это,    если это null, то
        protected int GetWeaponBonus() => EquippedWeapon?.BonusDamage ?? 0;
        void TakeDamage(int damage) => HP = Math.Max(0, HP - damage);
        public abstract string GetRole();
    }

    class Warrior: GameCharapter, ICombat
    {
        public Warrior(string Name, int HP, int Level) : base(Name, HP, Level) {}
        //                                Вывести это, если это null, то
        public string GetWeaponName() => EquippedWeapon?.Name ?? "без оружия";
        public int Attack() => Level * 10 + GetWeaponBonus();
        public override string GetRole() => "Воин";
    }
    class Mage: GameCharapter, ICombat, ISupport
    {
        public Mage(string Name, int HP, int Level): base(Name, HP, Level) {}
        public int Attack() => Level * 15 + GetWeaponBonus();
        public string Heal(int amount)
        {
            
            return $"{Name} лечит отряд на {amount}";
        }
        public override string GetRole() => "Маг";
    }
    class Party
    {
        private List<GameCharapter> _members = new();
        public GameCharapter this[int index] { // Тоже самое написано и в List<...>
            get => _members[index];
            set => _members[index] = value;
        }
        public void Add(GameCharapter GC) => _members.Add(GC);
        public void PrintAliveNames()
        {
            foreach(var i in _members)
            {
                if (i.IsAlive) Console.WriteLine(i.Name);
            }
        }
        public List<string> GetSupportNames()
        {
            List<string> result = new();
            
            foreach(var i in _members)
            {
                if (i.GetType().IsAssignableFrom(typeof(ISupport)))
                {
                    result.Add(i.Name);
                }
            }
            return result;
        }
        public int GetTotalHP()
        {
            int result = 0;
            foreach(var i in _members)
            {
                result += i.HP;
            }
            return result;
        }
        public List<string> GetNamesWithStrongWeapon(int minBonus)
        {
            List<string> result = new();
            foreach(var i in _members)
            {
                if (i.EquippedWeapon?.BonusDamage >= minBonus) result.Add(i.Name);
            }
            return result;
        }
        public void PrintSortedByLevelDesc()
        {
            foreach(var i in _members.OrderBy(x => x.Level).ToList())
            {
                // " [ПОГИБ]" if (!i.IsAlive()) else "";
                string status = i.IsAlive ? "" : " [ПОГИБ]";
                Console.WriteLine($"{i.GetRole()} {i.Name} (Ур. {i.Level}, HP: {i.HP}){status}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Party party = new();
            party.Add(new Warrior("Вояка", 100, 5));
            party.Add(new Mage("Фокус-Покус", 50, 10));
            party.Add(new Warrior("Слабак", 25, 1));
            party.Add(new Mage("Читер", 999, 99));
            party.PrintSortedByLevelDesc();
            // List<int> ints = new([0,1,2,3,4]);
            // Console.WriteLine(ints[0]);
            Console.WriteLine(party[0].Name);
        }
    }
}