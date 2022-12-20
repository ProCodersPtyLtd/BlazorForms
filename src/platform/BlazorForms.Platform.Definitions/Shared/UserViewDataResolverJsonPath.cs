using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.FastReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Definitions.Shared
{
    public class UserViewDataResolverJsonPath : IUserViewDataResolver
    {
        private readonly IJsonPathNavigator _jsonNavigator;
        private readonly IModelBindingNavigator _bindingNavigator;

        public UserViewDataResolverJsonPath(IJsonPathNavigator jsonNavigator, IModelBindingNavigator navigator)
        {
            _bindingNavigator = navigator;
            _jsonNavigator = jsonNavigator;
        }

        public string[,] ResolveData(FormDetails formDetails, IFlowModel model, ILogStreamer logStreamer)
        {
            var tableField = formDetails.Fields.First(f => !string.IsNullOrEmpty(f.Binding.TableBinding));

            if (tableField == null)
            {
                logStreamer.TrackException(new Exception($"Cannot find table for data resolution in {formDetails.Name}"));
                throw new Exception($"Cannot find table for data resolution in {formDetails.Name}");
            }

            return ResolveData(tableField.Binding.TableBinding, formDetails.Fields, model);
        }

        public string[,] ResolveData(string tableName, IEnumerable<FieldControlDetails> columns, IFlowModel model)
        {
            var list = _jsonNavigator.GetItems(model, tableName).ToList();
            //var fields = columns.Where(f => !string.IsNullOrEmpty(f.Binding.TableBinding));
            var tableColumns = columns.Where(f => f.Binding.TableBinding == tableName && f.Binding.BindingType == FieldBindingType.TableColumn && f.DisplayProperties?.Visible == true);
            var pkColumn = columns.FirstOrDefault(f => f.Binding.TableBinding == tableName && f.Binding.BindingType == FieldBindingType.TableColumn && f.DisplayProperties?.IsPrimaryKey == true);
            var rowCount = list?.Count();

            if (rowCount > 0)
            {
                var colCount = tableColumns.Count() + 1;// fisrtRow.Count();
                var result = new string[rowCount.Value, colCount];

                for (int row = 0; row < rowCount; row++)
                {
                    // element at index 0 is always reserved for PK value
                    var col = 0;
                    var item = list[row];

                    if (pkColumn != null)
                    {
                        //var acme = currentRow.SelectToken(pkColumn.Binding.Binding);
                        var acme = _bindingNavigator.GetValue(item, pkColumn.Binding);

                        if (acme != null)
                        {
                            string val = acme.ToString();
                            result[row, col] = val;
                        }
                    }

                    col++;

                    foreach (var column in tableColumns)
                    {
                        // ignore column-table definition
                        if (column.Binding.Binding == null)
                        {
                            continue;
                        }

                        //var acme = currentRow.SelectToken(column.Binding.Binding);
                        var acme = _bindingNavigator.GetValue(item, column.Binding);

                        if (acme != null)
                        {
                            if (string.IsNullOrWhiteSpace(column.DisplayProperties?.Format))
                            {
                                result[row, col] = acme.ToString(); 
                            }
                            else
                            {
                                var value = acme as IFormattable;

                                if (value != null)
                                {
                                    result[row, col] = value.ToString(column.DisplayProperties.Format, null);
                                }
                            }
                        }

                        col++;
                    }
                }

                return result;
            }
            else
            {
                return new string[0, 0];
            }
        }
    }
}
