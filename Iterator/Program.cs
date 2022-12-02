namespace Iterator;
struct Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

interface IAggregate
{
    IIterator CreateIterator();
}

interface IIterator
{
    Employee HasItem(bool first=false);
    void Reset();
}

class EmployeeAggregate : IAggregate
{
    List<Employee> Employees = new List<Employee>();
    public int Count { get => Employees.Count; }
    public void Add(Employee Model) => Employees.Add(Model);
    public Employee GetItem(int index) => Employees[index];
    public IIterator CreateIterator() => new EmployeeIterator(this);
}

class EmployeeIterator : IIterator
{
    EmployeeAggregate aggregate;
    int currentindex;
    public EmployeeIterator(EmployeeAggregate aggregate) => this.aggregate = aggregate;
    public Employee HasItem(bool firstCheck=false)
    {
        if (!firstCheck)
        {
            if (currentindex < aggregate.Count)
                return aggregate.GetItem(currentindex++);
            return new Employee();
        }
        else
        {
            if (currentindex < aggregate.Count)
                return aggregate.GetItem(currentindex);
            return new Employee();
        }
    }
    
    public void Reset()
    {
        currentindex = 0;
    }
}
class Program
{
    static void Main()
    {
        EmployeeAggregate aggregate = new EmployeeAggregate();
        aggregate.Add(new Employee { Id = 1, Name = "Mehmet", Surname = "Karahanli" });
        aggregate.Add(new Employee { Id = 2, Name = "Polat", Surname = "Alemdar" });
        aggregate.Add(new Employee { Id = 3, Name = "Suleyman", Surname = "Cakir" });
        aggregate.Add(new Employee { Id = 4, Name = "Memati", Surname = "Bas" });


        IIterator iterator = aggregate.CreateIterator();
        while (iterator.HasItem(true).Name is not null)
        {
            var nextItem = iterator.HasItem();
            Console.WriteLine($"ID : {nextItem.Id}\nName : {nextItem.Name}\nSurname : {nextItem.Surname}\n*****");
        }
        iterator.Reset();
        while (iterator.HasItem(true).Name is not null)
        {
            var nextItem = iterator.HasItem();
            Console.WriteLine($"ID : {nextItem.Id}\nName : {nextItem.Name}\nSurname : {nextItem.Surname}\n*****");
        }

    }
}