namespace NasaSyncService.Infrastructure.Hash
{
    internal interface IHasher
    {
        string ComputeHash(string input);
    }
}
