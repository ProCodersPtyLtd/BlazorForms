using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Extensions
{
    public enum FieldFilterType
    {
        None,
        Text,
        TextStarts,
        TextContains,
        TextEnds,
        NumExpression,
        DateExpressionEqual,
        DateExpressionToDate,
        DateExpressionFromDate,
        DateExpressionRange,        
        Select,
        MultiSelect,
        DecimalEqual,
        DecimalLessThan,
        DecimalGreaterThan,
        DecimalRange
    }

    public enum FieldFilterPositionType
    {
        FirstControl,
        SecondControl
    }
}
