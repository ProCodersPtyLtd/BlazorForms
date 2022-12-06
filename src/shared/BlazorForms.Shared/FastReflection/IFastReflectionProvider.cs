using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.FastReflection
{
    public interface IFastReflectionProvider
    {
        Func<object, object> GetStraightEmitterGet(Type modelType, string binding);
        Action<object, object> GetStraightEmitterSet(Type modelType, string binding);
        void UpdateBindingFastReflection(FieldBinding binding, Type modelType);
    }
}
