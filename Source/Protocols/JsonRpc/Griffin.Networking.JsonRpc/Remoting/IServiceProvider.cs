using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace Griffin.Networking.JsonRpc.Remoting
{
    public interface IServiceProvider
    {
        /// <summary>
        /// Invoke the RPC request.
        /// </summary>
        /// <param name="request">RPC request</param>
        /// <returns>Response to send back</returns>
        ResponseBase Invoke(Request request);
    }

    /// <summary>
    /// Basic implementation of a service provider.
    /// </summary>
    /// <remarks>
    /// <para>Override <see cref="GetInstance"/> if you want to use your InversionOfControl container (you can create a new scope just before and end it after)</para>
    /// <para>
    /// 
    /// </para>
    /// </remarks>
    public class ServiceProvider
    {
        private Dictionary<string, Mapping> _mappings = new Dictionary<string, Mapping>();

        class Mapping
        {
            public Type ServiceType { get; set; }
            public MethodInfo Method { get; set; }
        }

        /// <summary>
        /// Map a service to a RPC method
        /// </summary>
        /// <typeparam name="T">Type which methods are decorated with the <see cref="OperationContractAttribute"/>.</typeparam>
        /// <remarks>
        /// The <c>Name</c> property of <see cref="OperationContractAttribute"/> must equal the name in the RPC request.
        /// </remarks>
        public void Map<T>()
        {
            foreach (var methodInfo in typeof(T).GetMethods())
            {
                var attr = methodInfo.GetCustomAttributes(typeof(OperationContractAttribute), true).Cast<OperationContractAttribute>().FirstOrDefault();
                if (attr == null)
                    continue;


                _mappings.Add(attr.Name, new Mapping { ServiceType = typeof(T), Method = methodInfo });
            }
        }

        protected virtual ErrorResponse CreateErrorResponse(object id, int errorCode, string message)
        {
            return new ErrorResponse(id, new RpcError
                                             {
                                                 Code = errorCode,
                                                 Message = message
                                             });
        }

        public ResponseBase Invoke(Request request)
        {
            Mapping mapping;
            if (!_mappings.TryGetValue(request.Method, out mapping))
            {
                return CreateErrorResponse(request.Id, RpcErrorCode.MethodNotFound,
                                           string.Format("Failed to find method named '{0}'.", request.Method));
            }

            object result;
            object[] parameters;

            var methodParameters = mapping.Method.GetParameters();
            if (request.Parameters is IDictionary<string, object>)
            {
                var dictionary = (IDictionary<string, object>)request.Parameters;
                ResponseBase errorResponse;
                if (ConvertToObjectArray(request, methodParameters, dictionary, out errorResponse))
                    return errorResponse;
            }

            parameters = (object[])request.Parameters;
            if (parameters.Length != methodParameters.Length)
            {
                return CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                           "Expected '" + methodParameters.Length + "' number of parameters.");
            }

            ErrorResponse response;
            object[] fixedParameters = ConvertParameters(request, methodParameters, out response);
            if (fixedParameters == null)
                return response;


            var instance = GetInstance(mapping.ServiceType);
            result = mapping.Method.Invoke(instance, fixedParameters);

            return new Response(request.Id, result);
        }

        private bool ConvertToObjectArray(Request request, IEnumerable<ParameterInfo> methodParameters, IDictionary<string, object> requestParameters,
                                          out ResponseBase errorResponse)
        {
            var parameters = new object[requestParameters.Count];
            var index = 0;
            foreach (var methodParameter in methodParameters)
            {
                object value;
                if (!requestParameters.TryGetValue(methodParameter.Name, out value))
                {
                    errorResponse = CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                                        "Failed to find a parameter named '" + methodParameter.Name +
                                                        "' in the RPC request (which the service requires).");
                    return true;
                }

                parameters[index++] = value;
            }

            errorResponse = null;
            return false;
        }

        protected virtual object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        private object[] ConvertParameters(Request request, ParameterInfo[] methodParameters, out ErrorResponse error)
        {
            var fixedParameters = new object[methodParameters.Length];

            var parameters = (object[])request.Parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var methodParameter = methodParameters[i];

                // Convert the parameter if required.
                if (!methodParameter.ParameterType.IsAssignableFrom(parameter.GetType()))
                {
                    object converted;
                    if (!TryConvert(parameter, methodParameter.ParameterType, out converted))
                    {
                        error = CreateErrorResponse(request.Id, RpcErrorCode.InvalidParameters,
                                                   string.Format(
                                                       "Parameter '{0}' is a '{1}' while the service expected a '{2}'",
                                                       i, parameter.GetType().Name,
                                                       methodParameter.ParameterType.Name));


                        return null;
                    }

                    fixedParameters[i] = converted;
                }
                else
                    fixedParameters[i] = parameters[i];
            }

            error = null;
            return fixedParameters;
        }

        protected virtual bool TryConvert(object sourceValue, Type targetType, out object convertedValue)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(targetType);
            if (!tc.CanConvertFrom(sourceValue.GetType()))
            {
                convertedValue = null;
                return false;
            }

            convertedValue = tc.ConvertFrom(sourceValue);
            return true;
        }


    }

}
