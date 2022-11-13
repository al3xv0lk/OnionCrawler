class OnionAddrGenerator
{
    private static Random random = new Random();
    public static string RandOnion()
    {
        const string chars = "abcdefghijklmnopqrstuvxwyz0123456789";
        var baseAddr=  new string(Enumerable.Repeat(chars, 56)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        var onionAddr = "http://" + baseAddr + ".onion";
        // System.Console.WriteLine($"New addr: {onionAddr}");
        return onionAddr;
    }
}