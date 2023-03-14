using System.Linq;
using JavaCommons;
using LiteDB;

namespace CUI;
// Create your POCO class entity
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] Phones { get; set; }
    public bool IsActive { get; set; }
}
public static class Program
{
    public static void Main()
    {
        Util.Print(new[] { "Hello", "World" });
// Open database (or create if doesn't exist)
        using(var db = new LiteDatabase(@"MyData.ldb"))
        {
            // Get a collection (or create, if doesn't exist)
            var col = db.GetCollection<Customer>("customers");

            // Create your new customer instance
            var customer = new Customer
            { 
                Name = "John Doe", 
                Phones = new string[] { "8000-0000", "9000-0000" }, 
                IsActive = true
            };
	
            // Insert new customer document (Id will be auto-incremented)
            col.Insert(customer);
	
            // Update a document inside a collection
            customer.Name = "Jane Doe";
	
            col.Update(customer);
	
            // Index document using document Name property
            col.EnsureIndex(x => x.Name);
	
            // Use LINQ to query documents (filter, sort, transform)
            var results = col.Query()
                .Where(x => x.Name.StartsWith("J"))
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
                .Limit(10)
                .ToList();
            
            Util.Print(results);

            // Let's create an index in phone numbers (using expression). It's a multikey index
            col.EnsureIndex(x => x.Phones); 

            // and now we can query phones
            var r = col.FindOne(x => x.Phones.Contains("8000-0000"));
            Util.Print(r);
        }
    }
}