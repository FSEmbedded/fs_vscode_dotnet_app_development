internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        for (int i = 0; i < 10; ++i)
        {
            Console.WriteLine($"Running... {i + 1}");
            Thread.Sleep(1000);
        }

        Console.WriteLine("Finished running.\n");
    }
}