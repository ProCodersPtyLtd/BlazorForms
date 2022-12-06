namespace BlazorForms.Proxyma
{
    public interface IModelProxyInterceptor
    {
        bool InterceptSetter(IProxyPropertyBagStore obj, string prop, object val);
    }
}
