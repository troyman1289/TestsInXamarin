using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backend.Model;

namespace TestsFinal.DroidNative
{
    [Activity(Label = "CreateGlobalCalculationActivity")]
    public class CreateGlobalCalculationActivity : Activity
    {

        private EditText _labelEditText;
        private EditText _startOperandEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateGlobalCalculation);

            _labelEditText = FindViewById<EditText>(Resource.Id.LabelEditText);
            //Bindings.Add(this.SetBinding(() => ViewModel.Description, () => _descriptionEditText.Text, BindingMode.TwoWay));

            _startOperandEditText = FindViewById<EditText>(Resource.Id.StartOperandEditText);
            //Bindings.Add(this.SetBinding(() => ViewModel.Creator, () => _creatorEditText.Text, BindingMode.TwoWay));

            var saveButton = FindViewById<Button>(Resource.Id.ButtonSave);
            saveButton.Click += HandleSave;
            //saveButton.SetCommand("Click", ViewModel.SaveShoppingListElementCommand);
        }

        private void HandleSave(object sender, EventArgs e)
        {
            var newGlobalCalculation = new GlobalCalculation();
            newGlobalCalculation.Label = _labelEditText.Text;

            double startOperand = 0;
            double.TryParse(_startOperandEditText.Text, out startOperand);
            CustomLocator.CalculationManager.AddNewGlobalCalculation(newGlobalCalculation, startOperand);
            Finish();
        }
    }
}