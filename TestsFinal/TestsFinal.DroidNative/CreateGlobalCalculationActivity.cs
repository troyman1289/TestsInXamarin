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
    [Activity(Label = "CreateGlobalCalculationActivity", Theme = "@style/MainTheme")]
    public class CreateGlobalCalculationActivity : Activity
    {

        private EditText _labelEditText;
        private EditText _startOperandEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateGlobalCalculation);

            _labelEditText = FindViewById<EditText>(Resource.Id.LabelEditText);
            _startOperandEditText = FindViewById<EditText>(Resource.Id.StartOperandEditText);

            var saveButton = FindViewById<Button>(Resource.Id.ButtonSave);
            saveButton.Click += HandleSave;
        }

        private void HandleSave(object sender, EventArgs e)
        {
            var newGlobalCalculation = new GlobalCalculation();
            newGlobalCalculation.Label = _labelEditText.Text;

            decimal startOperand = 0;
            decimal.TryParse(_startOperandEditText.Text, out startOperand);
            CustomLocator.CalculationManager.AddNewGlobalCalculation(newGlobalCalculation, startOperand);
            Finish();
        }
    }
}