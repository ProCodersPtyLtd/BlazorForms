namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public interface IDocumentApi
    {
        /// <summary>
        /// Returns differences in model stored in the database
        /// </summary>
        /// <param name="modelProperies">
        /// Current model properties to check
        /// </param>
        /// <returns></returns>
        List<ModelChangeDetails> GetModelChanges(ModelProperies modelProperies);
    }

    public class DocumentApi : IDocumentApi
    {
        internal readonly static ModelProperies _dataCache = new ModelProperies();

        public List<ModelChangeDetails> GetModelChanges(ModelProperies modelProperies)
        {
            var result = new List<ModelChangeDetails>();

            foreach (var item in modelProperies.List)
            {
                var cacheItem = _dataCache.List.FirstOrDefault(i => i.Id == item.Id);

                if (cacheItem != null && item.Bindnig == cacheItem.Bindnig && item.Value != cacheItem.Value)
                {
                    result.Add(new ModelChangeDetails { Id = item.Id, Bindnig = item.Bindnig, OldValue = item.Value, Value = cacheItem.Value });
                }
            }

            return result;
        }
    }

    public class ModelChangeDetails
    {
        public string Id { get;set; }
        public string Bindnig { get;set; }
        public string OldValue { get;set; }
        public string Value { get;set; }
    }

    public class ModelProperies
    {
        public List<ModelPropertyDetails> List { get; private set; } = new List<ModelPropertyDetails>();
    }

    public class ModelPropertyDetails
    {
        public string Id { get; set; }
        public string Bindnig { get; set; }
        public string Value { get; set; }
    }
}
