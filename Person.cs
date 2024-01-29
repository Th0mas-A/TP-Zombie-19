namespace TPZombie_19;

internal class Person
{
    public Person(string name, bool isImmune, bool canInfect, bool isDead, int age, List<Variant> variants, Person? parent, List<Person> children)
    {
        Name = name;
        IsImmune = isImmune;
        CanInfect = canInfect;
        IsDead = isDead;
        Age = age;
        Variants = variants;
        Parent = parent;
        Children = children;
    }

    public string Name
    {
        get;
        set;
    }

    public bool IsImmune 
    { 
        get;
        set;
    }

    public bool CanInfect
    {
        get;
        set;
    }

    public bool IsDead
    {
        get;
        set;
    }

    public int Age
    {
        get;
        set;
    }

    public List<Variant> Variants
    {
        get;
        set;
    }

    public Person? Parent
    {
        get;
        set;
    }

    public List<Person> Children
    {
        get;
        set;
    }
}
