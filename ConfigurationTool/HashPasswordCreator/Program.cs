using System.Text;

namespace HashPasswordCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World! I will create account");

            string password = "MesUser!0";
            string salt = DateTime.Now.ToString();

            byte[] bytePassword = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] byteSalt = System.Text.Encoding.UTF8.GetBytes(salt);

            string encryptedString = RSACommon.PasswordSecurity.EncryptString(password);
            Console.WriteLine($"{password} => Encrypted PWD: {encryptedString}");

            string decryptedString = RSACommon.PasswordSecurity.DecryptString(encryptedString);
            Console.WriteLine($"Decrypted PWD: {decryptedString}");

            if (password == decryptedString)
                Console.WriteLine("OK");

            password = "Robots";
            encryptedString = RSACommon.PasswordSecurity.EncryptString(password);
            Console.WriteLine($"{password} => Encrypted PWD: {encryptedString}");
        }
    }
}