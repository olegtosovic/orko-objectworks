using Orko.ObjectWorks.Core;
using Orko.Usage.Models;
using System.Diagnostics;

partial class Program
{
    /// <summary>
    /// Number of iterations.
    /// </summary>
    private static readonly int SampleCount = 100_000_000;

    /// <summary>
    /// Test time required for getting values normally.
    /// </summary>
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
            //model5.Name = "Oleg Tošović";
            model5.Guid = Guid.NewGuid().ToString();
            //model5.Type = "Robot";
            model5.ChangedDate = DateTime.UtcNow;

            // Stop mesaure.
            stopWatch.Stop();
        }

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    /// <summary>
    /// Test time required for getting values normally.
    /// </summary>
    private static void TestNativeParalell()
    {
        // Create stopwatch.
        var stopWatch = new Stopwatch();

        // Run paralell.
        var range = Enumerable.Range(0, SampleCount);
        ParallelOptions options = new ParallelOptions();
        options.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;
        Parallel.ForEach(range, options, i =>
        {
            // Create test instance.
            var model5 = new Model5();

            // Start mesaure.
            stopWatch.Start();

            // Use native setters.
            model5.Id = i + 1;
            //model5.Name = "Oleg Tošović";
            model5.Guid = Guid.NewGuid().ToString();
            //model5.Type = "Robot";
            model5.ChangedDate = DateTime.UtcNow;

            // Stop mesaure.
            stopWatch.Stop();
        });

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    /// <summary>
    /// Test time required for getting values using cached reflection.
    /// </summary>
    private static void TestReflection()
    {
        // Create object context instance.
        var objectContext = ObjectContext<Model5>.GetInstance();

        // Container.
        var list = new List<Model5>();

        // Get all properties.
        var id = objectContext.GetProperty("Id");
        var name = objectContext.GetProperty("Name");
        var guid = objectContext.GetProperty("Guid");
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
            id.SetValueFast(model5, i + 1);
            //name.SetValueFast(model5, "Oleg Tošović");
            guid.SetValueFast(model5, Guid.NewGuid().ToString());
            //type.SetValueFast(model5, "Robot");
            changedDate.SetValueFast(model5, DateTime.UtcNow);

            // Stop mesaure.
            stopWatch.Stop();

            // Add to list.
            // list.Add(memoryObject);
        }

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    /// <summary>
    /// Test time required for getting values using cached reflection.
    /// </summary>
    private static void TestReflectionParalell()
    {
        // Create object context instance.
        var objectContext = ObjectContext<Model5>.GetInstance();

        // Container.
        var list = new List<Model5>();

        // Get all properties.
        var id = objectContext.GetProperty("Id");
        var name = objectContext.GetProperty("Name");
        var guid = objectContext.GetProperty("Guid");
        var type = objectContext.GetProperty("Type");
        var changedDate = objectContext.GetProperty("ChangedDate");

        // Create stopwatch.
        var stopWatch = new Stopwatch();

        var range = Enumerable.Range(0, SampleCount);

        ParallelOptions options = new ParallelOptions();
        options.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;
        Parallel.ForEach(range, options, i =>
        {
            // Create test instance.
            var model5 = new Model5();

            // Start mesaure.
            stopWatch.Start();

            // Use fast setters.
            id.SetValueFast(model5, i + 1);
            //name.SetValueFast(model5, "Oleg Tošović");
            guid.SetValueFast(model5, Guid.NewGuid().ToString());
            //type.SetValueFast(model5, "Robot");
            changedDate.SetValueFast(model5, DateTime.UtcNow);

            // Stop mesaure.
            stopWatch.Stop();
        });

        // Report.
        Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000F);
    }

    /// <summary>
    /// Test time required for getting values using DLR.
    /// </summary>
    private static void TestDynamic()
    {
        // Create stopwatch.
        var stopWatch = new Stopwatch();
        for (int i = 0; i < SampleCount; i++)
        {
            // Create test instance.
            dynamic model5 = new Model5();

            // Start mesaure.
            stopWatch.Start();

            // Use native setters.
            model5.Id = i + 1;
            //model5.Name = "Oleg Tošović";
            model5.Guid = Guid.NewGuid().ToString();
            //model5.Type = "Robot";
            model5.ChangedDate = DateTime.UtcNow;

            // Stop mesaure.
            stopWatch.Stop();
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

        Console.WriteLine("Test Native paralell");
        TestNativeParalell();

        Console.WriteLine("Test compiled cached method");
        TestReflection();

        Console.WriteLine("Test compiled cached method paralell");
        TestReflectionParalell();
    }
}