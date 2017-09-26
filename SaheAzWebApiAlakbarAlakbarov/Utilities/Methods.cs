using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SaheAzWebApiAlakbarAlakbarov.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity;



namespace SaheAzWebApiAlakbarAlakbarov.Utilities
{
    public class Methods
    {
        private MyDbEntities db = new MyDbEntities();

        public string DecryptPassword(string password)
        {
            string hash = "alakbar";
            byte[] data = Convert.FromBase64String(password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes =
                      new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    password = Encoding.UTF8.GetString(results);
                }
            }
            return password;
        }

        public string EncryptPassword(string password)
        {
            string hash = "alakbar";
            byte[] data = Encoding.UTF8.GetBytes(password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes =
                      new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    password = Convert.ToBase64String(results, 0, results.Length);
                }
            }
            return password;
        }


        public bool UserExists(int id)
        {
            return db.User.Count(e => e.id == id) > 0;
        }

        //================================================================================================================================================>

        //public bool EmailExists(string email, int id)
        //{
        //    //int x = Convert.ToInt32(HttpContext.Current.User.Identity);
        //    return db.User.Any(e => e.email == email && e.id != id);
        //}


        //public bool MobileExists(string mobile, int id)
        //{
        //    //int x = Convert.ToInt32(HttpContext.Current.User.Identity);
        //    return db.User.Any(e => e.mobile == mobile && e.id != id);
        //}


        //public bool UsernameExists(string username, int id)
        //{
        //    //int x = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        //    //if (db.User.Where(x => x.id == id && x.username == username).FirstOrDefault().ToString() == null)
        //    //{
        //        return db.User.Any(e => e.username == username && e.id != id);
        //    //}
        //     //else
        //       //  return false;

        //}

        public bool EmailExists(string email, int id)
        {
            if (db.User.Any(x => x.email == email && x.id != id))
            {


                return true;
            }
            else
            {
                return false;
            }
        }


        public bool MobileExists(string phone, int id)
        {
            if (db.User.Any(x => x.mobile == phone && x.id != id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool UsernameExists(string username, int id)
        {
            if (db.User.Any(e => e.username == username && e.id != id))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return db.User.Any(e => e.userName == username && e.Id != Convert.ToInt32(HttpContext.Current.User.Identity.Name));
        }





        //================================================================================================================================================>

        public bool EmailExistsRegister(string email)
        {
            return db.User.Any(e => e.email == email);
        }


        public bool MobileExistsRegister(string mobile)
        {
            return db.User.Any(e => e.mobile == mobile);
        }


        public bool UsernameExistsRegister(string username)
        {
            return db.User.Any(e => e.username == username);
        }

//================================================================================================================================================>

        public bool UserExistWithUserNamePassword(string username, string password)
        {
            return db.User.Any(e => e.username == username && e.password == password);
        }


        public bool UserExistWithEmailPassword(string email, string password)
        {
            return db.User.Any(e => e.email == email && e.password == password);
        }
    }
}