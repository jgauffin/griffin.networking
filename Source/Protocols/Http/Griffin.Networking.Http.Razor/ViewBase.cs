using Griffin.Networking.Protocol.Http.Server;

namespace Griffin.Networking.Protocol.Http.Razor
{
    public class ViewBase<T>
    {
        public ViewBase(T model, IItemStorage viewData)
        {
            Model = model;
            ViewData = viewData;
        }

        public IItemStorage ViewData { get; set; }

        public T Model { get; private set; }
    }
}