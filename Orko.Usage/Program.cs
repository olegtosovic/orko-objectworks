using Orko.ObjectWorks.Core;
using Orko.Usage.Models;
using System.Diagnostics;
using System.Reflection;

partial class Program
{
    private static readonly Random rd = new Random();

    private static readonly int SampleCount = 100_000_000;

    private static readonly int StringLength = 3;

    private static string CreateString(int stringLength)
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
        char[] chars = new char[stringLength];

        for (int i = 0; i < stringLength; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    private static void TestNative()
    {
        // Create stopwatch.
        var stopWatch = new Stopwatch();
        for (int i = 0; i < SampleCount; i++)
        {
            // Create test instance.
            var model5 = new Model5();

            // Start mesaure.
            stopWatch.Start();

            // Use native setters.
            model5.Id = i + 1;
            model5.Name = "Oleg Tošović";
            model5.Description = "Person who is testing this.";
            model5.Type = "Robot";
            model5.ChangedDate = DateTime.UtcNow;

            // Stop mesaure.
            stopWatch.Stop();
        }

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    private static void TestReflection()
    {
        // Create object context instance.
        var objectContext = ObjectContext<Model5>.GetInstance();

        // Container.
        var list = new List<Model5>();

        // Get all properties.
        var id = objectContext.GetProperty("Id");
        var name = objectContext.GetProperty("Name");
        var description = objectContext.GetProperty("Description");
        var type = objectContext.GetProperty("Type");
        var changedDate = objectContext.GetProperty("ChangedDate");

        // Create stopwatch.
        var stopWatch = new Stopwatch();
        for (int i = 0; i < SampleCount; i++)
        {
            // Create test instance.
            var model5 = new Model5();

            // Start mesaure.
            stopWatch.Start();

            // Use fast setters.
            id.SetValueFast(model5, i+1);
            name.SetValueFast(model5, "Oleg Tošović");
            description.SetValueFast(model5, "Person who is testing this.");
            type.SetValueFast(model5, "Robot");
            changedDate.SetValueFast(model5 , DateTime.UtcNow);

            // Stop mesaure.
            stopWatch.Stop();

            // Add to list.
            // list.Add(memoryObject);
        }

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    /// <summary>
    /// Main app entry point.
    /// </summary>
    static void Main(string[] args)
    {
        Console.WriteLine("Test Native");
        TestNative();

        Console.WriteLine("Test compiled cached method");
        TestReflection();

        Console.WriteLine("Test compiled cached method");
        TestReflection();
    }
}