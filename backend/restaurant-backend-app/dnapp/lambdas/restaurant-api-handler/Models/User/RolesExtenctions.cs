using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models.User
{
    public static  class RolesExtenctions
    {
        public static string ToDynamoDBType(this Roles role)
        {
            return role switch
            {
                Roles.Customer => "Customer",
                Roles.Waiter => "Waiter",
                _ => throw new ArgumentException("Unknown role")
            };
        }
        public static Roles ToRoles(this string role)
        {
            return role switch
            {
                
                "Customer" => Roles.Customer,
                "Waiter" => Roles.Waiter,
                _ => throw new ArgumentException($"Invalid role: {role}", nameof(role))
            };
        }
    }
}
