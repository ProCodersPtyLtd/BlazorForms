namespace BlazorForms.Flows.Definitions
{
    public class FlowParamsGeneric : FlowParamsBase
    {
        public string GetParam(string key)
        {
            if (DynamicInput.ContainsKey(key))
            {
                return DynamicInput[key];
            }

            return null;
        }

        public int? GetParamInt(string key)
        {
            var stringParam = GetParam(key);

            if (int.TryParse(stringParam, out int intParam))
            {
                return intParam;
            }

            return null;
        }
    }
}
