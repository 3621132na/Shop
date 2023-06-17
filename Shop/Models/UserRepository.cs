using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class UserRepository
    {
        private ShopDataDataContext dataContext;
        public UserRepository()
        {
            dataContext = new ShopDataDataContext();
        }
        public void UpdatePassword(string newPassword)
        {
            var currentUser = dataContext.KHACHHANGs.FirstOrDefault();
            if (currentUser != null)
            {
                currentUser.Matkhau = newPassword;
                dataContext.SubmitChanges();
            }
        }
    }
}