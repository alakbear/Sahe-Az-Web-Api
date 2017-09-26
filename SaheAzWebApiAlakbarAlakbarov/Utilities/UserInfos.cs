using SaheAzWebApiAlakbarAlakbarov.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaheAzWebApiAlakbarAlakbarov.Utilities
{
    public class UserInfos
    {
        public static User UserInfo()
        {
                using (MyDbEntities db = new MyDbEntities())
                    return db.User.Find(HttpContext.Current.User.Identity.Name);
            
        }
    }

}