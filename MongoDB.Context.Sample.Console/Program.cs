using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MongoDB.Context.Sample
{
  public static class Program
  {
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "A method test.")]
    public static async Task Main(string[] args)
    {
      Console.WriteLine("Hello World!");
    }
  }
}
