namespace LFG.Utility
{
  public interface IPasswordHasher
  {
    string Hash(string password);
    bool Verify(string password, string inputPassword);
  }
}