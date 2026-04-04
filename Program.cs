namespace GameWithUsingInterface
{
    interface ICombat
    {
        int Attack();
    }
    interface ISupport
    {
        void Heal(List<GameCharapter> targets, int amount);
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
        protected GameCharapter(string Name, int HP, int Level, Weapon? EquippedWeapon)
        {
            this.Name = Name;
            this.HP = HP;
            this.Level = Level;
            this.EquippedWeapon = EquippedWeapon;
        }
        public void Say(string value)
        {
            Console.WriteLine($"[{Name}] - {value}");
        }
        //                                Вывести это,    если это null, то
        protected int GetWeaponBonus() => EquippedWeapon?.BonusDamage ?? 0;
        public void TakeHeal(int amount) => HP += amount;
        public void TakeDamage(int damage) => HP = Math.Max(0, HP - damage);
        public abstract string GetRole();
    }

    class Warrior: GameCharapter, ICombat
    {
        public Warrior(string Name, int HP, int Level, Weapon? EquippedWeapon) : base(Name, HP, Level, EquippedWeapon) {}
        //                                Вывести это, если это null, то
        public string GetWeaponName() => EquippedWeapon?.Name ?? "без оружия";
        public int Attack() => Level * 10 + GetWeaponBonus();
        public override string GetRole() => "Воин";
    }
    class Mage: GameCharapter, ICombat, ISupport
    {
        public Mage(string Name, int HP, int Level, Weapon? EquippedWeapon): base(Name, HP, Level, EquippedWeapon) {}
        public int Attack() => Level * 15 + GetWeaponBonus();
        public void Heal(List<GameCharapter> targets, int amount)
        {
            foreach(var i in targets)
            {
                if (i.IsAlive)
                {
                    i.TakeHeal(amount);
                }
            }
            Console.WriteLine($"{Name} лечит отряд на {amount}");
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
        
        public void HealAll(ISupport healer, int amount)
        {
            healer.Heal(_members, amount);
        }

        public List<string> GetSupportNames()
        {
            List<string> result = new();
            
            foreach(var i in _members)
            {
                if (i is ISupport)
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
        
        public void PrintAliveNames()
        {
            foreach(var i in _members)
            {
                if (i.IsAlive) Console.WriteLine(i.Name);
            }
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
            party.Add(new Warrior("Вояка", 100, 5, new Weapon("Кинжал", 10)));
            party.Add(new Mage("Фокус-Покус", 50, 10, new Weapon("Посох", 5)));
            party.Add(new Warrior("Слабак-полудурок", 25, 1, null));
            party.Add(new Mage("Читер", 999, 99, null));
            Console.WriteLine("Герои в группе:");
            party.PrintSortedByLevelDesc();
            Console.WriteLine();
            // List<int> ints = new([0,1,2,3,4]);
            // Console.WriteLine(ints[0]);

            Warrior war1 = (Warrior)party[0];
            Console.WriteLine($"{war1.Name} атакует на {war1.Attack()}");
            Console.WriteLine($"{war1.Name} держит оружие {war1.GetWeaponName()}");
            Console.WriteLine();
            
            Warrior war2 = (Warrior)party[2];
            Console.WriteLine($"{war2.Name} может нанести {war2.Attack()} урона");

            Console.WriteLine($"{war2.Name} бьет {war1.Name} и наносит {war2.Attack()}");
            war1.TakeDamage(war2.Attack());
            Console.WriteLine();

            // Console.WriteLine($"[{war1.Name}] - Ай, ты дурак что-ли, {war2.Name}?");
            war1.Say($"Ай, ты дурак что-ли, {war2.Name}?");
            Console.WriteLine($"{war1.Name}, HP: {war1.HP}");
            Console.WriteLine();

            Mage mag1 = (Mage)party[1];
            party.HealAll(mag1, 10);
            Console.WriteLine($"{war1.Name}, HP: {war1.HP}");
        }
    }
}