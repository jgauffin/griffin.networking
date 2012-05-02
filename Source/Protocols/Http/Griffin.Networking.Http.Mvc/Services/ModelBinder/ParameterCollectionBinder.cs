using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Griffin.Networking.Http.Implementation;

namespace Griffin.Networking.Http.Mvc.Services.ModelBinder
{
    public class ParameterCollectionBinder
    {
        public void Bind(object model, ParameterCollection ps)
        {
            foreach (var parameter in ps)
            {
                var property = model.GetType().GetProperty(parameter.Name,
                                                           BindingFlags.NonPublic | BindingFlags.Instance);
                if (property == null)
                    continue;

                if (property.c)
                property.SetValue();
            }
        }
    }


    public 

    public interface IValueConverter
    {

        
    }
}
