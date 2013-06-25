using Raven.Imports.Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SessionTracker.Web.Entities
{
    public class Speaker
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        [JsonIgnore]
        public string GravatarHash
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(Email))
                {
                    // Create a new instance of the MD5CryptoServiceProvider object.
                    using (MD5 md5Hasher = MD5.Create())
                    {
                        // Convert the input string to a byte array and compute the hash.
                        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Email));

                        // Create a new Stringbuilder to collect the bytes  
                        // and create a string.
                        StringBuilder sBuilder = new StringBuilder();

                        // Loop through each byte of the hashed data  
                        // and format each one as a hexadecimal string.
                        for (int i = 0; i < data.Length; i++)
                        {
                            sBuilder.Append(data[i].ToString("x2"));
                        }

                        // Return the hexadecimal string.
                        return sBuilder.ToString();
                    }
                }

                return null;
            }
        }
    }
}