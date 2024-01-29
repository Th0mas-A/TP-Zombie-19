namespace TPZombie_19
{
    public static class Program
    {
        private static Dictionary<Variant, Action<Person>> _variantToMethod = new()
        {
            [Variant.ZombieA] = SpreadVariantA,
            [Variant.ZombieB] = SpreadVariantB,
            [Variant.Zombie32] = (person) => { SpreadVariant32Parent(person); SpreadVariant32Child(person); },
            [Variant.ZombieC] = SpreadVariantC,
            [Variant.ZombieUltime] = SpreadVariantUltime,
        };

        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            //Initialize datas
            var christian = new Person("Christian", false, true, false, 79, new() { Variant.ZombieA }, null, new());
            var regis = new Person("Regis", false, true, false, 55, new(), christian, new());
            var christelle = new Person("Christelle", false, true, false, 54, new() { Variant.ZombieC }, christian, new());
            christian.Children.Add(regis);
            christian.Children.Add(christelle);
            var louis = new Person("Louis", false, true, false, 24, new() { Variant.ZombieC }, regis, new());
            var thomas = new Person("Thomas", false, true, false, 22, new() { Variant.ZombieB }, regis, new());
            var ines = new Person("Ines", false, true, false, 19, new(), regis, new());
            regis.Children.Add(louis);
            regis.Children.Add(thomas);
            regis.Children.Add(ines);
            var marion = new Person("Marion", false, true, false, 28, new() { Variant.Zombie32 }, christelle, new());
            var clement = new Person("Clement", false, true, false, 22, new(), christelle, new());
            var baptiste = new Person("Baptiste", false, true, false, 20, new() { Variant.ZombieUltime }, christelle, new());
            christelle.Children.Add(marion);
            christelle.Children.Add(clement);
            christelle.Children.Add(baptiste);

            InfectPerson(christian);
            ImmunePerson(christian);
        }

        #region Variants
        static bool CanBeInfected(Person person, Variant variant, Func<Person, bool>? condition)
        {
            return !person.IsImmune 
                && person.CanInfect 
                && !person.IsDead
                && !person.Variants.Contains(variant)
                && (condition is null || condition(person));
        }

        static void SpreadVariantA(Person person)
        {
            if(CanBeInfected(person, Variant.ZombieA, null))
            {
                Console.WriteLine($"{person.Name} a ete infecte par le variant A");
                person.Variants.Add(Variant.ZombieA);
            }

            foreach (var child in person.Children)
                SpreadVariantA(child);
        }

        static void SpreadVariantB(Person person)
        {
            if (CanBeInfected(person, Variant.ZombieB, null))
            {
                Console.WriteLine($"{person.Name} a ete infecte par le variant B");
                person.Variants.Add(Variant.ZombieB);
            }

            if(person.Parent is not null)
                SpreadVariantB(person.Parent);
        }

        static void SpreadVariant32Parent(Person person)
        {
            if (CanBeInfected(person, Variant.Zombie32, (person) => person.Age >= 32 ))
            {
                Console.WriteLine($"{person.Name} a ete infecte par le variant 32");
                person.Variants.Add(Variant.Zombie32);
            }

            if(person.Parent is not null)
                SpreadVariant32Parent(person.Parent);
        }

        static void SpreadVariant32Child(Person person)
        {
            if (CanBeInfected(person, Variant.Zombie32, (person) => person.Age >= 32))
            {
                Console.WriteLine($"{person.Name} a ete infecte par le variant 32");
                person.Variants.Add(Variant.Zombie32);
            }

            foreach (var child in person.Children)
                SpreadVariant32Child(child);
        }

        static void SpreadVariantC(Person person)
        {
            foreach(var child in person.Children)
            {
                if (CanBeInfected(person, Variant.ZombieC, (person) => _random.Next(0, 2) == 0)) 
                {
                    Console.WriteLine($"{person.Name} a ete infecte par le variant C");
                    person.Variants.Add(Variant.ZombieC);
                }
            }
        }

        static void SpreadVariantUltime(Person person)
        {
            if(CanBeInfected(person, Variant.ZombieUltime, (person) => person.Parent is null))
            {
                Console.WriteLine($"{person.Name} a ete infecte par le variant Ultime");
                person.Variants.Add(Variant.ZombieUltime);
            }

            if(person.Parent != null)
                SpreadVariantUltime(person.Parent);
        }

        static void InfectPerson(Person person)
        {
            foreach(var variant in person.Variants)
                if (_variantToMethod.TryGetValue(variant, out var action))
                    action(person);

            foreach (var child in person.Children)
                InfectPerson(child);
        }
        #endregion

        #region Immunities

        static void SpreadImmuneA1(Person person)
        {
            if(person.Variants.Contains(Variant.ZombieA) || person.Variants.Contains(Variant.Zombie32))
            {
                if(person.Age <= 30)
                {
                    Console.WriteLine($"{person.Name} a ete vaccine par le vaccin A1");
                    person.Variants.Clear();
                    person.IsImmune = true;
                }
                else
                {
                    Console.WriteLine($"{person.Name} a plus de 30 ans, le vaccin A1 est inefficace");
                }
            }

            foreach (var child in person.Children)
                SpreadImmuneA1(child);
        }

        static void SpreadImmuneB1(Person person)
        {
            if (person.Variants.Contains(Variant.ZombieB) || person.Variants.Contains(Variant.ZombieC))
            {
                if(_random.Next(0, 2) == 0)
                {
                    Console.WriteLine($"{person.Name} a ete vaccine par le vaccin B1");
                    person.Variants = person.Variants.Where(variant => variant != Variant.ZombieB && variant != Variant.ZombieC).ToList();
                }
                else
                {
                    Console.WriteLine($"{person.Name} a ete tue par le vaccin B1");
                    person.IsDead = true;
                }
            }

            foreach (var child in person.Children)
                SpreadImmuneB1(child);
        }

        static void SpreadImmuneUltime(Person person)
        {
            if (person.Variants.Contains(Variant.ZombieUltime))
            {
                Console.WriteLine($"{person.Name} a ete vaccine par le vaccin Ultime");
                person.Variants.Clear();
                person.IsImmune = true;
                person.CanInfect = false;
            }

            foreach (var child in person.Children)
                SpreadImmuneUltime(child);
        }

        static void ImmunePerson(Person person)
        {
            SpreadImmuneA1(person);
            SpreadImmuneB1(person);
            SpreadImmuneUltime(person);
        }

        #endregion
    }
}