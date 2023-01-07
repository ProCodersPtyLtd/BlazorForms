using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    // ToDo: should be reviewed and tested - broken by flowRunId removal refactoring
    public class FlowEntityWrapper
    {
        public string ModelFullName { get; set; }
        public virtual object ModelUntyped { get => null; }

        public string id { get; set; }
        public string RefId { get; set; }
        public string FlowName { get; set; }
        //public virtual object RecordsUntyped { get; set; }

        public static FlowEntityWrapper CreateInstance(FlowEntity source)
        {
            Type generic = typeof(FlowEntityWrapper<>);
            var targetType = generic.MakeGenericType(new Type[] { source.Context.Model.GetType() });
            var dto = Activator.CreateInstance(targetType, source, source.Context.Model) as FlowEntityWrapper;
            return dto;
        }

        public static FlowEntity Deserialize(string json, IKnownTypesBinder knownTypesBinder)
        {
            var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
            var modelTypeName = obj.ModelFullName;
            var mt = typeof(NullFlowModel);

            if (modelTypeName != null)
            {
                mt = knownTypesBinder.KnownTypesDict[modelTypeName];
            }

            var targetType = typeof(FlowEntityWrapper<>).MakeGenericType(new Type[] { mt });
            var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as FlowEntityWrapper;
            
            var fullFlowEntity = new FlowEntity
            {
                id = wrapper.id,
                RefId = wrapper.RefId,
                FlowName = wrapper.FlowName,
                //Context = wrapper.Context,
                //Records = new Collection<FlowContext>()
            };

            fullFlowEntity.Context.Model = wrapper.ModelUntyped as IFlowModel;

            // ToDo: need to be fixed after FlowEntity refactoring
            //foreach (var record in (wrapper.RecordsUntyped as IEnumerable))
            //{
            //    var contextWrapper = record as FlowContextWrapper;
            //    var context = new FlowContext(contextWrapper, contextWrapper.ModelUntyped as IFlowModel);
            //    fullFlowEntity.LastContext = context;
            //    fullFlowEntity.Records.Add(context);
            //}

            return fullFlowEntity;
        }
    }

    public class FlowEntityWrapper<T> : FlowEntityWrapper
        where T : class, IFlowModel
    {
        public T LastModel { get; set; }

        public override object ModelUntyped { get => LastModel; }

        public List<FlowContextWrapper<T>> FlowContextWrapperRecords { get; set; }

        // ToDo: need to be fixed after FlowEntity refactoring
        //public override object RecordsUntyped { get => FlowContextWrapperRecords; }

        public FlowEntityWrapper()
        { }

        public FlowEntityWrapper(FlowEntity source, IFlowModel model)
        {
            id = source.id;
            RefId = source.RefId;
            FlowName = source.FlowName;
            FlowContextWrapperRecords = new List<FlowContextWrapper<T>>();

            // ToDo: need to be fixed after FlowEntity refactoring
            //foreach (var record in source.Records)
            //{
            //    var wrapperRecord = FlowContextWrapper.CreateInstance(record) as FlowContextWrapper<T>;
            //    FlowContextWrapperRecords.Add(wrapperRecord);
            //}

            LastModel = model as T;
            ModelFullName = typeof(T).FullName;
        }
    }
}
