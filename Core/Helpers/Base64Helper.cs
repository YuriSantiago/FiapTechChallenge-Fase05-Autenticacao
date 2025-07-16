using System.Text;

namespace Core.Helpers
{
    public static class Base64Helper
    {

        public static string Encode(string senha)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(senha));
        }

        public static string Decode(string senha)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(senha));
        }

    }
}
