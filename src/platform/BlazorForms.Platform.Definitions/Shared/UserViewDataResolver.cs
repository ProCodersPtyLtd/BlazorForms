using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Platform
{
    // element at index 0 is always reserved for PK value
    // if PK Column is visible - it will be duplicated at its position
    public class UserViewDataResolver : IUserViewDataResolver
    {
        public string[,] ResolveData(FormDetails formDetails, IFlowModel model, ILogStreamer _logStreamer)
        { 
            var tableField = formDetails.Fields.First(f => !string.IsNullOrEmpty(f.Binding.TableBinding));

            if(tableField == null)
            {
                _logStreamer.TrackException(new Exception($"Cannot find table for data resolution in {formDetails.Name}"));
                throw new Exception($"Cannot find table for data resolution in {formDetails.Name}");
            }

            return ResolveData(tableField.Binding.TableBinding, formDetails.Fields, model);
        }

        public string[,] ResolveData(string tableName, IEnumerable<FieldControlDetails> columns, IFlowModel model)
        {
            var jsonText = JsonConvert.SerializeObject(model);
            var json = JObject.Parse(jsonText);
            var tableColumns = columns.Where(f => f.Binding.TableBinding == tableName && f.Binding.BindingType == FieldBindingType.TableColumn && f.DisplayProperties?.Visible == true);
            var pkColumn = columns.FirstOrDefault(f => f.Binding.TableBinding == tableName && f.Binding.BindingType == FieldBindingType.TableColumn && f.DisplayProperties?.IsPrimaryKey == true);
            JToken tableToken = json.SelectToken(tableName);
            var list = tableToken.ToObject<IEnumerable<object>>();
            var rowCount = list?.Count();

            if (rowCount > 0)
            {
                var fisrtRow = tableToken.First;
                // element at index 0 is always reserved for PK value
                var colCount = tableColumns.Count() + 1;// fisrtRow.Count();
                var result = new string[rowCount.Value, colCount];
                var currentRow = fisrtRow;

                for (int row = 0; row < rowCount; row++)
                {
                    // element at index 0 is always reserved for PK value
                    var col = 0;

                    if(pkColumn != null)
                    {
                        var acme = currentRow.SelectToken(pkColumn.Binding.Binding);

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
                        if(column.Binding.Binding == null)
                        {
                            continue;
                        }

                        var acme = currentRow.SelectToken(column.Binding.Binding);

                        if (acme != null)
                        {
                            string val = acme.ToString();
                            result[row, col] = val;
                        }

                        col++;
                    }

                    currentRow = currentRow.Next;
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
