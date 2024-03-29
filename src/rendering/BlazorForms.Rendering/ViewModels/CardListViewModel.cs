﻿using BlazorForms.Forms;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.ViewModels
{
    public class CardListViewModel : ControlViewModel
    {
        protected bool _isEdititng;
        protected int _edititngRowIndex;

        public int EdititngRowIndex => _edititngRowIndex;
        public bool IsEditing => _isEdititng;

        public string EditingTextValue { get; set; }
        public object ModelUntyped { get; set; }
        public FieldControlDetails ListControl { get; set; }

        public async Task<bool> ApplyChanges()
        {
            if (!IsEditing)
            {
                return true;
            }

            var model = ModelUntyped;
            var table = ListControl;
            var viewModel = _formViewModel;
            var rowIndex = _edititngRowIndex;
            var bodyTextValue = EditingTextValue;
            var fieldSet = viewModel.Lists[table.Binding.TableBinding].First();
            var body = fieldSet.FindField(ControlType.CardBody);

            if (body != null)
            {
                viewModel.GetRowField(table, rowIndex);
                viewModel.FieldSetValue(model, body.Binding, bodyTextValue);

                try
                {
                    var task = await viewModel.TriggerRules(viewModel.FormData.ProcessTaskTypeFullName, table.Binding, FormRuleTriggers.ItemChanged, 
                        rowIndex);

                    if (!task.SkipThisChange)
                    {
                        viewModel.SetInputChanged();
                    }
                }
                catch (Exception exc)
                {
                    viewModel.PopulateException(exc);
                    return false;
                }
            }

            FinishEdit();
            return true;
        }

        public override async Task Close()
        {
            await ApplyChanges();
        }

        public override bool PreventCloseWithoutSave()
        {
            return _isEdititng;
        }

        public void RemoveAt(int rowIndex)
        {
            if (IsEditing && rowIndex < _edititngRowIndex)
            {
                _edititngRowIndex--;
            }
        }

        public bool IsEditingRow(int rowIndex)
        {
            return _isEdititng && _edititngRowIndex == rowIndex;
        }

        public bool StartEdit(int rowIndex)
        {
            if (_isEdititng)
            {
                return false;
            }

            _isEdititng = true;
            _edititngRowIndex = rowIndex;
            return true;
        }

        public void FinishEdit()
        { 
            _isEdititng = false;
        }
    }
}
