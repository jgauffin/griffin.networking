using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Mvc.Services.ModelBinder;

namespace Griffin.Networking.Http.Mvc.Tests.Services.ModelBinders
{
    public class MapClass
    {
        public void Map()
        {
            var binder = new ParameterCollectionBinder();
            var ps = new ParameterCollection();
            ps.Add("FirstName", "Jonas");
            ps.Add("Age", "22");

            var user = new User();
            binder.Bind(user, ps);
        }

        public class User
        {
            public string FirstName { get; set; }
            public string Age { get; set; }
        }
    }

}
