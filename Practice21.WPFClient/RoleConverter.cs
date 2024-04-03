using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Practice21.MinimalAPI.Models;

namespace Practice21.WPFClient
{
    internal class RoleConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int roleId = (int)values[0];
            IEnumerable<Role> roles = (IEnumerable<Role>)values[1];
            return roles.FirstOrDefault(x => x.Id == roleId)?.Name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //string roleName = (string)value;
            throw new NotImplementedException();
        }
    }
}
