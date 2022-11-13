class OnionAddrGenerator
{
    private static Random random = new Random();
    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvxwyz0123456789";
        var baseAddr=  new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        var onionAddr = "http://" + baseAddr + ".onion";
        return onionAddr;
    }
}