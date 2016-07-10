using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpMvvmExtension
{
    public static class DataContextExtension
    {
        public static void Execute(this object dataContext, string commandName, object parameter = null)
        {
            if (dataContext != null)
            {
                var command = dataContext.GetType().GetProperty(commandName).GetValue(dataContext) as ICommand;
                if (command != null)
                {
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
            }
        }

        public static T GetPopertyValue<T>(this object dataContext, string popertyName, object parameter = null)
        {
            T propertyValue = default(T);
            if (dataContext != null)
            {
                object propertyValueObject = dataContext.GetType().GetProperty(popertyName).GetValue(dataContext);
                if (propertyValueObject != null && propertyValueObject is T)
                    return propertyValue = (T)propertyValueObject;
            }

            return propertyValue;
        }
    }
}
