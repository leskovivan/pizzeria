using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace pizzeria.Data.Helpers
{
    public static class ApplicationExtensions
    {
        public static string GetDescription(this Enum myEnum)
        {
            return myEnum.GetType()
                .GetMember(myEnum.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>().Name;
        }
    }
}
